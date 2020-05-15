using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace AioiLight.Anna_Hiiragi
{
    class Program
    {
        private static string Token
        {
            get
            {
                return File.ReadAllText("token.txt", Encoding.UTF8);
            }
        }

        static readonly HttpClient Client = new HttpClient();

        private static Config Config { get; set; }

        static async Task Main()
        {
            if (File.Exists("config.json"))
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json", Encoding.UTF8));
            }
            else
            {
                Config = new Config();
            }

            var client = new DiscordSocketClient();

            client.MessageReceived += Client_MessageReceived;

            await client.LoginAsync(TokenType.Bot, Token);
            await client.StartAsync();

            Console.ReadLine();
        }

        static async Task Client_MessageReceived(SocketMessage arg)
        {
            var server = (arg.Channel as SocketGuildChannel).Guild;

            {
                var hasConfig = Config.Servers.Where(s => s.ServerID == server.Id);
                if (hasConfig.Any())
                {
                    var cfg = hasConfig.First();
                    if (arg.Channel.Id == cfg.WatchChannel)
                    {
                        try
                        {
                            var response = await Client.GetAsync(arg.Content);
                            response.EnsureSuccessStatusCode();
                            var responseBody = await response.Content.ReadAsStringAsync();

                            var parser = new HtmlParser();
                            var document = await parser.ParseDocumentAsync(responseBody);

                            var h1 = document.GetElementsByTagName("h1");
                            var title = h1.Last().TextContent.Trim();

                            var monster = new Monster[] { cfg.ClanBattle[0], cfg.ClanBattle[1], cfg.ClanBattle[2], cfg.ClanBattle[3], cfg.ClanBattle[4] };

                            var match = monster.Where(m => m.Hook.Where(h => title.Contains(h)).Any());

                            if (match.Any())
                            {
                                var result = match.First();

                                var channel = server.GetChannel(result.ChannelID) as ISocketMessageChannel;

                                await  channel.SendMessageAsync(arg.Content);
                                return;
                            }
                        }
                        catch (Exception e)
                        {
                            return;
                        }
                    }
                }
            }

            if (arg.Content.StartsWith("!anna init"))
            {
                var roleID = Convert.ToUInt64(arg.Content.Substring("!anna init ".Length));
                if (Config.Servers.Where(s => s.ServerID == server.Id).Any())
                {
                    await arg.Channel.SendMessageAsync($"{server.Name} はすでに初期設定が済んでいます！\n" +
                        $"リセットする場合は(権限のあるユーザーで)``!anna reset``");
                    return;
                }
                else
                {
                    Config.Servers.Add(new Server() { ServerID = server.Id, UserRole = roleID });
                    var role = server.Roles.Where(r => r.Id == roleID).First();
                    await arg.Channel.SendMessageAsync($"初期設定が完了しました。Bot を操作できる役職: {role.Name}");
                    return;
                }
            }

            if (arg.Content.StartsWith("!anna reset"))
            {
                var hasConfig = Config.Servers.Where(s => s.ServerID == server.Id);
                if (hasConfig.Any())
                {
                    var cfg = hasConfig.First();
                    if (cfg.ServerID == server.Id && server.GetRole(cfg.UserRole).Members.Where(m => m.Id == arg.Author.Id).Any())
                    {
                        Config.Servers.Remove(cfg);
                        await arg.Channel.SendMessageAsync($"{server.Name} の設定を削除しました。");
                        return;
                    }
                    else
                    {
                        await arg.Channel.SendMessageAsync($"{server.Name} の設定を削除できません。\n" +
                            $"役職: ${server.GetRole(cfg.UserRole).Name} 実行するユーザーに必要です。");
                        return;
                    }
                }
                else
                {
                    await arg.Channel.SendMessageAsync($"{server.Name} の設定はすでにありません。\n");
                    return;
                }
            }

            if (arg.Content.StartsWith("!anna"))
            {
                var hasConfig = Config.Servers.Where(s => s.ServerID == server.Id);
                if (!hasConfig.Any())
                {
                    await arg.Channel.SendMessageAsync($"{server.Name} の初期設定が済んでいません！\n" +
                        $"``!anna init <roleID>``で初期設定");
                    return;
                }

                var cfg = hasConfig.First();
                if (cfg.ServerID != server.Id || !server.GetRole(cfg.UserRole).Members.Where(m => m.Id == arg.Author.Id).Any())
                {
                    await arg.Channel.SendMessageAsync($"設定を行う権限がありません。\n" +
                            $"役職: ${server.GetRole(cfg.UserRole).Name} 実行するユーザーに必要です。");
                    return;
                }

                var command = arg.Content.Substring("!anna ".Length);
                var param = command.Split(' ');
                var paramList = param.ToList();
                paramList.RemoveAll(f => string.IsNullOrWhiteSpace(f));
                param = paramList.ToArray();

                if (param.Count() >= 1)
                {
                    if (param[0] == "test")
                    {
                        if (param.Count() >= 2)
                        {
                            try
                            {
                                var response = await Client.GetAsync(param[1]);
                                response.EnsureSuccessStatusCode();
                                var responseBody = await response.Content.ReadAsStringAsync();

                                var parser = new HtmlParser();
                                var document = await parser.ParseDocumentAsync(responseBody);

                                var h1 = document.GetElementsByTagName("h1");
                                var title = h1.Last().TextContent.Trim();

                                await arg.Channel.SendMessageAsync($"テスト:\n" +
                                    $"URL: <{param[1]}>\n" +
                                    $"タイトル: {title}");
                                return;
                            }
                            catch (Exception e)
                            {
                                await arg.Channel.SendMessageAsync($"動画の解析ができませんでした……。\n" +
                                    $"URL: <{param[1]}>\n" +
                                    $"エラー: {e.Message}");
                                return;
                            }
                        }
                    }
                    else if (param[0] == "watch")
                    {
                        if (param.Count() >= 2)
                        {
                            var ch = server.Channels.Where(c => c.Id == Convert.ToUInt64(param[1]));
                            if (ch.Any())
                            {
                                cfg.WatchChannel = Convert.ToUInt64(ch.First().Id);
                                await arg.Channel.SendMessageAsync($"動画を監視するチャンネルを <#{ch.First().Id}> に設定しました。");
                                return;
                            }
                            else
                            {
                                await arg.Channel.SendMessageAsync($"エラー: チャンネルが存在しません。");
                                return;
                            }
                        }
                    }
                    else if (param[0] == "setch")
                    {
                        if (param.Count() >= 3)
                        {
                            var ch = server.Channels.Where(c => c.Id == Convert.ToUInt64(param[2]));
                            if (ch.Any())
                            {
                                cfg.ClanBattle[Convert.ToInt32(param[1]) - 1].ChannelID = Convert.ToUInt64(ch.First().Id);
                                await arg.Channel.SendMessageAsync($"クランバトル{param[1]}体目の動画を送信するチャンネルを <#{ch.First().Id}> に設定しました。");
                                return;
                            }
                            else
                            {
                                await arg.Channel.SendMessageAsync($"エラー: チャンネルが存在しません。");
                                return;
                            }
                        }
                    }
                    else if (param[0] == "name")
                    {
                        if (param.Count() >= 3)
                        {
                            cfg.ClanBattle[Convert.ToInt32(param[1]) - 1].Hook = param[2].Split(',');
                            await arg.Channel.SendMessageAsync($"クランバトル{param[1]}体目の名前を {string.Join("/", param[2].Split(','))} に設定しました。");
                            return;
                        }
                    }
                    else if (param[0] == "save")
                    {
                        var json = JsonConvert.SerializeObject(Config);
                        File.WriteAllText("config.json", json, Encoding.UTF8);
                        await arg.Channel.SendMessageAsync($"設定を保存しました。");
                        return;
                    }
                }
            }
        }
    }
}

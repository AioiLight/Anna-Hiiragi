using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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

        private static Config Config { get; set; }

        static async Task Main()
        {
            if (File.Exists("config.json"))
            {
                Config = JsonConvert.DeserializeObject<Config>("config.json");
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
            if (arg.Content.StartsWith("!anna"))
            {
                await arg.Channel.SendMessageAsync("Hello, world!");
            }
        }
    }
}

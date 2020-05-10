namespace AioiLight.Anna_Hiiragi
{
    public class Server
    {
        public Server()
        {
            ClanBattle = new ClanBattle();
        }

        public ulong ServerID { get; set; }
        public ulong UserRole { get; set; }
        public ulong WatchChannel { get; set; }
        public ClanBattle ClanBattle { get; set; }
    }
}

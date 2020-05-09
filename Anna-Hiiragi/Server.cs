namespace AioiLight.Anna_Hiiragi
{
    public class Server
    {
        public Server()
        {
            ClanBattle = new ClanBattle();
        }

        public string UserRole { get; set; }
        public string WatchChannel { get; set; }
        public ClanBattle ClanBattle { get; set; }
    }
}

using System.Collections.Generic;

namespace AioiLight.Anna_Hiiragi
{
    public class Config
    {
        public Config()
        {
            Servers = new List<Server>();
        }

        public List<Server> Servers { get; set; }
    }
}

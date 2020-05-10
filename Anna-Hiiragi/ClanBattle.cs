using System;
using System.Collections.Generic;
using System.Text;

namespace AioiLight.Anna_Hiiragi
{
    public class ClanBattle
    {
        public ClanBattle()
        {
            Monster1 = new Monster();
            Monster2 = new Monster();
            Monster3 = new Monster();
            Monster4 = new Monster();
            Monster5 = new Monster();
        }

        public Monster Monster1 { get; set; }
        public Monster Monster2 { get; set; }
        public Monster Monster3 { get; set; }
        public Monster Monster4 { get; set; }
        public Monster Monster5 { get; set; }

        public Monster this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Monster1;
                    case 1:
                        return Monster2;
                    case 2:
                        return Monster3;
                    case 3:
                        return Monster4;
                    case 4:
                        return Monster5;
                    default:
                        return Monster1;
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        Monster1 = value;
                        break;
                    case 1:
                        Monster2 = value;
                        break;
                    case 2:
                        Monster3 = value;
                        break;
                    case 3:
                        Monster4 = value;
                        break;
                    case 4:
                        Monster5 = value;
                        break;
                    default:
                        Monster1 = value;
                        break;
                }
            }
        }
    }
}

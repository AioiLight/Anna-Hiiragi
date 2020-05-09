using System;
using System.Collections.Generic;
using System.Text;

namespace AioiLight.Anna_Hiiragi
{
    public class ClanBattle
    {
        public string Monster1 { get; set; }
        public string Monster2 { get; set; }
        public string Monster3 { get; set; }
        public string Monster4 { get; set; }
        public string Monster5 { get; set; }

        public string this[int index]
        {
            get
            {
                switch (index)
                {
                    case 1:
                        return Monster1;
                    case 2:
                        return Monster2;
                    case 3:
                        return Monster3;
                    case 4:
                        return Monster4;
                    case 5:
                        return Monster5;
                    default:
                        return Monster1;
                }
            }

            set
            {
                switch (index)
                {
                    case 1:
                        Monster1 = value;
                        break;
                    case 2:
                        Monster2 = value;
                        break;
                    case 3:
                        Monster3 = value;
                        break;
                    case 4:
                        Monster4 = value;
                        break;
                    case 5:
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

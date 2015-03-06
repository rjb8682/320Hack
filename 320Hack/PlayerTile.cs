using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _320Hack
{
    class PlayerTile
    {
        private String name;
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private static Tuple<int, int> position;
        public static Tuple<int, int> Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public PlayerTile(String name)
        {
            this.name = name;
            Position = new Tuple<int, int>(2, 37);
        }
        
    }
}

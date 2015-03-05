using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _320Hack
{
    class Map
    {

        private List<List<Char>> levelMap;
        public List<List<Char>> LevelMap
        {
            get
            {
                return this.levelMap;
            }

            set
            {
                this.levelMap = value;
            }
        }

        public Map(List<List<Char>> map)
        {
            this.levelMap = map;
        }

        public String printMap()
        {
            String resultLevel = "";

            foreach (List<Char> list in levelMap)
            {
                foreach (Char c in list)
                {
                    resultLevel += c;
                }
                resultLevel += '\n';
            }
            return resultLevel;
        }
    }
}

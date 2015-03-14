using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _320Hack
{
    public class Monster
    {
        public int Id { get; set; }

        [Required]
        public String Symbol { get; set; }

        public int HP { get; set; }

    }

    public class Room
    {
        public int Id { get; set; }

        public String Map { get; set; }

        public byte[] Seen { get; set; }

        [NotMapped]
        public List<List<Char>> LevelChars { get; set; }

        [NotMapped]
        public List<List<Tile>> LevelTiles { get; set; }

        public Boolean IsTileSeen (int i) {
            return !Seen[i].Equals(Convert.ToByte(0));
        }

        public void UpdateSeenValues (List<List<Tile>> map)
        {
            int i = 0;
            foreach (List<Tile> row in map)
            {
                foreach (Tile tile in row)
                {
                    Seen[i++] = tile.Seen ? Convert.ToByte(1) : Convert.ToByte(0);
                }
            }
        }

        public void buildLevelChars()
        {
            LevelChars = new List<List<Char>>();
            List<Char> currentRow = new List<Char>();

            foreach (Char c in Map)
            {
                if (c == '\n')
                {
                    LevelChars.Add(currentRow);
                    currentRow = new List<Char>();
                }
                else
                {
                    currentRow.Add(c);
                }
            }
            LevelChars.Add(currentRow);
        }

        public void buildLevelTiles()
        {
            int i = 0;
            LevelTiles = new List<List<Tile>>();

            foreach (List<Char> list in LevelChars)
            {
                List<Tile> currentRow = new List<Tile>();
                foreach (Char c in list)
                {
                    Tile t = new Tile(c);
                    t.Seen = IsTileSeen(i++);
                    currentRow.Add(t);
                }
                LevelTiles.Add(currentRow);
            }
        }
    }

    public class Player
    {
        public int Id { get; set; }

        public int CurrentRoom { get; set; }

        public int LastRoom { get; set; }

        public int Row { get; set; }

        public int Col { get; set; }

        public int Experience { get; set; }

        public int Health { get; set; }

        public String Name { get; set; }
    }

    public class Door
    {
        public int Id { get; set; }

        public int Row { get; set; }

        public int Col { get; set; }

        public int LivesIn { get; set; }

        public int ConnectsTo { get; set; }
    }
}

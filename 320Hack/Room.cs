using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _320Hack
{
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

        public void setupMap()
        {
            buildLevelChars();
            buildLevelTiles();
        }

        private void buildLevelChars()
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

        private void buildLevelTiles()
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

        public void reset()
        {
            Seen = new byte[Map.Length];
        }
    }
}

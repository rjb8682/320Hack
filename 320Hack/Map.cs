using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _320Hack
{
    class Map
    {
        private int PlayerStartRow = 2;
        private int PlayerStartCol = 37;

        private int playerRow;
        private int playerCol;

        private List<List<Tile>> levelMap;
        public List<List<Tile>> LevelMap
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

        public Map(List<List<Tile>> map)
        {
            map[playerRow = PlayerStartRow][playerCol = PlayerStartCol] = new Tile('@');
            this.levelMap = map;
        }

        public String printMap()
        {
            String resultLevel = "";

            updateSeen();

            foreach (List<Tile> list in levelMap)
            {
                foreach (Tile c in list)
                {
                    if (c.Seen || c.Symbol == '@')
                        resultLevel += c.Symbol;
                    else
                        resultLevel += ' ';
                }
                resultLevel += '\n';
            }
            return resultLevel;
        }

        public void movePlayer(int dir)
        {
            if (dir == MainWindow.UP)
            {
                if (levelMap[playerRow - 1][playerCol].Symbol == MainWindow.floor)
                {
                    Tile temp = levelMap[playerRow - 1][playerCol];
                    Tile player = levelMap[playerRow][playerCol];
                    levelMap[playerRow--][playerCol] = temp;
                    levelMap[playerRow][playerCol] = player;
                }
            }
            else if (dir == MainWindow.DOWN)
            {
                if (levelMap[playerRow + 1][playerCol].Symbol == MainWindow.floor)
                {
                    Tile temp = levelMap[playerRow + 1][playerCol];
                    Tile player = levelMap[playerRow][playerCol];
                    levelMap[playerRow++][playerCol] = temp;
                    levelMap[playerRow][playerCol] = player;
                }
            }
            else if (dir == MainWindow.LEFT)
            {
                if (levelMap[playerRow][playerCol - 1].Symbol == MainWindow.floor)
                {
                    Tile temp = levelMap[playerRow][playerCol - 1];
                    Tile player = levelMap[playerRow][playerCol];
                    levelMap[playerRow][playerCol--] = temp;
                    levelMap[playerRow][playerCol] = player;
                }
            }
            else if (dir == MainWindow.RIGHT)
            {
                if (levelMap[playerRow][playerCol + 1].Symbol == MainWindow.floor)
                {
                    Tile temp = levelMap[playerRow][playerCol + 1];
                    Tile player = levelMap[playerRow][playerCol];
                    levelMap[playerRow][playerCol++] = temp;
                    levelMap[playerRow][playerCol] = player;
                }
            }
        }

        public void updateSeen()
        {
            // Update the tiles in levelMap to be seen if the player is near them
            int numRows = levelMap.Count;

            for (int i = 0; i < numRows; i++)
            {
                int numColsForThisRow = levelMap[i].Count;
                for (int j = 0; j < numColsForThisRow; j++)
                {
                    if (!levelMap[i][j].Seen)
                    {
                        int taxiValue = Math.Abs(i - playerRow) + Math.Abs(j - playerCol);
                        if (taxiValue < 5)
                        {
                            levelMap[i][j].Seen = true;
                        }
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _320Hack
{
    class Map
    {
        //private int PlayerStartRow = 2;
        //private int PlayerStartCol = 37;
        private int PlayerStartRow = 9;
        private int PlayerStartCol = 4;

        private int playerRow;
        private int playerCol;
        private Coordinate playerCoord;

        private Coordinate monster;

        private List<char> floorTiles;

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
            map[playerRow = PlayerStartRow][playerCol = PlayerStartCol] = new Tile(MainWindow.player);
            monster = new Coordinate(PlayerStartRow + 2, PlayerStartCol + 4);
            //map[monster.row][monster.col] = new Tile('o');
            this.levelMap = map;
        }

        public void swap(int r1, int c1, int r2, int c2)
        {
            Tile temp = levelMap[r1][c1];
            levelMap[r1][c1] = levelMap[r2][c2];
            levelMap[r2][c2] = temp;
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
                    swap(playerRow, playerCol, --playerRow, playerCol);
                }
            }
            else if (dir == MainWindow.DOWN)
            {
                if (levelMap[playerRow + 1][playerCol].Symbol == MainWindow.floor)
                {
                    swap(playerRow, playerCol, ++playerRow, playerCol);
                }
            }
            else if (dir == MainWindow.LEFT)
            {
                if (levelMap[playerRow][playerCol - 1].Symbol == MainWindow.floor)
                {
                    swap(playerRow, playerCol, playerRow, --playerCol);
                }
            }
            else if (dir == MainWindow.RIGHT)
            {
                if (levelMap[playerRow][playerCol + 1].Symbol == MainWindow.floor)
                {
                    swap(playerRow, playerCol, playerRow, ++playerCol);
                }
            }
            else if (dir == MainWindow.UP_LEFT)
            {
                if (levelMap[playerRow - 1][playerCol - 1].Symbol == MainWindow.floor)
                {
                    swap(playerRow, playerCol, --playerRow, --playerCol);
                }
            }
            else if (dir == MainWindow.UP_RIGHT)
            {
                if (levelMap[playerRow - 1][playerCol + 1].Symbol == MainWindow.floor)
                {
                    swap(playerRow, playerCol, --playerRow, ++playerCol);
                }
            }
            else if (dir == MainWindow.DOWN_LEFT)
            {
                if (levelMap[playerRow + 1][playerCol - 1].Symbol == MainWindow.floor)
                {
                    swap(playerRow, playerCol, ++playerRow, --playerCol);
                }
            }
            else if (dir == MainWindow.DOWN_RIGHT)
            {
                if (levelMap[playerRow + 1][playerCol + 1].Symbol == MainWindow.floor)
                {
                    swap(playerRow, playerCol, ++playerRow, ++playerCol);
                }
            }

            playerCoord = new Coordinate(playerRow, playerCol);

            List<char> floorOrPlayer = new List<char>();
            floorOrPlayer.Add(MainWindow.floor);
            floorOrPlayer.Add(MainWindow.player);
            Coordinate monsterNext = shortestPathTo(monster, playerCoord, floorOrPlayer);
            if (monsterNext == playerCoord)
            {
                // Here is where the monster would attack the player (since it's adjacent to it)
            }
            else
            {
                // TODO clean up ugly code like this (make a swap / move method?)
                Tile monsterNew = levelMap[monsterNext.row][monsterNext.col];
                Tile monsterTile = levelMap[monster.row][monster.col];
                levelMap[monster.row][monster.col] = monsterNew;
                levelMap[monsterNext.row][monsterNext.col] = monsterTile;
                monster = monsterNext;
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
                    //levelMap[i][j].Seen = false;
                    if (!levelMap[i][j].Seen) levelMap[i][j].Seen = canSeeTile(i, j);
                }
            }
        }

        private bool canSeeTile(int r, int c)
        {
            if (playerRow == r && playerCol == c) return true;
            int rowSign = playerRow - r > 0 ? -1 : 1;
            int colSign = playerCol - c > 0 ? -1 : 1;

            // If the tile in question is in the same column, iterate from playerRow to this row.
            if (playerCol == c)
            {
                for (int row = playerRow; row != r; row += rowSign)
                {
                    if (!isValidCoordinate(row, playerCol)) return false;
                    Tile t = levelMap[row][playerCol];
                    if (t.Symbol != MainWindow.floor && t.Symbol != 'o' && t.Symbol != MainWindow.player)
                    {
                        return false;
                    }
                }

                // Don't bother with the slope, since it's infinity.
                return true;
            }

            double slope = Math.Abs((playerRow - r) * 1.0 / (playerCol - c));
            double currentRow = Convert.ToDouble(playerRow);

            for (int col = playerCol; col != c; col += colSign)
            {
                int newRow = Convert.ToInt32(currentRow + 0.0);

                if (!isValidCoordinate(newRow, col)) { return false; }

                Tile t = levelMap[newRow][col];
                if (t.Symbol != MainWindow.floor && 
                    t.Symbol != 'o' && 
                    t.Symbol != MainWindow.player) {
                    return false;
                }

                currentRow += rowSign * slope;
            }

            return true;
        }

        /**
         * Returns all tiles neighboring start. Only counts 'floor' as reachable.
         */
        public List<Coordinate> neighborCoordinates(Coordinate start, List<char> validSpaces)
        {
            return neighborCoordinates(start.row, start.col, validSpaces);
        }

        /**
         * Returns all tiles neighboring row and col. Only counts 'floor' or the player as reachable.
         * Result if of form (int[] {row, col}).
         */
        public List<Coordinate> neighborCoordinates(int r, int c, List<char> validTypes)
        {
            List<Coordinate> result = new List<Coordinate>();

            if (isSpaceType(r, c + 1, validTypes))
            {
                result.Add(new Coordinate(r, c + 1));
            }
            if (isSpaceType(r, c - 1, validTypes))
            {
                result.Add(new Coordinate(r, c - 1));
            }
            if (isSpaceType(r + 1, c, validTypes))
            {
                result.Add(new Coordinate(r + 1, c));
            }
            if (isSpaceType(r - 1, c, validTypes))
            {
                result.Add(new Coordinate(r - 1, c));
            }

            // Difficulty toggle for the future
            if (true)
            {
                if (isSpaceType(r + 1, c + 1, validTypes))
                {
                    result.Add(new Coordinate(r + 1, c + 1));
                }
                if (isSpaceType(r + 1, c - 1, validTypes))
                {
                    result.Add(new Coordinate(r + 1, c - 1));
                }
                if (isSpaceType(r - 1, c + 1, validTypes))
                {
                    result.Add(new Coordinate(r - 1, c + 1));
                }
                if (isSpaceType(r - 1, c - 1, validTypes))
                {
                    result.Add(new Coordinate(r - 1, c - 1));
                }
            }

            return result;
        }

        /**
         * Returns true iff the coordinate is valid on this map.
         */
        public Boolean isValidCoordinate(int r, int c)
        {
            return r > 0 && r < levelMap.Count && c > 0 && c < levelMap[r].Count;
        }

        /**
         * Returns true iff the given space is in the list of given types (checks validity first).
         */
        private Boolean isSpaceType(int r, int c, List<Char> validTypes)
        {
            return isValidCoordinate(r, c) && validTypes.Contains(levelMap[r][c].Symbol);
        }

        /**
         * Performs a BFS from the start to any coordinate in finish.
         * Returns the next tile to move to on the shortest path,
         * or start if finish is not reachable.
         * validSpaces is a list of chars that should be considered neighbors.
         * (e.g. for AI pathfinding, that should be ['.', '@'].
         */
        public Coordinate shortestPathTo(Coordinate start, Coordinate finish, List<char> validSpaces)
        {
            if (finish.Equals(start))
            {
                return start;
            }

            Queue<Coordinate> toVisit = new Queue<Coordinate>();
            toVisit.Enqueue(start);
            HashSet<Coordinate> marked = new HashSet<Coordinate>();
            Dictionary<Coordinate, Coordinate> edgeTo = new Dictionary<Coordinate, Coordinate>();

            while (toVisit.Count > 0)
            {
                Coordinate v = toVisit.Dequeue();

                foreach (Coordinate n in neighborCoordinates(v, validSpaces))
                {
                    if (!marked.Contains(n))
                    {
                        marked.Add(n);
                        edgeTo.Add(n, v);
                        toVisit.Enqueue(n);

                        if (finish.Equals(n))
                        {
                            // Since we've found the result, backtrack through the edges
                            // until we find the node just before the start.
                            Coordinate cur = n;
                            while (!edgeTo[cur].Equals(start))
                            {
                                cur = edgeTo[cur];
                            }

                            return cur;
                        }
                    }
                }
            }

            // No path was found.
            return start;
        }
    }
}

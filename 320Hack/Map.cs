using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _320Hack
{
    class Map
    {
        private Player player;
        private Coordinate playerCoord;

        private Coordinate monster;

        private List<char> floorTiles;

        private List<Door> doors;
        private Room room;


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

        public Map(List<List<Tile>> map, Room room, List<Door> doors, Player player)
        {
            //monster = new Coordinate(PlayerStartRow + 2, PlayerStartCol + 4);
            //map[monster.row][monster.col] = new Tile('o');
            this.doors = doors;
            this.levelMap = map;
            this.player = player;
            this.room = room;

            foreach (Door door in doors) {
                levelMap[door.Row][door.Col] = new Tile(MainWindow.door);
            }
        }

        public String printMap()
        {
            String resultLevel = "";

            updateSeen();

            int maxRows = levelMap.Count;
            for (int row = 0; row < maxRows; row++ )
            {
                int maxCols = levelMap[row].Count;
                for (int col = 0; col < maxCols; col++)
                {
                    resultLevel += findChar(row, col);
                }
                resultLevel += '\n';
            }
            return resultLevel;
        }

        private char findChar(int row, int col)
        {
            if (row == player.Row && col == player.Col)
            {
                return MainWindow.player;
            }

            Tile current = levelMap[row][col];

            if (!current.Seen)
            {
                return ' ';
            }

            foreach (Door d in doors)
            {
                if (row == d.Row && col == d.Col)
                {
                    return MainWindow.door;
                }
            }

            return current.Symbol;
        }

        public void movePlayer(int dir)
        {
            if (dir == MainWindow.UP)
            {
                if (levelMap[player.Row - 1][player.Col].Symbol == MainWindow.floor)
                {
                    player.Row--;
                }
            }
            else if (dir == MainWindow.DOWN)
            {
                if (levelMap[player.Row + 1][player.Col].Symbol == MainWindow.floor)
                {
                    player.Row++;
                }
            }
            else if (dir == MainWindow.LEFT)
            {
                if (levelMap[player.Row][player.Col - 1].Symbol == MainWindow.floor)
                {
                    player.Col--;
                }
            }
            else if (dir == MainWindow.RIGHT)
            {
                if (levelMap[player.Row][player.Col + 1].Symbol == MainWindow.floor)
                {
                    player.Col++;
                }
            }
            else if (dir == MainWindow.UP_LEFT)
            {
                if (levelMap[player.Row - 1][player.Col - 1].Symbol == MainWindow.floor)
                {
                    player.Col--;
                    player.Row--;
                }
            }
            else if (dir == MainWindow.UP_RIGHT)
            {
                if (levelMap[player.Row - 1][player.Col + 1].Symbol == MainWindow.floor)
                {
                    player.Col++;
                    player.Row--;
                }
            }
            else if (dir == MainWindow.DOWN_LEFT)
            {
                if (levelMap[player.Row + 1][player.Col - 1].Symbol == MainWindow.floor)
                {
                    player.Col--;
                    player.Row++;
                }
            }
            else if (dir == MainWindow.DOWN_RIGHT)
            {
                if (levelMap[player.Row + 1][player.Col + 1].Symbol == MainWindow.floor)
                {
                    player.Col++;
                    player.Row++;
                }
            }

            using (var db = new DbModel())
            {
                db.Player.Attach(player);
                db.Entry(player).State = System.Data.Entity.EntityState.Modified;

                db.Rooms.Attach(room);
                db.Entry(room).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
            }
            // TODO did we hit the door? if so, load that map...

            /* TODO make monsters a collection etc.
            playerCoord = new Coordinate(player.Row, player.Col);

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
             */
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

            room.UpdateSeenValues(levelMap);
        }

        private bool canSeeTile(int r, int c)
        {
            if (player.Row == r && player.Col == c) return true;
            int rowSign = player.Row - r > 0 ? -1 : 1;
            int colSign = player.Col - c > 0 ? -1 : 1;

            double slope = Math.Abs((player.Row - r) * 1.0 / (player.Col - c));


            // If the tile in question is in the same column, iterate from playerRow to this row.
            if (player.Col == c || Math.Abs(slope) > 2)
            {
                for (int row = player.Row; row != r; row += rowSign)
                {
                    // Off the map--return false.
                    if (!isValidCoordinate(row, player.Col)) { return false; }
                    Tile t = levelMap[row][player.Col];

                    // If the symbol isn't floor, monster, or player, return false.
                    if (t.Symbol != MainWindow.floor &&
                        t.Symbol != 'o' &&
                        t.Symbol != MainWindow.player) { return false; }
                }

                // Don't bother with the slope, since it's infinity.
                return true;
            }

            double currentRow = Convert.ToDouble(player.Row);

            for (int col = player.Col; col != c; col += colSign)
            {
                int newRow = Convert.ToInt32(currentRow);

                // Off the map--return false.
                if (!isValidCoordinate(newRow, col)) { return false; }

                // If the symbol isn't floor, monster, or player, return false.
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

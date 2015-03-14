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

        private char[] walkTiles;

        private List<Door> doors;
        private List<MonsterInstance> monsters;
        private Room room;

        public Map(Player p)
        {
            player = p;
            setupState(p.CurrentRoom);
            this.walkTiles = new char[] { MainWindow.floor, MainWindow.door };
        }

        // Sets up the game state in the given room.
        public void setupState(int roomId)
        {
            using (var db = new DbModel())
            {
                player.CurrentRoom = roomId;

                room = (from r in db.Rooms
                        where r.Id == player.CurrentRoom
                        select r).Single();

                doors = (from d in db.Doors
                         where d.LivesIn == player.CurrentRoom
                         select d).ToList();

                monsters = (from m in db.MonsterInstances
                            where m.RoomId == player.CurrentRoom
                            select m).ToList();
            }

            room.setupMap();
        }

        public String printMap()
        {
            String resultLevel = "";

            updateSeen();

            int maxRows = room.LevelTiles.Count;
            for (int row = 0; row < maxRows; row++ )
            {
                int maxCols = room.LevelTiles[row].Count;
                for (int col = 0; col < maxCols; col++)
                {
                    resultLevel += findChar(row, col);
                }
                resultLevel += '\n';
            }
            return resultLevel;
        }

        // Returns the appropriate character to be printed at row, col.
        private char findChar(int row, int col)
        {
            if (row == player.Row && col == player.Col)
            {
                return MainWindow.player;
            }

            Tile current = room.LevelTiles[row][col];
            if (!current.Seen)
            {
                return ' ';
            }

            Door door = doors.Find(d => d.Row == row && d.Col == col);
            if (door != null)
            {
                return MainWindow.door;
            }

            MonsterInstance monster = monsters.Find(m => m.Row == row && m.Col == col);
            if (monster != null)
            {
                return Convert.ToChar(monster.Symbol);
            }

            return current.Symbol;
        }

        // Given a row delta and col delta, moves the player if the new tile is valid.
        public void movePlayer(int dRow, int dCol)
        {
            if (walkTiles.Contains(room.LevelTiles[player.Row + dRow][player.Col + dCol].Symbol))
            {
                player.Row += dRow;
                player.Col += dCol;
            }

            Door door = doors.Find(d => d.Row == player.Row && d.Col == player.Col);
            if (door != null) 
            {
                reloadMap(door);
            }

            save();
        }

        // Saves all game state.
        public void save()
        {
            using (var db = new DbModel())
            {
                db.Player.Attach(player);
                db.Entry(player).State = System.Data.Entity.EntityState.Modified;

                db.Rooms.Attach(room);
                db.Entry(room).State = System.Data.Entity.EntityState.Modified;

                foreach (MonsterInstance monster in monsters)
                {
                    db.MonsterInstances.Attach(monster);
                    db.Entry(monster).State = System.Data.Entity.EntityState.Modified;
                }

                db.SaveChanges();
            }
        }

        public void reset()
        {
            using (var db = new DbModel())
            {
                /*
                List<Room> allRooms = (from r in db.Rooms select r).ToList();
                foreach (Room r in allRooms)
                {
                    r.reset();
                    db.Rooms.Attach(r);
                    db.Entry(r).State = System.Data.Entity.EntityState.Modified;
                }
                */

                db.SaveChanges();
            }
        }

        // Reloads the map given the door the player stood on.
        public void reloadMap(Door door)
        {
            setupState(door.ConnectsTo);
            Door newDoor = doors.Find(d => d.ConnectsTo == door.LivesIn);
            if (newDoor == null) { throw new Exception("No corresponding door in the new room! newRoom=" + room.Id); }

            player.Row = newDoor.Row;
            player.Col = newDoor.Col;
        }

        // Updates which tiles the player has seen is this room.
        public void updateSeen()
        {
            // Update the tiles in levelMap to be seen if the player is near them
            int numRows = room.LevelTiles.Count;

            for (int i = 0; i < numRows; i++)
            {
                int numColsForThisRow = room.LevelTiles[i].Count;
                for (int j = 0; j < numColsForThisRow; j++)
                {
                    if (!room.LevelTiles[i][j].Seen) room.LevelTiles[i][j].Seen = canSeeTile(i, j);
                }
            }

            room.UpdateSeenValues(room.LevelTiles);
        }

        // Returns true if the player is able to see the title at r, c.
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
                    Tile t = room.LevelTiles[row][player.Col];

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
                Tile t = room.LevelTiles[newRow][col];
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
         * Returns all tiles neighboring start that are contained in validSpaces.
         */
        public List<Coordinate> neighborCoordinates(Coordinate start, List<char> validSpaces)
        {
            return neighborCoordinates(start.row, start.col, validSpaces);
        }

        /**
         * Returns all tiles neighboring row and col. Only counts 'floor' or the player as reachable.
         * Result if of form (int[] {row, col}).
         * TODO loop around the 9 square block so we don't have these ifs?
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
            return r > 0 && r < room.LevelTiles.Count && c > 0 && c < room.LevelTiles[r].Count;
        }

        /**
         * Returns true iff the given space is in the list of given types (checks validity first).
         */
        private Boolean isSpaceType(int r, int c, List<Char> validTypes)
        {
            return isValidCoordinate(r, c) && validTypes.Contains(room.LevelTiles[r][c].Symbol);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace _320Hack
{
    class Map
    {
        private Player player;

        private char[] walkTiles;
        private char[] seeTiles;

        private List<Door> doors;
        private List<MonsterInstance> monsters;
        private Room room;

        public Map(Player p)
        {
            player = p;
            setupState(p.CurrentRoom);
            this.walkTiles = new char[] { MainWindow.floor, MainWindow.door };
            this.seeTiles = new char[] { MainWindow.floor, MainWindow.door};
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

        public void printMap(TextBlock gameArea)
        {
            gameArea.Text = "";
            gameArea.Inlines.Clear();

            String inlineBuffer = "";
            updateSeen();

            int maxRows = room.LevelTiles.Count;
            for (int row = 0; row < maxRows; row++ )
            {
                int maxCols = room.LevelTiles[row].Count;
                for (int col = 0; col < maxCols; col++)
                {
                    inlineBuffer = processChar(row, col, inlineBuffer, gameArea);
                }
                inlineBuffer += '\n';
            }

            gameArea.Inlines.Add(new Run(inlineBuffer));
        }


        private String processChar(int row, int col, String buffer, TextBlock gameArea)
        {
            if (row == player.Row && col == player.Col)
            {
                gameArea.Inlines.Add(new Run(buffer));
                gameArea.Inlines.Add(new Run(Convert.ToString(MainWindow.playerChar)) { Foreground = Brushes.Gold });
                return "";
            }

            Tile current = room.LevelTiles[row][col];
            if (!current.Seen)
            {
                return buffer + " ";
            }

            MonsterInstance monster = monsters.Find(m => m.Row == row && m.Col == col);
            if (monster != null)
            {
                gameArea.Inlines.Add(new Run(buffer));
                gameArea.Inlines.Add(new Run(monster.Symbol) { 
                    Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(monster.Color))
                });
                return "";
            }

            Door door = doors.Find(d => d.Row == row && d.Col == col);
            if (door != null)
            {
                return buffer + door.getChar();
            }


            return buffer + Convert.ToString(current.Symbol);
        }

        // Given a row delta and col delta, moves the player if the new tile is valid.
        public void movePlayer(int dRow, int dCol, bool goInDoor = false, bool up = false)
        {
            int newRow = player.Row + dRow;
            int newCol = player.Col + dCol;

            MonsterInstance monsterToAttack = monsters.Find(m => m.Row == newRow && m.Col == newCol);
            if (monsterToAttack != null)
            {
                monsterToAttack.attack(player);
            }
            else if (walkTiles.Contains(room.LevelTiles[newRow][newCol].Symbol))
            {
                player.Row = newRow;
                player.Col = newCol;
            }

            foreach (MonsterInstance monster in monsters)
            {
                monster.move(room, player, walkTiles);
            }

            Door door = doors.Find(d => d.Row == player.Row && d.Col == player.Col);
            if (door != null && goInDoor)
            {
                if ((up && door.isUp()) || (!up && !door.isUp()))
                {
                    reloadMap(door);
                }
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
            int dr = r - player.Row;
            int dc = c - player.Col;

            double distance = (1.5 * Math.Sqrt(Math.Pow(dr, 2) + Math.Pow(dc, 2)));
            double straightness = Math.Abs(dr) * Math.Abs(dc);
            if (straightness != 0) {
                distance -= 6.0 / straightness;
            }
            else
            {
                distance -= 1;
            }
            if (distance > 7)
            {
                return false;
            }

            //double distance = (Math.Sqrt(
            //    Math.Pow((r - player.Row) * 0.9, 2) +
            //    Math.Pow((c - player.Col) * 0.70, 2)));
            //if (distance > 4)
            //{
            //    return false;
            //}

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
                    if (!seeTiles.Contains(t.Symbol)) { return false; }
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
                if (!seeTiles.Contains(t.Symbol))
                {
                    return false;
                }

                currentRow += rowSign * slope;
            }
            
            return true;
        }

        /**
         * Returns true iff the coordinate is valid on this map.
         */
        public Boolean isValidCoordinate(int r, int c)
        {
            return r > 0 && r < room.LevelTiles.Count && c > 0 && c < room.LevelTiles[r].Count;
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

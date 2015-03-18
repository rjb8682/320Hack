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

        public String Symbol { get; set; }

        public String Color { get; set; }

        public String Name { get; set; }

        public int HP { get; set; }
    }

    public class MonsterHistory
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public int MonsterId { get; set; }

        public int Count { get; set; }
    }

    public class MonsterInstance
    {
        public int Id { get; set; }

        public int MonsterId { get; set; }

        public int CurrentHP { get; set; }

        public String Color { get; set; }

        public String Name { get; set; }

        public int RoomId { get; set; }

        public int Row { get; set; }

        public int Col { get; set; }

        public int Power { get; set; }

        public String Symbol { get; set; }

        public int getAttackPower()
        {
            return Power;
        }

        public void place(DbModel db, Random random)
        {
            CurrentHP = (int)((from n in db.Monsters where n.Id == MonsterId select n.HP).Single() * 1.25 * RoomId);
            Room room = (from r in db.Rooms where RoomId == r.Id select r).Single();
            room.setupMap();

            List<MonsterInstance> otherMonsters = (from ms in db.MonsterInstances where RoomId == ms.RoomId select ms).ToList();

            int row = 0;
            int col = 0;
            Predicate<MonsterInstance> pred = m => m.Row == row && m.Col == col;

            do
            {
                row = random.Next(0, room.LevelChars.Count);
                col = random.Next(0, room.LevelChars[row].Count);
            }
            while (room.LevelChars[row][col] != MainWindow.floor && 
                otherMonsters.Find(pred) == null);

            Row = row;
            Col = col;

            db.MonsterInstances.Attach(this);
            db.Entry(this).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges();
        }

        public void attack(Player player)
        {
            int damage = player.getAttackPower();
            CurrentHP -= damage;
            Console.WriteLine("You dealt " + damage + " damage to the " + Name + "!");

            if (isDead())
            {
                Console.WriteLine("You killed the " + Name + "!");

                player.awardExperience(Power);

                Row = MainWindow.INVALID_POSITION;
                Col = MainWindow.INVALID_POSITION;

                // Add this monster to the monster history table.
                using (var db = new DbModel())
                {
                    MonsterHistory monsterHistory = (from m in db.MonsterHistory
                                                     where m.MonsterId == this.MonsterId && m.PlayerId == player.Id
                                                     select m).SingleOrDefault();

                    if (monsterHistory != null)
                    {
                        monsterHistory.Count++;
                        db.MonsterHistory.Attach(monsterHistory);
                        db.Entry(monsterHistory).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        monsterHistory = new MonsterHistory { PlayerId = player.Id, MonsterId = this.MonsterId, Count = 1 };
                        db.MonsterHistory.Attach(monsterHistory);
                        db.Entry(monsterHistory).State = System.Data.Entity.EntityState.Added;
                    }

                    // Save the state of this monster instance.
                    db.MonsterInstances.Attach(this);
                    db.Entry(this).State = System.Data.Entity.EntityState.Modified;

                    // Save the updated player experience.
                    db.Player.Attach(player);
                    db.Entry(player).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
            }
        }

        public bool isDead()
        {
            return CurrentHP <= 0;
        }

        public void move(Room room, Player player, char[] validSpaces, Predicate<Coordinate> monsterTest)
        {
            Coordinate start = new Coordinate(Row, Col);
            Coordinate target = new Coordinate(player.Row, player.Col);

            if (neighborCoordinates(room, start, validSpaces).Contains(target))
            {
                player.attack(this);
                return;
            }

            Coordinate result = shortestPathTo(room,
                start,
                target,
                validSpaces);

            // Don't move on top of another monster.
            if (!monsterTest(result))
            {
                Row = result.row;
                Col = result.col;
            }
            
        }

        /**
         * Returns all tiles neighboring start that are contained in validSpaces.
         */
        public List<Coordinate> neighborCoordinates(Room room, Coordinate start, char[] validSpaces)
        {
            return neighborCoordinates(room, start.row, start.col, validSpaces);
        }

        /**
         * Returns all tiles neighboring row and col. Only counts 'floor' or the player as reachable.
         * Result if of form (int[] {row, col}).
         * TODO loop around the 9 square block so we don't have these ifs?
         * TODO Make it so monsters can't walk on other monsters
         */
        public List<Coordinate> neighborCoordinates(Room room, int r, int c, char[] validTypes)
        {
            List<Coordinate> result = new List<Coordinate>();

            if (isSpaceType(room, r, c + 1, validTypes))
            {
                result.Add(new Coordinate(r, c + 1));
            }
            if (isSpaceType(room, r, c - 1, validTypes))
            {
                result.Add(new Coordinate(r, c - 1));
            }
            if (isSpaceType(room, r + 1, c, validTypes))
            {
                result.Add(new Coordinate(r + 1, c));
            }
            if (isSpaceType(room, r - 1, c, validTypes))
            {
                result.Add(new Coordinate(r - 1, c));
            }

            // Difficulty toggle for the future
            if (true)
            {
                if (isSpaceType(room, r + 1, c + 1, validTypes))
                {
                    result.Add(new Coordinate(r + 1, c + 1));
                }
                if (isSpaceType(room, r + 1, c - 1, validTypes))
                {
                    result.Add(new Coordinate(r + 1, c - 1));
                }
                if (isSpaceType(room, r - 1, c + 1, validTypes))
                {
                    result.Add(new Coordinate(r - 1, c + 1));
                }
                if (isSpaceType(room, r - 1, c - 1, validTypes))
                {
                    result.Add(new Coordinate(r - 1, c - 1));
                }
            }

            return result;
        }

        /**
         * Returns true iff the coordinate is valid on this map.
         */
        public Boolean isValidCoordinate(Room room, int r, int c)
        {
            return r > 0 && r < room.LevelTiles.Count && c > 0 && c < room.LevelTiles[r].Count;
        }

        /**
         * Returns true iff the given space is in the list of given types (checks validity first).
         */
        private Boolean isSpaceType(Room room, int r, int c, char[] validTypes)
        {
            return isValidCoordinate(room, r, c) && validTypes.Contains(room.LevelTiles[r][c].Symbol);
        }

        /**
         * Performs a BFS from the start to any coordinate in finish.
         * Returns the next tile to move to on the shortest path,
         * or start if finish is not reachable.
         * validSpaces is a list of chars that should be considered neighbors.
         * (e.g. for AI pathfinding, that should be ['.', '@'].
         */
        public Coordinate shortestPathTo(Room room, Coordinate start, Coordinate finish, char[] validSpaces)
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

                foreach (Coordinate n in neighborCoordinates(room, v, validSpaces))
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

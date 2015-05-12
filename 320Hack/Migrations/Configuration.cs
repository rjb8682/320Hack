namespace _320Hack.Migrations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    internal sealed class Configuration : DbMigrationsConfiguration<_320Hack.DbModel>
    {
        private const String COMMENT = "//";
        private char[] delimiters;

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            delimiters = new char[] { ' ' };
        }

        protected override void Seed(_320Hack.DbModel context)
        {
            AddMonsters(context);
            AddItems(context);
            AddRoomsAndDoors(context);
            AddTestPlayer(context);
            SetItemSpawns(context);
            base.Seed(context);
        }

        private void SetItemSpawns(_320Hack.DbModel context)
        {
            ItemSpawnPoint sp = new ItemSpawnPoint();

            // Long list of adding spawn points
            sp.Row = 2;
            sp.Col = 25;
            sp.RoomId = 1;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 8;
            sp.Col = 35;
            sp.RoomId = 1;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 12;
            sp.Col = 6;
            sp.RoomId = 1;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 16;
            sp.Col = 25;
            sp.RoomId = 1;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 3;
            sp.Col = 38;
            sp.RoomId = 2;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 1;
            sp.Col = 77;
            sp.RoomId = 2;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 7;
            sp.Col = 2;
            sp.RoomId = 2;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 6;
            sp.Col = 44;
            sp.RoomId = 2;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 14;
            sp.Col = 72;
            sp.RoomId = 2;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 17;
            sp.Col = 9;
            sp.RoomId = 2;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 1;
            sp.Col = 61;
            sp.RoomId = 3;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 6;
            sp.Col = 57;
            sp.RoomId = 3;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 7;
            sp.Col = 19;
            sp.RoomId = 3;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 13;
            sp.Col = 60;
            sp.RoomId = 3;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 19;
            sp.Col = 33;
            sp.RoomId = 3;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 19;
            sp.Col = 60;
            sp.RoomId = 3;

            context.ItemSpawns.AddOrUpdate(sp);

            sp = new ItemSpawnPoint();
            sp.Row = 21;
            sp.Col = 55;
            sp.RoomId = 3;

            context.ItemSpawns.AddOrUpdate(sp);

            addItems(context);
        }

        private void addItems(_320Hack.DbModel context)
        {
            StreamReader itemReader = new StreamReader(SourceCodePath("../../GameData/Items.txt"));
            int id = 1;
            String line = itemReader.ReadLine();
            while (line != null)
            {
                if (!line.StartsWith(COMMENT))
                {
                    String[] tempTokens = line.Split(',');
                    String name = tempTokens[0];
                    String[] tokens = tempTokens[1].Split(' ');
                    String type = tokens[0];

                    Item item = new Item
                    {
                        Id = id++,
                        Name = name
                    };

                    if (type == "Health")
                    {
                        item.AmountToHeal = Convert.ToInt32(tokens[1]);
                    }
                    else if (type == "Stat")
                    {
                        item.Stat = tokens[1];
                        item.Effect = Convert.ToInt32(tokens[2]);
                    }
                    context.Items.AddOrUpdate(item);
                }
                line = itemReader.ReadLine();
            }
        }

        private void AddMonsters(_320Hack.DbModel context)
        {
            StreamReader monsterReader = new StreamReader(SourceCodePath("../../GameData/Monsters.txt"));
            int id = 1;
            String line = monsterReader.ReadLine();
            while (line != null)
            {
                if (!line.StartsWith(COMMENT))
                {
                    String[] tokens = line.Split(delimiters);
                    Monster monster = new Monster
                    {
                        Id = id++,
                        Symbol = tokens[0],
                        HP = Convert.ToInt32(tokens[1]),
                        Color = tokens[2],
                        MinimumRoom = Convert.ToInt32(tokens[3]),
                        Name = tokens[4],
                        Speed = Convert.ToInt32(tokens[5]),
                        Attack = Convert.ToInt32(tokens[6])
                    };
                    context.Monsters.AddOrUpdate(monster);


                }
                line = monsterReader.ReadLine();
            }
        }

        private void AddItems(_320Hack.DbModel context)
        {
            StreamReader itemReader = new StreamReader(SourceCodePath("../../GameData/Items.txt"));
            int id = 1;
            String line = itemReader.ReadLine();
            while (line != null)
            {
                if (!line.StartsWith(COMMENT))
                {
                    String[] tokens = line.Split(delimiters);
                    /*
                    context.Monsters.AddOrUpdate(new Monster
                    {
                        Id = id++,
                        Symbol = tokens[0],
                        HP = Convert.ToInt32(tokens[1])
                    });
                     */
                }
                line = itemReader.ReadLine();
            }
        }

        private String fixCharacters(String map)
        {
            return map.Replace('.', MainWindow.floor).Replace('-', MainWindow.horizWall);
        }

        private void AddRoomsAndDoors(_320Hack.DbModel context)
        {
            String level1 = fixCharacters(new StreamReader(SourceCodePath("../../GameData/Levels/level1.map")).ReadToEnd());
            String level2 = fixCharacters(new StreamReader(SourceCodePath("../../GameData/Levels/level2.map")).ReadToEnd());
            String level3 = fixCharacters(new StreamReader(SourceCodePath("../../GameData/Levels/level3.map")).ReadToEnd());
            context.Rooms.AddOrUpdate(new Room { 
                Id = 1, 
                Map = level1,
                Seen = new byte[level1.Length] });
            context.Rooms.AddOrUpdate(new Room { 
                Id = 2, 
                Map = level2,
                Seen = new byte[level2.Length] });
            context.Rooms.AddOrUpdate(new Room
            {
                Id = 3,
                Map = level3,
                Seen = new byte[level3.Length]
            });

            context.Stairs.AddOrUpdate(new Stair { Id = 1, LivesIn = 1, ConnectsTo = 2, Row = 2, Col = 37 });
            context.Stairs.AddOrUpdate(new Stair { Id = 2, LivesIn = 2, ConnectsTo = 1, Row = 2, Col = 8 });
            context.Stairs.AddOrUpdate(new Stair { Id = 3, LivesIn = 2, ConnectsTo = 3, Row = 3, Col = 23 });
            context.Stairs.AddOrUpdate(new Stair { Id = 4, LivesIn = 3, ConnectsTo = 2, Row = 3, Col = 23 });
            context.Stairs.AddOrUpdate(new Stair { Id = 5, LivesIn = 2, ConnectsTo = 3, Row = 5, Col = 72 });
            context.Stairs.AddOrUpdate(new Stair { Id = 6, LivesIn = 3, ConnectsTo = 2, Row = 5, Col = 72 });
        }

        private void AddTestPlayer(_320Hack.DbModel context)
        {
            context.Player.AddOrUpdate(new Player { 
                Id = MainWindow.TEST_PLAYER_ID, 
                Name = "testPlayer", 
                CurrentRoom = Player.startRoomId, 
                Row = Player.startRow,
                Col = Player.startCol,
                Experience = 0, 
                Health = 100 });
        }

        // Gets us relative paths even though we're in a different directory.
        private string SourceCodePath(string seedFile)
        {
            var absolutePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            var directoryName = Path.GetDirectoryName(absolutePath);
            var path = Path.Combine(directoryName, ".." + seedFile.TrimStart('~').Replace('/', '\\'));

            return path;
        }
    }
}

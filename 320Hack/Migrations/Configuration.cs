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
            base.Seed(context);
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

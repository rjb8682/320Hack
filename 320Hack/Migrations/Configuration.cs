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

    internal sealed class Configuration : DbMigrationsConfiguration<_320Hack.Model1>
    {
        private const String COMMENT = "//";
        private char[] delimiters;

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            delimiters = new char[] { ' ' };
        }

        protected override void Seed(_320Hack.Model1 context)
        {
            AddMonsters(context);
            AddItems(context);
            AddRoomsAndDoors(context);
            AddTestPlayer(context);
            base.Seed(context);
        }

        private void AddMonsters(_320Hack.Model1 context)
        {
            StreamReader monsterReader = new StreamReader(SourceCodePath("../../GameData/Monsters.txt"));
            int id = 1;
            String line = monsterReader.ReadLine();
            while (line != null)
            {
                if (!line.StartsWith(COMMENT))
                {
                    String[] tokens = line.Split(delimiters);
                    context.Monsters.AddOrUpdate(new Monster
                    {
                        Id = id++,
                        Symbol = tokens[0],
                        HP = Convert.ToInt32(tokens[1])
                    });
                }
                line = monsterReader.ReadLine();
            }
        }

        private void AddItems(_320Hack.Model1 context)
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

        private void AddRoomsAndDoors(_320Hack.Model1 context)
        {
            context.Rooms.AddOrUpdate(new Room { 
                    Id = 1, 
                    Map = new StreamReader(SourceCodePath("../../GameData/Levels/level1.map")).ReadToEnd() });
            context.Rooms.AddOrUpdate(new Room { 
                Id = 2, 
                Map = new StreamReader(SourceCodePath("../../GameData/Levels/level2.map")).ReadToEnd() });
            context.Doors.AddOrUpdate(new Door { Id = 1, LivesIn = 1, ConnectsTo = 2, Row = 2, Col = 37 });
        }

        private void AddTestPlayer(_320Hack.Model1 context)
        {
            context.Player.AddOrUpdate(new Player { 
                Id = 1, 
                Name = "testPlayer", 
                CurrentRoom = 1, 
                LastRoom = 0,
                Row = 9,
                Col = 4,
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

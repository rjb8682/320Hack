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
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(_320Hack.Model1 context)
        {
            AddMonsters(context);
            AddRoomsAndDoors(context);
            AddTestPlayer(context);
            base.Seed(context);
        }

        private void AddMonsters(_320Hack.Model1 context)
        {
            context.Monsters.AddOrUpdate(new Monster { Id = 1, Symbol = "o", HP = 20 });
            context.Monsters.AddOrUpdate(new Monster { Id = 2, Symbol = "D", HP = 3 });
            context.Monsters.AddOrUpdate(new Monster { Id = 3, Symbol = "k", HP = 8 });
        }

        private void AddRoomsAndDoors(_320Hack.Model1 context)
        {
            StreamReader levelReader = new StreamReader(SourceCodePath("../../Levels/level1.map"));
            Room level1 = new Room { Id = 1, Map = levelReader.ReadToEnd()};
            Room level2 = new Room { Id = 2, Map = level1.Map };
            context.Rooms.AddOrUpdate(level1);
            context.Rooms.AddOrUpdate(level2);
            context.Doors.AddOrUpdate(new Door { Id = 1, LivesIn = 1, ConnectsTo = 2, Row = 2, Col = 37 });
        }

        private void AddTestPlayer(_320Hack.Model1 context)
        {
            context.Player.AddOrUpdate(new Player { Id = 1, Name = "testPlayer", CurrentRoom = 1, LastRoom = 0, Experience = 0, Health = 100 });
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

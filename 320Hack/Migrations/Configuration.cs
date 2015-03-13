namespace _320Hack.Migrations
{
    using System;
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
            context.MyEntities.AddOrUpdate(new MyEntity { Name = "Herp" });
            AddMonsters(context);
            AddRooms(context);
            AddTestPlayer(context);
            base.Seed(context);
        }

        private void AddMonsters(_320Hack.Model1 context)
        {
            context.Monsters.AddOrUpdate(new Monster { Id = 1, Symbol = "o", HP = 20 });
            context.Monsters.AddOrUpdate(new Monster { Id = 2, Symbol = "D", HP = 3 });
            context.Monsters.AddOrUpdate(new Monster { Id = 3, Symbol = "k", HP = 8 });
        }

        private void AddRooms(_320Hack.Model1 context)
        {
            StreamReader levelReader = new StreamReader(SourceCodePath("../../Levels/level1.map"));
            context.Rooms.AddOrUpdate(new Room { Id = 1, map = levelReader.ReadToEnd() });
        }

        private void AddTestPlayer(_320Hack.Model1 context)
        {
            context.Player.AddOrUpdate(new Player{ Id = 1, name = "testPlayer", currentRoom = 1, lastRoom = 0, experience = 0, health = 100 });
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

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
            base.Seed(context);
        }

        protected void AddMonsters(_320Hack.Model1 context)
        {
            context.Monsters.AddOrUpdate(new Monster { Id = 1, Symbol = "o", HP = 20 });
            context.Monsters.AddOrUpdate(new Monster { Id = 2, Symbol = "D", HP = 3 });
            context.Monsters.AddOrUpdate(new Monster { Id = 3, Symbol = "k", HP = 8 });
        }

        protected void AddRooms(_320Hack.Model1 context)
        {
            StreamReader levelReader = new StreamReader(MapPath("../../Levels/level1.map"));
            context.Rooms.AddOrUpdate(new Room { Id = 1, map = levelReader.ReadToEnd() });
        }

        // Gets us relative paths even though we're in a different directory.
        private string MapPath(string seedFile)
        {
            var absolutePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            var directoryName = Path.GetDirectoryName(absolutePath);
            var path = Path.Combine(directoryName, ".." + seedFile.TrimStart('~').Replace('/', '\\'));

            return path;
        }
    }
}

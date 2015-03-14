namespace _320Hack
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class DbModel : DbContext
    {
        // Your context has been configured to use a 'Model1' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // '_320Hack.Model1' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Model1' 
        // connection string in the application configuration file.
        public DbModel()
            : base("name=Model1")  { }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Monster> Monsters { get; set; }
        public virtual DbSet<MonsterInstance> MonsterInstances { get; set; }
        public virtual DbSet<MonsterHistory> MonsterHistory { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Player> Player { get; set; }
        public virtual DbSet<Door> Doors { get; set; }
    }
}
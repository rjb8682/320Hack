using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _320Hack
{
    public class Monster
    {
        public int Id { get; set; }

        [Required]
        public String Symbol { get; set; }

        public int HP { get; set; }

    }

    public class Room
    {
        public int Id { get; set; }

        public String Map { get; set; }
    }

    public class Player
    {
        public int Id { get; set; }

        public int CurrentRoom { get; set; }

        public int LastRoom { get; set; }

        public int Row { get; set; }

        public int Col { get; set; }

        public int Experience { get; set; }

        public int Health { get; set; }

        public String Name { get; set; }
    }

    public class Door
    {
        public int Id { get; set; }

        public int Row { get; set; }

        public int Col { get; set; }

        public int LivesIn { get; set; }

        public int ConnectsTo { get; set; }
    }
}

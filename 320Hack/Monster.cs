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

        public String Symbol { get; set; }

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

        public int RoomId { get; set; }

        public int Row { get; set; }

        public int Col { get; set; }

        public String Symbol { get; set; }

        public void move()
        {

        }
    }
}

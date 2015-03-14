using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _320Hack
{
    public class Player
    {
        public int Id { get; set; }

        public int CurrentRoom { get; set; }

        public int Row { get; set; }

        public int Col { get; set; }

        public int Experience { get; set; }

        public int Health { get; set; }

        public String Name { get; set; }

        public bool isDead()
        {
            return Health <= 0;
        }

        public void attack(MonsterInstance monster)
        {
            // TODO function of level + armor + chance
            // return some string "glancing blow" "beheaded you" etc
            int damage = monster.getAttackPower();
            Health -= damage;
            Console.WriteLine("The " + monster.Symbol + " dealt " + damage + "to you!");
        }

        public int getAttackPower()
        {
            return (int) (Experience * 1.25) + 5;
        }
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

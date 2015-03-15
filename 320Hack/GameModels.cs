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

        public String getInfo()
        {
            String toReturn = "";

            toReturn += "Level: " + getLevel();

            return toReturn;
        }

        public bool isDead()
        {
            return Health <= 0;
        }

        public int getLevel()
        {
            if (Experience > 0) { return (int)Math.Log(Experience, 2); }
            return 0;
        }

        private double expRequired(int level)
        {
            return Math.Pow(level, 2);
        }

        public double getFracToNextLevel()
        {
            int currLevel = getLevel();
            double expForNext = expRequired(currLevel + 1) - expRequired(currLevel);
            return (Experience - expRequired(currLevel)) / expForNext;
        }

        public void attack(MonsterInstance monster)
        {
            // TODO function of level + armor + chance
            // return some string "glancing blow" "beheaded you" etc
            int damage = monster.getAttackPower();
            Health -= damage;
            Console.WriteLine("The " + monster.Symbol + " dealt " + damage + " damage to you!");

            if (Health <= 0)
            {
                Console.WriteLine("The " + monster.Symbol + " killed you!");
            }
        }

        public int getAttackPower()
        {
            return (int) (getLevel() * 1.25) + 5;
        }

        public void awardExperience(int exp)
        {
            int level = getLevel();
            Experience += exp;
            if (getLevel() > level)
            {
                Console.WriteLine("Congratulations! You are now level " + getLevel() + ".");
            }
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

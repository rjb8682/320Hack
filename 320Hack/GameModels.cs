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
        public const int startCol = 48;
        public const int startRow = 4;
        public const int startRoomId = 1;

        public int maxHealth { get; set; }

        public int Id { get; set; }

        public int CurrentRoom { get; set; }

        public int Row { get; set; }

        public int Col { get; set; }

        public int Experience { get; set; }

        public int Health { get; set; }

        public int Speed { get; set; }

        public String Name { get; set; }

        public String getInfo()
        {
            String toReturn = "";
            toReturn += Name + " - ";
            toReturn += "Level: " + getLevel();

            return toReturn;
        }

        public bool isDead()
        {
            return Health <= 0;
        }

        public int getLevel()
        {
            if (Experience > 0) { return (int)Math.Sqrt(Experience); }
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
            Console.WriteLine("The " + monster.Name + " dealt " + damage + " damage to you!");

            if (Health <= 0)
            {
                Console.WriteLine("The " + monster.Name + " killed you!");
            }
        }

        public int getAttackPower()
        {
            return (int) Math.Pow(getLevel(), 1.5) + 5;
        }

        public void awardExperience(int exp)
        {
            int level = getLevel();
            Console.WriteLine("You got " + exp + " experience.");
            Experience += exp;
            if (getLevel() > level)
            {
                levelUp();
            }
        }

        public void levelUp()
        {
            Console.WriteLine("Congratulations! You are now level " + getLevel() + ".");
            maxHealth = (int)(maxHealth * 1.25);
            Health = maxHealth;
        }

        public void revive()
        {
            Row = startRow;
            Col = startCol;
            CurrentRoom = startRoomId;
            Health = maxHealth;
        }
    }

    public class Stair
    {
        public int Id { get; set; }

        public int Row { get; set; }

        public int Col { get; set; }

        public int LivesIn { get; set; }

        public int ConnectsTo { get; set; }

        public String getChar()
        {
            return isUp() ? "‹" : "›";
        }

        public bool isUp()
        {
            return LivesIn > ConnectsTo;
        }
    }
}

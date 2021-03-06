﻿using System;
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

        public double Strength { get; set; }

        public double Defense { get; set; }

        public int Dodge { get; set; }

        public String getInfo()
        {
            String toReturn = "";
            toReturn += Name + " - ";
            toReturn += "Level: " + getLevel();
            toReturn += " Strength: " + (int)(Strength + 0.5);
            toReturn += " Defense: " + (int)(Defense + 0.5);

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

        public int attack(MonsterInstance monster)
        {
            // TODO function of level + armor + chance
            // return some string "glancing blow" "beheaded you" etc
            int incomingDamage = monster.getAttackPower();
            int damage = (int)(incomingDamage * (1 - ((Defense * 1.125) / (incomingDamage * 2)))) + 1;
            if (damage < 1) damage = 1;
            Health -= damage;
            return damage;
        }

        public int getAttackPower()
        {
            return (int) Math.Pow((Strength / 10), 1.5) + 5;
        }

        public String awardExperience(int exp)
        {
            int level = getLevel();
            String consoleText = "";
            consoleText += "You got " + exp + " experience.";
            Experience += exp;
            if (getLevel() > level)
            {
                levelUp();
            }
            return consoleText;
        }

        public void levelUp()
        {
            Console.WriteLine("Congratulations! You are now level " + getLevel() + ".");
            maxHealth = (int)(maxHealth * 1.25);
            Strength = Strength * 1.125;
            if (getLevel() % 3 == 0) Defense = Defense * 1.5;
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

    public class Item
    {
        public int Id { get; set; }

        public String Name { get; set; }

        public String Type { get; set; }

        public int AmountToHeal { get; set; }

        public String Stat { get; set; }

        public int Effect { get; set; }
    }

    public class ItemSpawnPoint
    {
        public int Id { get; set; }

        public int Row { get; set; }

        public int Col { get; set; }

        public int RoomId { get; set; }

        public ItemInstance Item { get; set; }
    }

    public class ItemInstance
    {
        public int Id { get; set; }

        public String Name { get; set; }

        public String Type { get; set; }

        public int AmountToHeal { get; set; }

        public String Stat { get; set; }

        public int Effect { get; set; }

        public int RoomId { get; set; }

        public int Row { get; set; }

        public int Col { get; set; }

        public ItemInstance() { }

        public ItemInstance(Item itemTemp, int roomId)
        {
            Name = itemTemp.Name;
            Type = itemTemp.Type;
            RoomId = roomId;

            if (Type == "Health")
            {
                AmountToHeal = itemTemp.AmountToHeal;
            }
            else if (Type == "Stat")
            {
                Stat = itemTemp.Stat;
                Effect = itemTemp.Effect;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _320Hack
{
    public class Tile
    {
        // Private fields and their getters/setters
        private Char symbol;
        public Char Symbol {
            get
            {
                return this.symbol;
            }

            set
            {
                this.symbol = value;
            }
        }

        private Boolean canWalkOn;
        public Boolean CanWalkOn
        {
            get
            {
                return this.canWalkOn;
            }

            set
            {
                this.canWalkOn = value;
            }
        }

        // Constructor
        public Tile(Char sym, Boolean canWalk)
        {
            this.symbol = sym;
            this.canWalkOn = canWalk;
        }


    }
}

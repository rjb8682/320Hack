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
        private Boolean seen;
        public Boolean Seen
        {
            get
            {
                return this.seen;
            }

            set
            {
                this.seen = value;
            }
        }

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

        // Constructor
        public Tile(Char sym)
        {
            this.symbol = sym;
            this.seen = false;
        }


    }
}

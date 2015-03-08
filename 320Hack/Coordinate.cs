using System;

namespace _320Hack
{
    public struct Coordinate : IEquatable<Coordinate>
    {
        private readonly int _row;
        private readonly int _col;

        public Coordinate(int r, int c)
        {
            _row = r;
            _col = c;
        }

        public int row
        {
            get { return _row; }
        }

        public int col
        {
            get { return _col; }
        }

        public override int GetHashCode()
        {
            return _row ^ _col;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Coordinate))
                return false;

            return Equals((Coordinate)obj);
        }

        public bool Equals(Coordinate other)
        {
            if (_row != other._row)
                return false;

            return _col == other._col;
        }

        public static bool operator ==(Coordinate coordinate1, Coordinate coordinate2)
        {
            return coordinate1.Equals(coordinate2);
        }

        public static bool operator !=(Coordinate coordinate1, Coordinate coordinate2)
        {
            return !coordinate1.Equals(coordinate2);
        }
    }
}

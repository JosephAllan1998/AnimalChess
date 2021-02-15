using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AnimalChess.Models
{
    public class MyProcess
    {
        public Image image { get; set; }
        public Animal animal { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
    }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        Position()
        {
            X = 0;
            Y = 0;
        }
        Position(int _x, int _y)
        {
            X = _x;
            Y = _y;
        }
        Position(Position p)
        {
            X = p.X;
            Y = p.Y;
        }
    }

    public class History
    {
        public Team Round { get; set; }
        public Image Source { get; set; }
        public int ColumnSource { get; set; }
        public int RowSource { get; set; }
        public Image Destination { get; set; }
        public int ColumnDestination { get; set; }
        public int RowDestination { get; set; }
    }
}

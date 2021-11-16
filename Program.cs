using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoodcatCalculator
{
    enum types { top, down, right, left };
    class Program
    {
        static void Main(string[] args)
        {
            COORDINATES cOORDINATES = new COORDINATES();
            cOORDINATES.coordinates = new OneCoordinate[]{
                new OneCoordinate(0, 0),
                new OneCoordinate(5, 0),
                new OneCoordinate(5, 5),
                new OneCoordinate(0, 5)
            };
            cOORDINATES.update_type();
            Console.WriteLine(cOORDINATES);
        }
    }
    class OneCoordinate
    {
        public double x;
        public double y;
        public types type;
        public OneCoordinate(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public OneCoordinate(double x, double y, types type)
        {

        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")[" + type + "]";
        }
    }
    class COORDINATES
    {
        public OneCoordinate[] coordinates;
        public void update_type()
        {

            for (int i = 0; i < coordinates.Length; i++)
            {
                int next = i + 1;
                if (next == coordinates.Length)
                    next = 0;

                if (coordinates[next].y > coordinates[i].y)
                    coordinates[i].type = types.top;

                else if (coordinates[next].y < coordinates[i].y)
                    coordinates[i].type = types.down;

                else if (coordinates[next].x > coordinates[i].x)
                    coordinates[i].type = types.right;

                else
                    coordinates[i].type = types.left;
            }

        }

        public override string ToString()
        {
            string str="";
            foreach (OneCoordinate oc in coordinates)
            {
                str += oc+" ~~ ";
            }
            return str;
        }

    }


    class piese : IComparable<piese>
    {
        public double Length;
        public double Width;
        public piese(double length, double width)
        {
            if (width > length)
            {
                Length = width;
                Width = length;
            }
            else
            {
                Length = length;
                Width = width;
            }
        }
        public override string ToString()
        {
            return "(" + Length + " x " + Width + ")";
        }
        public int CompareTo(piese other)
        {
            if (this.Length > other.Length)
                return -1;
            if (this.Length < other.Length)
                return 1;
            if (this.Width > other.Width)
                return -1;
            if (this.Width < other.Width)
                return 1;
            return 0;

        }
    }
}

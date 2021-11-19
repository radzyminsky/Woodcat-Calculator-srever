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

            COORDINATES C = new COORDINATES();
            C.insertRange(0, new Coordinate[]{
                new Coordinate(0, 0),
                new Coordinate(5, 0),
                new Coordinate(5, 5),
                new Coordinate(0, 5)
            });
            Console.WriteLine(C);
            Console.WriteLine("add");
            C.insertRange(3, new Coordinate[]{
                new Coordinate(6, 5),
                new Coordinate(6, 7),
                new Coordinate(0, 7)
            });
            Console.WriteLine(C);
            C.removeUnnecessaryCoordinate();
            Console.WriteLine(C);
            Console.WriteLine("remove");
             C.removeRange(new Coordinate(6, 5), 3);
            Console.WriteLine(C);
            Console.WriteLine(C.count());
            
           
        }
    }
    class Coordinate
    {
        public double x;
        public double y;
        public types type;
        public Coordinate(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public Coordinate(double x, double y, types type) : this(x, y)
        {
            this.type = type;
        }
        public static bool operator !=(Coordinate c1, Coordinate c2)
        {
            return !(c1 == c2);
        }


        public static bool operator ==(Coordinate c1, Coordinate c2)
        {
            if (c1.x == c2.x && c1.y == c2.y)
                return true;
            return false;
        }
        public override string ToString()
        {
            return "(" + x + "," + y + ")[" + type + "]";
        }
    }
    class COORDINATES
    {
        private List<Coordinate> coordinates;
        public COORDINATES()
        {
            coordinates = new List<Coordinate>();
        }

        public void insertRange(int index,Coordinate[]arr)
        {
            coordinates.InsertRange(index, arr);
            update_type();
            removeUnnecessaryCoordinate();
            if (checkIsInvalid())
                throw new Exception("the coordinates is invalid");

        }

        public void removeRange(Coordinate c1,int count)
        {
            int index = coordinates.FindIndex((c2) => { return c2 == c1; });
            coordinates.RemoveRange(index, count);
            update_type();
            removeUnnecessaryCoordinate();
            if (checkIsInvalid())
                throw new Exception("the coordinates is invalid");

        }

        public int count()
        {
            return coordinates.Count;
        }

        //check if the coordinates is invalid, for exemple: if the coordinates represent a triangle...
        public bool checkIsInvalid()
        {
            int next;
            for (int i = 0; i < coordinates.Count; i++)
            {
                next = i + 1;
                if (next == coordinates.Count)
                    next = 0;
                if (coordinates[i].x != coordinates[next].x && coordinates[i].y != coordinates[next].y)
                    return true;                
            }
            return false;
        }

        //remove all coordinate that unnecessary for exemple: (0,0)~~(5,0)~~(6,0)~~.., the coordinate (5,0) is unnecessary.
        public void removeUnnecessaryCoordinate()
        {
            int next;
            for (int i = 0; i < coordinates.Count; i++)
            {
                next = i + 1;
                if (next == coordinates.Count)
                    next = 0;
                if (coordinates[i].type == coordinates[next].type)
                {
                    coordinates.Remove(coordinates[next]);
                    i--;
                }
            }
        }

        //insert value to enum field "type".
        private void update_type()
        {

            for (int i = 0; i < coordinates.Count; i++)
            {
                int next = i + 1;
                if (next == coordinates.Count)
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

        //The function receives an index and checks whether the point is external.
        //exemple for internal point: (0,0)~~(5,0)~~(5,1)~~(6,1)~~(6,4)~~(0,4)
        //the point (5,1) is internal, while others are external
        public bool check_if_is_external(int index)
        {
            int previous = index - 1;
            if (index == 0)
                previous = coordinates.Count - 1;
            if (coordinates[index].type == types.left && coordinates[previous].type == types.down)
                return false;
            if (coordinates[index].type == types.right && coordinates[previous].type == types.top)
                return false;
            if (coordinates[index].type == types.top && coordinates[previous].type == types.left)
                return false;
            if (coordinates[index].type == types.down && coordinates[previous].type == types.right)
                return false;
            return true;
        }

        public override string ToString()
        {
            string str = "";
            foreach (Coordinate c in coordinates)
            {
                if (check_if_is_external(coordinates.IndexOf(c)))
                    str += "(externul)";
                else
                    str += "(internal)";
                str += c + " ~~ ";
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
            //the "length" allwais needs biger from width
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoodcatCalculator
{
    enum types { top=1, down=2, right=4, left=8 };
    class Program
    {
        static void Main(string[] args)
        {

            COORDINATES C = new COORDINATES();
            C.insertRange(0, new Coordinate[]{
                new Coordinate(0, 0),
                new Coordinate(3, 0),

                new Coordinate(3,4),
                new Coordinate(0,4)
            });
            Console.WriteLine(C);
            Console.WriteLine("add");
            C.insertRange(2, new Coordinate[]{
               new Coordinate(3, 2),
                new Coordinate(1, 2),
                new Coordinate(1, 3),
                new Coordinate(3,3),
            });
            Console.WriteLine(C);
            Console.WriteLine(C.count());

            Console.WriteLine(C.fun4(new piece(2, 0.5), 0));

            Console.WriteLine();
            Console.WriteLine(C.fun4(new piece(2.5, 1.5), 0));

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

        //insert new coordinate or coordinates, update "type" field, and removing unnecessary cordinate from 
        //the coordinates array, finaly, checking if is valid or not.
        public void insertRange(int index, Coordinate[] arr)
        {
            coordinates.InsertRange(index, arr);
            update_type();
            removeUnnecessaryCoordinate();
            if (checkIsInvalid())
                throw new Exception("the coordinates is invalid");

        }

        //remove coordinate or coordinates from the coordinates array,
        //update "type" field, and removing unnecessary cordinate , finaly, checking if is valid or not.
        public void removeRange(Coordinate c1, int count)
        {
            int index = this.findIndex(c1);
            coordinates.RemoveRange(index, count);
            update_type();
            removeUnnecessaryCoordinate();
            if (checkIsInvalid())
                throw new Exception("the coordinates is invalid");

        }

        //this function return an index of given coordinate
        //the function uses at "FindIndex" function of "List" that gets a delegate
        public int findIndex(Coordinate c)
        {
            return coordinates.FindIndex((c2) => { return c2 == c; });
        }
        public int count()
        {
            return coordinates.Count;
        }

        //check if the coordinates is invalid, for exemple: if the coordinates represent a triangle...
        //(all two adjacent coordinates should be equal in "y" or "x").
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

        //"fun4" get a given piece and index, then it checks if can cat this piece from the array at the index given,
        //"fun4" uses in tow functions "help_fun4_Increase_current" and "help_fun4_Increase_current",
        //the first of which initializes values, the second, increas the index every iteration
        public bool fun4(piece p, int index_of_coord)
        {
            double x_right, x_left, y_top, y_down;

            help_fun4_init_values(out x_right, out x_left, out y_top, out y_down, index_of_coord, p);

            Console.WriteLine("check if can insert piece: (" + x_left + "," + x_right + "," + y_top + "," + y_down + ")\nat coordinate: " +coordinates[ index_of_coord]);

            int current = index_of_coord + 1;
            int next;

            while (current != index_of_coord )
            {           
                if (coordinates[current].type == types.left || coordinates[current].type == types.right)
                {
                    if (coordinates[current].y < y_top && coordinates[current].y > y_down)
                    {
                        next = current + 1;
                        if ((coordinates[current].x >= x_right && coordinates[next].x < x_right) || (coordinates[current].x <= x_left && coordinates[next].x > x_left))
                            return false;
                        help_fun4_Increase_current(ref current);
                    }
                }
                else
                {
                    if (coordinates[current].x < x_right && coordinates[current].x > x_left)
                    {
                        next = current + 1;
                        if ((coordinates[current].y >= y_top && coordinates[next].y < y_top) || (coordinates[current].y <= y_down && coordinates[next].y > y_down))
                            return false;
                        help_fun4_Increase_current(ref current);
                    }
                }
                help_fun4_Increase_current(ref current);
            }

            return true;
        }

        void help_fun4_Increase_current(ref int curr)
        {
            curr++;
            if (curr >= coordinates.Count)
                curr = 0;
        }
        void help_fun4_init_values(out double x_right, out double x_left, out double y_top, out double y_down, int index_of_coord, piece p)
        {
            Coordinate c = coordinates[index_of_coord];
            if (c.type == types.top)
            {
                x_right = c.x;
                x_left = c.x - p.Width;
                y_top = c.y + p.Length;
                y_down = c.y;
            }
            else if (c.type == types.down)
            {
                x_right = c.x + p.Width;
                x_left = c.x;
                y_top = c.y;
                y_down = c.y - p.Length;
            }
            else if (c.type == types.right)
            {
                x_right = c.x + p.Width;
                x_left = c.x;
                y_top = c.y + p.Length;
                y_down = c.y;
            }
            else
            {
                x_right = c.x;
                x_left = c.x - p.Width;
                y_top = c.y;
                y_down = c.y - p.Length;
            }
        }

        public COORDINATES[] fun6()
        {
            COORDINATES[] coordinatesArray = new COORDINATES[] { };
            return coordinatesArray;
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


    class piece : IComparable<piece>
    {
        public double Length;
        public double Width;
        public piece(double length, double width)
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
        public int CompareTo(piece other)
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

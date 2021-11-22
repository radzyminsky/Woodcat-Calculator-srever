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
            cOORDINATES.add(0, new Coordinate[]{
                new Coordinate(0, 0),
                new Coordinate(5, 0),
                new Coordinate(5, 5),
                new Coordinate(0, 5)
            });
            Console.WriteLine(cOORDINATES);
            Console.WriteLine("add");
            cOORDINATES.add(3, new Coordinate[]{
                new Coordinate(6, 5),
                new Coordinate(6, 7),
                new Coordinate(0, 7)
            });
<<<<<<< HEAD
            Console.WriteLine(cOORDINATES);
            Console.WriteLine("remove");
            cOORDINATES.remove(cOORDINATES.find(new Coordinate(6, 5)), cOORDINATES.find(new Coordinate(0, 7)));
            Console.WriteLine(cOORDINATES);
            List<Coordinate> list = new List<Coordinate>();
            
           
=======
            Console.WriteLine(C);
            Console.WriteLine(C.count());

            Console.WriteLine(C.fun4(new piece(2, 0.5), 0));

            Console.WriteLine();
            Console.WriteLine(C.fun4(new piece(2.5, 1.5), 0));

>>>>>>> 96b400b (i try commit)
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
        public Coordinate(double x, double y, types type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")[" + type + "]";
        }
    }
    class COORDINATES
    {
        private Coordinate[] coordinates;

        public COORDINATES()
        {

            coordinates = new Coordinate[0];
        }

        // add "sub array" (c) to the array (coordinates) from the index paramater.
        public void add(int from_index, Coordinate[] c)
        {
            Coordinate[] new_coordinates = new Coordinate[coordinates.Length + c.Length];
            for (int i = 0; i < from_index; i++)
            {
                new_coordinates[i] = coordinates[i];
            }
            for (int i = from_index; i < c.Length + from_index; i++)
            {
                new_coordinates[i] = c[i - from_index];
            }
            for (int i = c.Length + from_index; i < new_coordinates.Length; i++)
            {
                new_coordinates[i] = coordinates[i - c.Length];
            }
            coordinates = new_coordinates;
            update_type();


        }

        //renove from the array from index "from_index" until "till_index" inclloded "till_index".
        public void remove(int from_index, int till_index)
        {
            int difference = till_index - from_index;
            int size = coordinates.Length - (difference + 1);

            Coordinate[] new_coordinates = new Coordinate[size];

            for (int i = 0; i < from_index; i++)
            {
                new_coordinates[i] = coordinates[i];
            }

            for (int i = till_index + 1; i < coordinates.Length; i++)
            {
                new_coordinates[i - (difference + 1)] = coordinates[i];
            }

            coordinates = new_coordinates;
            update_type();

        }

        //return index of given parameter or -1 if isn't found.
        public int find(Coordinate oc)
        {
            for (int i = 0; i < coordinates.Length; i++)
            {
                if (coordinates[i].x == oc.x && coordinates[i].y == oc.y)
                    return i;
            }
            return -1;
        }

        //insert value to "type" enum field.
        private void update_type()
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

        //The function receives an index and checks whether the point is external.
        public bool check_if_is_external(int index)
        {
            int previous = index - 1;
            if (index == 0)
                previous = coordinates.Length - 1;
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
            foreach (Coordinate oc in coordinates)
            {
                if (check_if_is_external(find(oc)))
                    str += "(externul)";
                else
                    str += "(internal)";
                str += oc + " ~~ ";
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

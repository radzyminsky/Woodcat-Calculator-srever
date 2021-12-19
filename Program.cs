using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoodcatCalculator
{
    [Flags]
    enum types
    {
        top = 1, down = 2, right = 4, left = 8
    };
    class Program
    {
        static void Main(string[] args)
        {

            Coordinates C = new Coordinates(
                new OneCoordinate[]
                {
                new OneCoordinate(0, 3),
               
                new OneCoordinate(1, 3),
                new OneCoordinate(1, 0),
                new OneCoordinate(4, 0),
                new OneCoordinate(4, 4),
                new OneCoordinate(2, 4),
                new OneCoordinate(2,5),
                new OneCoordinate(0,5),
                new OneCoordinate(0,5)
               
                //new OneCoordinate(0,1),
                //new OneCoordinate(2,1),
                //new OneCoordinate(2,0),
                //new OneCoordinate(6,0),
                //new OneCoordinate(6,6),
                //new OneCoordinate(4,6),
                //new OneCoordinate(4,7),
                //new OneCoordinate(2,7),
                //new OneCoordinate(2,6),
                //new OneCoordinate(1,6),
                //new OneCoordinate(1,5),
                //new OneCoordinate(0,5),
                //new OneCoordinate(0,3),
                //new OneCoordinate(1,3),
                //new OneCoordinate(1,2),
                //new OneCoordinate(0,2),
                });
            Console.WriteLine(C);



            Console.WriteLine("count: " + C.count());
            AllCoordinatesList AllCoordinates = new AllCoordinatesList();

            AllCoordinates.All.AddRange(C.fun6(new piece(3, 1), 6));
           // AllCoordinates.All.AddRange(C.fun6(new piece(2, 5), 9));

            foreach (var item in AllCoordinates.All)
            {
                Console.WriteLine("----------------------new--------------------");
                Console.WriteLine(item);
            }


        }
    }
    class AllCoordinatesList
    {
        public List<Coordinates> All = new List<Coordinates>();
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < All.Count; i++)
            {
                str += i + ": " + All[i] + "\n";
            }
            return str;
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
        public OneCoordinate(double x, double y, types type) : this(x, y)
        {
            this.type = type;
        }
        public OneCoordinate(OneCoordinate c) : this(c.x, c.y, c.type) { }

        public static bool operator !=(OneCoordinate c1, OneCoordinate c2)
        {
            return !(c1 == c2);
        }
        public static bool operator ==(OneCoordinate c1, OneCoordinate c2)
        {
            if (c1.x == c2.x && c1.y == c2.y)
                return true;
            return false;
        }
        public static bool operator >(OneCoordinate c1, OneCoordinate c2)
        {

            if (c1.x == c2.x || c1.y == c2.y)
            {
                if (c1.y > c2.y || c1.x > c2.x)
                    return true;
                return false;
            }
            throw (new Exception("a and b aren't on the same axis "));
        }
        public static bool operator <(OneCoordinate c1, OneCoordinate c2)
        {

            if (c1.x == c2.x || c1.y == c2.y)
            {
                if (c1.y < c2.y || c1.x < c2.x)
                    return true;
                return false;
            }
            throw (new Exception("a and b aren't on the same axis "));
        }
        public static bool operator <=(OneCoordinate c1, OneCoordinate c2)
        {
            return (c1 < c2 || c1 == c2);
        }
        public static bool operator >=(OneCoordinate c1, OneCoordinate c2)
        {
            return (c1 > c2 || c1 == c2);
        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")[" + type + "]";
        }
    }

    class Coordinates
    {
        public List<OneCoordinate> coordinates;
        public Coordinates()
        {
            coordinates = new List<OneCoordinate>();
        }
        public Coordinates(IEnumerable<OneCoordinate> c) : this()
        {
            OneCoordinate[] c_;

            if (c.GetType() == typeof(OneCoordinate[]))
                c_ = (OneCoordinate[])c;
            else if (c.GetType() == typeof(List<OneCoordinate>))
                c_ = ((List<OneCoordinate>)c).ToArray();
            else
                throw new Exception("the parameter type is dont match to convert");

            for (int i = 0; i < c_.Length; i++)
            {
                coordinates.Add(new OneCoordinate(c_[i]));
            }

            ////--------------------
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine(this);
            ////--------------------

            UpdateTypeAndRemoveUnnecessaryCoordinate();

            ////---------------------
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine(this);
            //Console.ForegroundColor = ConsoleColor.White;
            ////-----------------------

            if (checkIsInvalid())
                throw new Exception("the coordinates is invalid");

        }

        public Coordinates(Coordinates C) : this(C.coordinates) { }

        //insert new coordinate or coordinates from the specified undex, update "type" field, and removing unnecessary cordinate from 
        //the coordinates array, finaly, checking if is valid or not.
        public void insertRange(int index, OneCoordinate[] arr)
        {
            coordinates.InsertRange(index, arr);

            //------------
            Console.WriteLine("before update and remove:");
            Console.WriteLine(this);
            //----------------

            UpdateTypeAndRemoveUnnecessaryCoordinate();

            //------------
            Console.WriteLine("after update and remove:");
            Console.WriteLine(this);
            //----------------

            if (checkIsInvalid())
                throw new Exception("the coordinates is invalid");

        }


        //remove coordinate or coordinates from the coordinates array,
        //update "type" field, and removing unnecessary cordinate , finaly, checking if is valid or not.
        public void removeRange(OneCoordinate c1, int count)
        {
            int index = this.findIndex(c1);
            coordinates.RemoveRange(index, count);
            UpdateTypeAndRemoveUnnecessaryCoordinate();
            if (checkIsInvalid())
                throw new Exception("the coordinates is invalid");

        }
        public void removeRange(int from_index, int until_index)
        {
            if (from_index <= until_index)
            {
                coordinates.RemoveRange(from_index, until_index - from_index + 1);
                UpdateTypeAndRemoveUnnecessaryCoordinate();
                if (checkIsInvalid())
                    throw new Exception("the coordinates is invalid");
            }
            else
            {
                coordinates.RemoveRange(from_index, coordinates.Count - from_index);
                coordinates.RemoveRange(0, until_index + 1);
                UpdateTypeAndRemoveUnnecessaryCoordinate();
                if (checkIsInvalid())
                    throw new Exception("the coordinates is invalid");
            }
        }

        public List<OneCoordinate> getRange(int from_index, int until_index)
        {
            if (from_index <= until_index)
            {
                return coordinates.GetRange(from_index, until_index - from_index + 1);
            }
            List<OneCoordinate> temp = new List<OneCoordinate>(coordinates.GetRange(from_index, coordinates.Count - from_index));
            temp.AddRange(coordinates.GetRange(0, until_index + 1));
            return temp;

        }
        
        //this function return an index of given coordinate
        //the function uses at "FindIndex" function of "List" that gets a delegate
        public int findIndex(OneCoordinate c)
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

        //remove all coordinate that unnecessary for exemple: (0,0)~~(5,0)~~(6,0)~~.., the coordinate (5,0) is unnecessary
        //or: (0,0)~~(6,0)~~(5,0)~~.., the coordinate (6,0) is not valid and unnecessary.
        public void UpdateTypeAndRemoveUnnecessaryCoordinate()
        {
            types t1 = types.down | types.top;
            types t2 = types.right | types.left;
            types t3;

            int next;
            bool flag = true;
            while (flag)
            {
                update_type();
                flag = false;

                for (int i = 0; i < coordinates.Count; i++)
                {
                    next = i + 1;
                    if (next == coordinates.Count)
                        next = 0;

                    t3 = coordinates[i].type | coordinates[next].type;
                    if ((byte)(t3 & t1) == 0 || (byte)(t3 & t2) == 0)
                    {
                        coordinates.Remove(coordinates[next]);
                        i--;

                        //becaus now, maybe the "type" field of each coordinate changes
                        flag = true;
                    }
                }
            }
        }

        //insert value to enum field "type" and removes two adjacent coordinates equal .
        private void update_type()
        {
            for (int current = 0; current < coordinates.Count; current++)
            {
                int next = current + 1;
                if (next == coordinates.Count)
                    next = 0;
                if (coordinates[next] == coordinates[current])
                {
                    coordinates.RemoveAt(current);
                    current--;
                }
                else if (coordinates[next].y > coordinates[current].y)
                    coordinates[current].type = types.top;

                else if (coordinates[next].y < coordinates[current].y)
                    coordinates[current].type = types.down;

                else if (coordinates[next].x > coordinates[current].x)
                    coordinates[current].type = types.right;

                else
                    coordinates[current].type = types.left;
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

            // Console.WriteLine("check if can insert piece: (" + x_left + "," + x_right + "," + y_top + "," + y_down + ")\nat coordinate: " +coordinates[ index_of_coord]);

            Console.WriteLine("check if can insert piece: {0}{1}{2}{3}\nat coordinate: {4}"
                , new OneCoordinate(x_left, y_down), new OneCoordinate(x_right, y_down), new OneCoordinate(x_right, y_top),
                new OneCoordinate(x_left, y_top), coordinates[index_of_coord]);

            int current = index_of_coord + 1;
            int next;

            while (current != index_of_coord)
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
            OneCoordinate c = coordinates[index_of_coord];
            switch (c.type)
            {
                case types.top:
                    x_right = c.x;
                    x_left = c.x - p.Width;
                    y_top = c.y + p.Length;
                    y_down = c.y;
                    break;
                case types.down:
                    x_right = c.x + p.Width;
                    x_left = c.x;
                    y_top = c.y;
                    y_down = c.y - p.Length;
                    break;
                case types.right:
                    x_right = c.x + p.Width;
                    x_left = c.x;
                    y_top = c.y + p.Length;
                    y_down = c.y;
                    break;
                case types.left:
                    x_right = c.x;
                    x_left = c.x - p.Width;
                    y_top = c.y;
                    y_down = c.y - p.Length;
                    break;
                default:
                    throw new Exception();

            }

        }

        public List<Coordinates> fun6(piece p, int here_insert)
        {
            List<Coordinates> AllList = new List<Coordinates>();

         //   AllCoordinatesList AllList = new AllCoordinatesList();

            Coordinates c_temp, c_temp2;
            OneCoordinate[] c = help_fun_6_create_sub_array_with_the_new_coordinates(here_insert, p);
            //-----------
            Console.Write("insert sub arrai: ");
            foreach (var item in c)
            {
                Console.Write(item + ", ");
            }
            Console.WriteLine("; instead of coordinate: " + coordinates[here_insert]);
            //------------
            c_temp = new Coordinates(coordinates);

            c_temp.coordinates.RemoveAt(here_insert);

            //i use in "insertRang" function, not "InsertRange", because i need it to play some functions...
            c_temp.insertRange(here_insert, c);



            AllList.Add(c_temp);

            for (int i = 0; i < AllList.Count; i++)
            {


                MyIndex next, after_next, index_checked, before_index_checked;


                for (int j = 0; j < AllList[i].coordinates.Count; j++)
                {

                    next = new MyIndex(j + 1, AllList[i].coordinates.Count - 1);
                    after_next = new MyIndex(j + 2, AllList[i].coordinates.Count - 1);
                    index_checked = new MyIndex(j + 2, AllList[i].coordinates.Count - 1);
                    //----------------------
                    Console.WriteLine(" AllCoordinates[" + i + "]: " + AllList[i]);
                    Console.WriteLine(" AllCoordinates[" + i + "].count: " + AllList[i].count());
                    //---------------------
                    while (index_checked != j)
                    {
                        //-----------------------
                        Console.WriteLine("j= ({0}), index_checked= ({1})", j, index_checked);
                        Console.WriteLine("check if: {2}, beetwin coordinats: {0}~~{1}",
                             AllList[i].coordinates[j], AllList[i].coordinates[next.get()], AllList[i].coordinates[index_checked.get()]);
                        //---------------------------
                        if (AllList[i].help_fun6_check_if_is_beetwin(AllList[i].coordinates[j], AllList[i].coordinates[next.get()], index_checked.get()))
                        {
                            //-----------------------
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("--yes--");
                            Console.ForegroundColor = ConsoleColor.White;
                            //-----------------------
                            before_index_checked = new MyIndex(index_checked);
                            before_index_checked--;

                            c_temp2 = new Coordinates(AllList[i].getRange(next.get(), index_checked.get()));

                            try   // if the coordinate remainder in c_temp is invalid, so delete c_temp
                            {
                                AllList[i].removeRange(after_next.get(), before_index_checked.get());
                            }
                            catch (Exception e)
                            {     //delete
                                AllList[i].coordinates.RemoveRange(0, AllList[i].coordinates.Count);
                                Console.WriteLine(e.StackTrace);
                            }

                            AllList.Add(c_temp2);

                            //---------------------
                            Console.WriteLine(AllList);
                            //----------------------

                            if (!(j < AllList[i].coordinates.Count))
                                break;
                            next = new MyIndex(j + 1, AllList[i].coordinates.Count - 1);
                            after_next = new MyIndex(j + 2, AllList[i].coordinates.Count - 1);
                            index_checked = new MyIndex(j + 2, AllList[i].coordinates.Count - 1);
                        }
                        else
                        {
                            Console.WriteLine("--no--");

                            index_checked = new MyIndex(index_checked.get(), AllList[i].count() - 1);
                            index_checked++;
                        }
                    }
                }
                if (AllList[i].coordinates.Count == 0)
                {
                    AllList.RemoveAt(i);
                    i--;
                }
            }
            return AllList;
        }

        OneCoordinate[] help_fun_6_create_sub_array_with_the_new_coordinates(int here_insert, piece p)
        {
            OneCoordinate[] c;
            double x_right, x_left, y_top, y_down;
            help_fun4_init_values(out x_right, out x_left, out y_top, out y_down, here_insert, p);
            //------
            Console.WriteLine("x_right: {0}, x_left: {1}, y_top: {2}, y_down: {3}", x_right, x_left, y_top, y_down);

            switch (coordinates[here_insert].type)
            {
                case types.top:
                    c = new OneCoordinate[]{
                        new OneCoordinate(x_left, y_down),
                        new OneCoordinate(x_left,y_top),
                        new OneCoordinate(x_right,y_top)
                    };
                    break;
                case types.down:
                    c = new OneCoordinate[]{
                        new OneCoordinate(x_right, y_top),
                        new OneCoordinate(x_right,y_down),
                        new OneCoordinate(x_left,y_down)
                    };
                    break;
                case types.right:
                    c = new OneCoordinate[]{
                        new OneCoordinate(x_left, y_top),
                        new OneCoordinate(x_right,y_top),
                        new OneCoordinate(x_right,y_down)
                    };
                    break;
                case types.left:
                    c = new OneCoordinate[]{
                        new OneCoordinate(x_right, y_down),
                        new OneCoordinate(x_left,y_down),
                        new OneCoordinate(x_left,y_top)
                    };
                    break;
                default:
                    throw new Exception("yoe arrived to \"default switch\"");

            }
            return c;
        }
        public bool help_fun6_check_if_is_beetwin(OneCoordinate small, OneCoordinate larg, int index)
        {
            if (small > larg)
            {
                OneCoordinate temp = small;
                small = larg;
                larg = temp;
            }
            try
            {
                if (coordinates[index] >= small && coordinates[index] <= larg)
                    return true;
            }
            catch (Exception e)
            {
                //  Console.WriteLine(e.Message);
            }
            return false;
        }

        public override string ToString()
        {
            string str = "";
            foreach (OneCoordinate c in coordinates)
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


    //class that responsive on indexes (for all indexes, the smallest value is 0)
    class MyIndex
    {
        int index, max;
        public MyIndex(int index, int max)
        {
            this.max = max;

            set(index);
        }
        public MyIndex(MyIndex myIndex) : this(myIndex.index, myIndex.max) { }


        public void set(int index)
        {
            if (index > max)
                index = index % (max + 1);
            this.index = index;
        }
        public int get()
        {
            return index;
        }
        public static MyIndex operator ++(MyIndex myIndex)
        {
            if (myIndex.max > myIndex.index)
                myIndex.index++;
            else
                myIndex.index = 0;
            return myIndex;
        }
        public static MyIndex operator --(MyIndex myIndex)
        {
            myIndex.index--;
            return myIndex;
        }
        public static bool operator <(MyIndex myIndex, int i)
        {
            return myIndex.index < i;
        }
        public static bool operator >(MyIndex myIndex, int i)
        {
            return myIndex.index > i;
        }
        public static bool operator <(int i, MyIndex myIndex)
        {
            return i < myIndex.index;
        }
        public static bool operator >(int i, MyIndex myIndex)
        {
            return i > myIndex.index;
        }
        public static bool operator ==(MyIndex myIndex, int i)
        {
            return myIndex.index == i;
        }
        public static bool operator !=(MyIndex myIndex, int i)
        {
            return myIndex.index != i;
        }
        public static bool operator ==(int i, MyIndex myIndex)
        {
            return myIndex.index == i;
        }
        public static bool operator !=(int i, MyIndex myIndex)
        {
            return myIndex.index != i;
        }
        public static bool operator <(MyIndex myIndex, MyIndex myIndex2)
        {
            return myIndex.index < myIndex2;
        }
        public static bool operator >(MyIndex myIndex, MyIndex myIndex2)
        {
            return myIndex.index > myIndex2;
        }
        public static bool operator ==(MyIndex myIndex, MyIndex myIndex2)
        {
            return myIndex.index == myIndex2;
        }
        public static bool operator !=(MyIndex myIndex, MyIndex myIndex2)
        {
            return myIndex.index != myIndex2;
        }
        public override string ToString()
        {
            return String.Format("index= {0}, max= {1}", index, max);
        }
    }
}

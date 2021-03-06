using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
        public static double thicknessOfBlade = 0.5;
        static Coordinates C;
        static List<piece> pieses;
        static List<PieseAndLocation> pieseAndLocationS;


        static HttpListener httpListener = new HttpListener();
        static void Main(string[] args)
        {
            HttpListenerPrefixCollection prefixes = httpListener.Prefixes;
            prefixes.Add("http://localhost:12345/WoodcatCalculator/");
            httpListener.Start();






            while (true)
            {
                pieses = new List<piece>();
                pieseAndLocationS = new List<PieseAndLocation>();

                HttpListenerContext context;
                context = httpListener.GetContext();


                //------------------------------
                DateTime start = DateTime.Now;
                //-----------------------------------

                HttpListenerRequest req;
                HttpListenerResponse res;
                req = context.Request;
                res = context.Response;
                res.AddHeader("Access-Control-Allow-Origin", " *");
                byte[] reqArray = new byte[req.ContentLength64];

                //the request must look like this:
                //
                //          plates: (5, 5);
                //          cuts: (4, 1) + (4, 1) + (1, 4) + (1, 4) + (3, 3);
                //          blade: 0

                req.InputStream.Read(reqArray, 0, reqArray.Length);

                string str = Encoding.ASCII.GetString(reqArray);
                string[] strArray = str.Split(new char[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[][] str2D = new string[strArray.Length][];

                for (int i = 0; i < strArray.Length; i++)
                {
                    Console.WriteLine(strArray[i]);
                    str2D[i] = strArray[i].Split(':');
                }
                for (int i = 0; i < str2D.Length; i++)
                {
                    Console.WriteLine(str2D[i][0]);
                    Console.WriteLine(str2D[i][1]);
                    switch (str2D[i][0])
                    {
                        case "plates":
                            initPlates(str2D[i][1].Split('+'));
                            break;

                        case "cuts":
                            initPieses(str2D[i][1].Split('+'));
                            break;

                        case "blade":
                            initThicknessOfBlade(str2D[i][1]);
                            break;

                        default:
                            throw new Exception("the input ERROR in iterate: " + i + 1);
                    }
                }
                List<Coordinates> ll = new List<Coordinates>();
                ll.Add(new Coordinates(C));
                pieses.Sort();
                foreach (piece item in pieses)
                {
                    Console.WriteLine(item);
                }
                Check3(ll, 0, new List<PieseAndLocation>());

                string response = "[\n";
                for (int i = pieseAndLocationS.Count - 1; i > 0; i--)
                {
                    response += pieseAndLocationS[i] + ",\n";
                }
                if (pieseAndLocationS.Count > 0)
                    response += pieseAndLocationS[0] + "\n]";
                else
                    response = "can't insert all cuts!";
                Console.WriteLine(response);
                byte[] resArray = Encoding.ASCII.GetBytes(response);
                res.ContentType = "json";

                res.OutputStream.Write(resArray, 0, resArray.Length);

                res.Close();

                //------------------------------
                DateTime end = DateTime.Now;
                Console.WriteLine("\n\n\n");
                Console.WriteLine(end - start);
                //-----------------------------------

            }
        }
        static void initPlates(string[] str)
        {
            onePlate p = new onePlate(str[0]);
            C = new Coordinates(p.plate);
        }
        static void initPieses(string[] str)
        {
            foreach (var item in str)
            {
                pieses.Add(new piece(item));
            }
        }
        static void initThicknessOfBlade(string str)
        {
            try
            {
                thicknessOfBlade = double.Parse(str);
            }
            catch
            {
                throw new Exception("the blade input is ERROR");
            }
        }

        static void Check(List<Coordinates> coordList, int indexOfPieses)
        {
            List<Option> options = createOptionsList(coordList, pieses[indexOfPieses]);

            for (int i = 0; i < options.Count; i++)
            {
                if (indexOfPieses == pieses.Count - 1)//that is we complete cut all pieses, so we finished to caculate
                {
                    pieseAndLocationS.Add(options[0].piese_loocation);
                    return;
                }

                //requrse call
                Check(options[i].listCoordinates, indexOfPieses + 1);
                if (pieseAndLocationS.Count > 0)
                {
                    pieseAndLocationS.Add(options[i].piese_loocation);
                    return;
                }
            }
        }

        //אני חיב לסיים את הפונ' הזו מחר עד 12 בצהריים, ולהתקדם הלאה עם המשימות!! בהצלחה
        //הפונ' לא עובדת טוב, אני אמור ליצור pieseAndLocationS2 עבור כל איטרציה של ריקורסיה וכל pieseAndLocationS2
        //כזה, יכלול בעצם את כל הערכים של pieseAndLocationS2 שקדם לו, ורק כאשר יסתיימו החתיכות אז הpieseAndLocationS יכיל את 
        //הpieseAndLocationS2   האחרון שהוא הכי עדכני
        //ובמידה ובאחת מאיטרציות הרקורסיה, מערך ה options ריק, אז צריך להתקדם לאיטרציה הבאה עם pieseAndLocationS2 הקודם.
        //כמובן שלפי זה יוצא שהפונקציה צריכה לקבל משתנה של pieseAndLocationS2 וכך בכל קריאה רקורסיבית, 
        //הפונקציה תקבל כפרמטר את הpieseAndLocationS2 שלה והיא בעצמה תיצור אחד חדש כזה 
        //ואז תעתיק את כל התאים של ה pieseAndLocationS2 שהתקבל כפרמטר, אל תוך המערך החדש שזה עתה היא יצרה
        static void Check2(List<Coordinates> listCoordinates, int indexOfPieses, List<PieseAndLocation> locationsPrevius)
        {
            List<PieseAndLocation> locationsCurrent;
            List<Option> options = createOptionsList(listCoordinates, pieses[indexOfPieses]);
            int i = 0;
            do
            {
                if (indexOfPieses == pieses.Count - 1)//that is we complete cut all pieses, so we finished to caculate
                {
                    locationsCurrent = new List<PieseAndLocation>(locationsPrevius);
                    if (options.Count > 0)
                        locationsCurrent.Add(options[0].piese_loocation);

                    if (pieseAndLocationS.Count < locationsCurrent.Count)
                    {
                        pieseAndLocationS.RemoveRange(0, pieseAndLocationS.Count);
                        pieseAndLocationS.InsertRange(0, locationsCurrent);
                    }
                    return;
                }
                if (options.Count > 0)
                {
                    locationsCurrent = new List<PieseAndLocation>(locationsPrevius);
                    locationsCurrent.Add(options[i].piese_loocation);
                    Check2(options[i].listCoordinates, indexOfPieses + 1, locationsCurrent);
                }
                else
                    Check2(listCoordinates, indexOfPieses + 1, locationsPrevius);
                if (pieseAndLocationS.Count == pieses.Count)
                    return;
                i++;
            } while (i < options.Count);
        }
        static void Check3(List<Coordinates> listCoordinates, int indexOfPieses, List<PieseAndLocation> locationsPrevius)
        {
            List<PieseAndLocation> locationsCurrent;
            List<Option> options = createOptionsList(listCoordinates, pieses[indexOfPieses]);

            if (indexOfPieses == pieses.Count - 1)//that is we complete cut all pieses, so we finished to caculate
            {
              
                locationsCurrent = new List<PieseAndLocation>(locationsPrevius);
                if (options.Count > 0)
                    locationsCurrent.Add(options[0].piese_loocation);

                if (pieseAndLocationS.Count < locationsCurrent.Count)
                {
                    pieseAndLocationS.RemoveRange(0, pieseAndLocationS.Count);
                    pieseAndLocationS.InsertRange(0, locationsCurrent);
                }
              
                return;
            }
            for (int i = 0; i < options.Count; i++)
            {
                locationsCurrent = new List<PieseAndLocation>(locationsPrevius);
                locationsCurrent.Add(options[i].piese_loocation);
                Check3(options[i].listCoordinates, indexOfPieses + 1, locationsCurrent);
                if (pieseAndLocationS.Count == pieses.Count)
                    return;
            }
            if (options.Count == 0)
                Check3(listCoordinates, indexOfPieses + 1, locationsPrevius);
        }

        static endNode pointToEndNode;
        static node check4(List<Coordinates> listCoordinates)
        {
            pointToEndNode = new endNode(null, listCoordinates, -1);
            node nod = check4(listCoordinates, 0);
            while (pointToEndNode.index < pieses.Count - 1)
               pointToEndNode.pointToEnd.next= check4(pointToEndNode.coordinatesList, pointToEndNode.index+1);
            return nod;
        }
        static node check4(List<Coordinates> listCoordinates, int indexOfPieses)
        {
            if (indexOfPieses == pieses.Count)
                return null;

            node  temp= new node();
            node nod = new node();
            List<Option> options = createOptionsList(listCoordinates, pieses[indexOfPieses]);

            for (int i = 0; i < options.Count; i++)
            {
                temp.value = options[i].piese_loocation;                
                temp.next = check4(options[i].listCoordinates, indexOfPieses + 1);
                if (temp.next != null)
                    temp.offstring = temp.next.offstring + 1;
                else
                    temp.offstring = 0;

                if (nod.offstring < temp.offstring)                
                    nod = new node(temp);

                if (pointToEndNode.index < indexOfPieses)
                    pointToEndNode.set(nod, options[i].listCoordinates, indexOfPieses);

                if (pointToEndNode.index==pieses.Count-1)                
                    return nod;          
            }
            return null;
        }


        //this function gets 'List<Coordinates>' and one 'piece',
        //then return 'List<Option>'for catting the piese was given,
        //that contains the all options, and sorts them order best to worst,
        //(the best option is it that has min "cordinates[]" and for each  (cordinates[]) it has min coordinates)
        static List<Option> createOptionsList(List<Coordinates> coordinatesList, piece p)
        {
            List<Option> options = new List<Option>();
            for (int i = 0; i < coordinatesList.Count; i++)
            {
                for (int i2 = 0; i2 < coordinatesList[i].count(); i2++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 1)
                            p.turn();
                        if (coordinatesList[i].check_if_is_external(i2) && coordinatesList[i].fun4(p, i2))
                        {
                            PieseAndLocation pisLocat = new PieseAndLocation(coordinatesList[i].coordinates[i2], new piece(p));
                            List<Coordinates> coordList = coordinatesList[i].fun6(p, i2);
                            Option option = new Option(pisLocat, coordList);
                            options.Add(option);
                        }
                    }
                    if (coordinatesList[i].count() == 4)
                        break;
                }
            }
            options.Sort();
            return options;
        }
    }
    class endNode
    {
        public node pointToEnd;
        public List<Coordinates> coordinatesList;
        public int index;
        public endNode(node n, List<Coordinates> l, int i)
        {
            pointToEnd = n;
            coordinatesList = l;
            index = i;
        }
        public void set(node n, List<Coordinates> l,int i)
        {
            pointToEnd = n;
            coordinatesList = l;
            index = i;
        }
    }

    class node
    {
        public node next=null;
        public int offstring = -1;
        public bool arrivedToEnd = false;
        public PieseAndLocation value = null;
        public node(){}
        public node(node n)
        {
            next = n.next;
            offstring = n.offstring;
            arrivedToEnd = n.arrivedToEnd;
        }
    }
    class PieseAndLocation
    {
        public double x_left;
        public double y_down;
        public bool isVertical;
        public piece p;
        public PieseAndLocation()
        {

        }
        public PieseAndLocation(OneCoordinate oc, piece p_)
        {
            isVertical = p_.Length >= p_.Width;
            p = p_;
            init_x_left_y_down(oc);
        }
        void init_x_left_y_down(OneCoordinate oc)
        {
            switch (oc.type)
            {
                case types.top:
                    x_left = oc.x - p.Width;
                    y_down = oc.y;
                    break;
                case types.down:
                    x_left = oc.x;
                    y_down = oc.y - p.Length;
                    break;
                case types.right:
                    x_left = oc.x;
                    y_down = oc.y;
                    break;
                case types.left:
                    x_left = oc.x - p.Width;
                    y_down = oc.y - p.Length;
                    break;
                default:
                    throw new Exception();
            }
        }
        public override string ToString()
        {
            return string.Format("{3}\"in_coordinate\":{3}\"left\":\"{0}\",\"bottom\":\"{1}\"{4},\"put_piese\":{2}{4}", x_left, y_down, p, "{", "}");
        }
    }
    class Option : IComparable<Option>
    {
        public PieseAndLocation piese_loocation;
        public List<Coordinates> listCoordinates;
        public Option(PieseAndLocation pl, List<Coordinates> l)
        {
            piese_loocation = pl;
            listCoordinates = l;
        }

        public int CompareTo(Option other)
        {
            return sumOfCounts(other);
        }

        //this fun recives another "option" and  checks if its count is larger from "this" or smaller
        // if is equals, so checks the  sum of all counts in the list ("List<Coordinates> cl")
        //fun returns -1 if "this" smaller from the "option" parameter, 1 if "this" is larger, 0 if is equals
        public int sumOfCounts(Option o)
        {
            if (listCoordinates.Count < o.listCoordinates.Count)
                return -1;
            if (listCoordinates.Count > o.listCoordinates.Count)
                return 1;
            int sum1, sum2;
            sum1 = sum2 = 0;
            for (int i = 0; i < listCoordinates.Count; i++)
            {
                sum1 += listCoordinates[i].coordinates.Count;
                sum2 += o.listCoordinates[i].coordinates.Count;
            }
            if (sum1 < sum2)
                return -1;
            if (sum1 > sum2)
                return 1;
            return 0;
        }
        //----------------------------
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < listCoordinates.Count; i++)
            {
                str += listCoordinates[i] + "\n";
            }
            return "piese_loocation:  " + piese_loocation + " \nlistCoordinates:  " + str;
        }
        //-------------------------------------
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

        //  private double x_right, x_left, y_top, y_down;
        // private double mostTop, mostLeft, mostRight, top_down, down_top, mostDown, right_left, left_right;

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
            UpdateTypeAndRemoveUnnecessaryCoordinate();
            if (checkIsInvalid())
                throw new Exception("the coordinates is invalid");

        }

        public Coordinates(Coordinates C) : this(C.coordinates) { }

        //insert new coordinate or coordinates from the specified undex, update "type" field, and removing unnecessary cordinate from 
        //the coordinates array, finaly, checking if is valid or not.
        public void insertRange(int index, OneCoordinate[] arr)
        {
            coordinates.InsertRange(index, arr);

            UpdateTypeAndRemoveUnnecessaryCoordinate();

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

        //"fun4" get a given piece and index, then it checks if it can cat this piece from the array at the index given,
        //"fun4" uses at one function "help_fun4_init_values"  which initializes values,
        //also uses at class "myIndex" for some indexes.
        public bool fun4(piece p, int index_of_coord)
        {

            double x_right, x_left, y_top, y_down;

            help_fun4_init_values(out x_right, out x_left, out y_top, out y_down, index_of_coord, p);

            MyIndex current = new MyIndex(index_of_coord + 1, coordinates.Count - 1);
            MyIndex next;

            while (current != index_of_coord)
            {
                if (coordinates[current.get()].type == types.left || coordinates[current.get()].type == types.right)
                {
                    if (coordinates[current.get()].y < y_top && coordinates[current.get()].y > y_down)
                    {
                        next = new MyIndex(current.get() + 1, coordinates.Count - 1);
                        if ((coordinates[current.get()].x >= x_right && coordinates[next.get()].x < x_right) || (coordinates[current.get()].x <= x_left && coordinates[next.get()].x > x_left))
                            return false;
                        current++;
                    }
                }
                else
                {
                    if (coordinates[current.get()].x < x_right && coordinates[current.get()].x > x_left)
                    {
                        next = new MyIndex(current.get() + 1, coordinates.Count - 1);
                        if ((coordinates[current.get()].y >= y_top && coordinates[next.get()].y < y_top) || (coordinates[current.get()].y <= y_down && coordinates[next.get()].y > y_down))
                            return false;
                        current++;
                    }
                }
                current++;
            }

            return true;
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

            Coordinates c_temp, c_temp2;
            OneCoordinate[] c = help_fun_6_create_sub_array_with_the_new_coordinates(here_insert, p);

            c_temp = new Coordinates(coordinates);

            c_temp.coordinates.RemoveAt(here_insert);

            //i use in "insertRang" function, not "InsertRange", because i need it to play some functions...
            c_temp.insertRange(here_insert, c);

            //for init values of c_temp,that his values dosn't initialed
            double x_right, x_left, y_top, y_down;
            help_fun4_init_values(out x_right, out x_left, out y_top, out y_down, here_insert, p);
            c_temp.sawdustCalculate(x_right, x_left, y_top, y_down);

            AllList.Add(c_temp);

            for (int i = 0; i < AllList.Count; i++)
            {

                MyIndex next, after_next, index_checked, before_index_checked;

                for (int j = 0; j < AllList[i].coordinates.Count; j++)
                {

                    next = new MyIndex(j + 1, AllList[i].coordinates.Count - 1);
                    after_next = new MyIndex(j + 2, AllList[i].coordinates.Count - 1);
                    index_checked = new MyIndex(j + 2, AllList[i].coordinates.Count - 1);

                    while (index_checked != j)
                    {
                        if (AllList[i].help_fun6_check_if_is_beetwin(AllList[i].coordinates[j], AllList[i].coordinates[next.get()], index_checked.get()))
                        {
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

                            if (!(j < AllList[i].coordinates.Count))
                                break;
                            next = new MyIndex(j + 1, AllList[i].coordinates.Count - 1);
                            after_next = new MyIndex(j + 2, AllList[i].coordinates.Count - 1);
                            index_checked = new MyIndex(j + 2, AllList[i].coordinates.Count - 1);
                        }
                        else
                        {
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

        public void sawdustCalculate(double right_left, double left_right, double top_down, double down_top)
        {
            double mostTop, mostLeft, mostRight, mostDown;

            mostTop = top_down + Program.thicknessOfBlade;
            mostLeft = left_right - Program.thicknessOfBlade;
            mostRight = right_left + Program.thicknessOfBlade;
            mostDown = down_top - Program.thicknessOfBlade;


            for (int i = 0; i < coordinates.Count; i++)
            {
                MyIndex next = new MyIndex(i + 1, coordinates.Count - 1);
                if (coordinates[i].type == types.right || coordinates[i].type == types.left)
                {
                    if (coordinates[i].y <= down_top && coordinates[i].y > mostDown)
                        i = helpSawdustCalculateDown(mostRight, right_left, mostLeft, left_right, mostTop, top_down, mostDown, down_top, i, next.get());
                    else if (coordinates[i].y < mostTop && coordinates[i].y >= top_down)
                        i = helpSawdustCalculateTop(mostRight, right_left, mostLeft, left_right, mostTop, top_down, mostDown, down_top, i, next.get());
                }

                else if (coordinates[i].type == types.top || coordinates[i].type == types.down)
                {
                    if (coordinates[i].x <= left_right && coordinates[i].x > mostLeft)
                        i = helpSawdustCalculateRight(mostRight, right_left, mostLeft, left_right, mostTop, top_down, mostDown, down_top, i, next.get());
                    else if (coordinates[i].x >= right_left && coordinates[i].x < mostRight)
                        i = helpSawdustCalculateLeft(mostRight, right_left, mostLeft, left_right, mostTop, top_down, mostDown, down_top, i, next.get());
                }
                else
                    throw new Exception("it miss type");
            }
            UpdateTypeAndRemoveUnnecessaryCoordinate();
        }

        int helpSawdustCalculateDown(double mostRight, double right_left, double mostLeft, double left_right, double mostTop, double top_down, double mostDown, double down_top, int i, int next)
        {
            if (coordinates[i].x <= mostLeft)
            {
                if (coordinates[next].x >= mostRight)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostLeft,coordinates[i].y),
                                    new OneCoordinate(mostLeft,mostDown),
                                    new OneCoordinate(mostRight,mostDown),
                                    new OneCoordinate(mostRight,coordinates[i].y)
                                });
                    i += 4;// new MyIndex(i + 4, coordinates.Count).get();
                }
                //@{
                else if (coordinates[next].x >= right_left)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostLeft,coordinates[i].y),
                                    new OneCoordinate(mostLeft,mostDown),
                                    new OneCoordinate(mostRight,mostDown)
                                });
                    i += 3;
                }

                else if (coordinates[next].x > left_right)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostLeft,coordinates[i].y),
                                    new OneCoordinate(mostLeft,mostDown),
                                    new OneCoordinate(coordinates[next].x,mostDown)
                                });
                    i += 3;
                }
                //@}

                else if (coordinates[next].x > mostLeft)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostLeft,coordinates[i].y),
                                    new OneCoordinate(mostLeft,mostDown)
                                });
                    i += 2;
                }
            }
            else if (coordinates[i].x >= mostRight)
            {
                if (coordinates[next].x <= mostLeft)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostRight,coordinates[i].y),
                                    new OneCoordinate(mostRight,mostDown),
                                    new OneCoordinate(mostLeft,mostDown),
                                    new OneCoordinate(mostLeft,coordinates[i].y)
                                });
                    i += 4;
                }
                //@{
                else if (coordinates[next].x <= left_right)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostRight,coordinates[i].y),
                                    new OneCoordinate(mostRight,mostDown),
                                    new OneCoordinate(mostLeft,mostDown)
                                });
                    i += 3;
                }

                else if (coordinates[next].x < right_left)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostRight,coordinates[i].y),
                                    new OneCoordinate(mostRight,mostDown),
                                    new OneCoordinate(coordinates[next].x,mostDown)
                                });
                    i += 3;
                }
                //@}

                else if (coordinates[next].x < mostRight)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostRight,coordinates[i].y),
                                    new OneCoordinate(mostRight,mostDown)
                                });
                    i += 2;
                }
            }
            else
            {
                if (coordinates[next].x <= mostLeft)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostDown),
                                    new OneCoordinate(mostLeft,mostDown),
                                    new OneCoordinate(mostLeft,coordinates[i].y)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 2;
                }
                //@{
                else if (coordinates[next].x <= left_right)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostDown),
                                    new OneCoordinate(mostLeft,mostDown)
                                }); if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 1;
                }
                //@}
                else if (coordinates[next].x >= mostRight)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostDown),
                                    new OneCoordinate(mostRight,mostDown),
                                    new OneCoordinate(mostRight,coordinates[i].y)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 2;
                }
                //@{
                else if (coordinates[next].x >= right_left)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostDown),
                                    new OneCoordinate(mostRight,mostDown)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 1;
                }
                //@}
                else
                {
                    coordinates[i].y = mostDown;
                    //   coordinates.Insert(next, new OneCoordinate(coordinates[next].x, mostDown));
                    coordinates[next].y = mostDown;
                    i += 1;
                }
            }
            return i;
        }
        int helpSawdustCalculateTop(double mostRight, double right_left, double mostLeft, double left_right, double mostTop, double top_down, double mostDown, double down_top, int i, int next)
        {
            if (coordinates[i].x <= mostLeft)
            {
                if (coordinates[next].x >= mostRight)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostLeft,coordinates[i].y),
                                    new OneCoordinate(mostLeft,mostTop),
                                    new OneCoordinate(mostRight,mostTop),
                                    new OneCoordinate(mostRight,coordinates[i].y)
                                });
                    i += 4;
                }
                //@{
                else if (coordinates[next].x >= right_left)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostLeft,coordinates[i].y),
                                    new OneCoordinate(mostLeft,mostTop),
                                    new OneCoordinate(mostRight,mostTop)
                                });
                    i += 3;
                }

                else if (coordinates[next].x > left_right)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostLeft,coordinates[i].y),
                                    new OneCoordinate(mostLeft,mostTop),
                                    new OneCoordinate(coordinates[next].x,mostTop)
                                });
                    i += 3;
                }
                //@}

                else if (coordinates[next].x > mostLeft)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostLeft,coordinates[i].y),
                                    new OneCoordinate(mostLeft,mostTop)
                                });
                    i += 2;
                }
            }
            else if (coordinates[i].x >= mostRight)
            {
                if (coordinates[next].x <= mostLeft)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostRight,coordinates[i].y),
                                    new OneCoordinate(mostRight,mostTop),
                                    new OneCoordinate(mostLeft,mostTop),
                                    new OneCoordinate(mostLeft,coordinates[i].y)
                                });
                    i += 4;
                }
                //@{
                else if (coordinates[next].x <= left_right)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostRight,coordinates[i].y),
                                    new OneCoordinate(mostRight,mostTop),
                                    new OneCoordinate(mostLeft,mostTop)
                                });
                    i += 3;
                }
                else if (coordinates[next].x < right_left)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostRight,coordinates[i].y),
                                    new OneCoordinate(mostRight,mostTop),
                                    new OneCoordinate(coordinates[next].x,mostTop)
                                });
                    i += 3;
                }
                //@}
                else if (coordinates[next].x < mostRight)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostRight,coordinates[i].y),
                                    new OneCoordinate(mostRight,mostTop)
                                });
                    i += 2;
                }
            }
            else
            {
                if (coordinates[next].x <= mostLeft)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostTop),
                                    new OneCoordinate(mostLeft,mostTop),
                                    new OneCoordinate(mostLeft,coordinates[i].y)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 2;
                }
                //@{
                else if (coordinates[next].x <= left_right)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostTop),
                                    new OneCoordinate(mostLeft,mostTop)
                                });

                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i); i += 1;
                }
                //@}
                else if (coordinates[next].x >= mostRight)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostTop),
                                    new OneCoordinate(mostRight,mostTop),
                                    new OneCoordinate(mostRight,coordinates[i].y)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 2;
                }
                //@{
                else if (coordinates[next].x >= right_left)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostTop),
                                    new OneCoordinate(mostRight,mostTop)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 1;
                }
                //@}
                else
                {
                    coordinates[i].y = mostTop;
                    coordinates[next].y = mostTop;
                    i += 1;
                }
            }
            return i;
        }
        int helpSawdustCalculateRight(double mostRight, double right_left, double mostLeft, double left_right, double mostTop, double top_down, double mostDown, double down_top, int i, int next)
        {
            if (coordinates[i].y <= mostDown)
            {
                if (coordinates[next].y >= mostTop)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostDown),
                                    new OneCoordinate(mostLeft,mostDown),
                                    new OneCoordinate(mostLeft,mostTop),
                                    new OneCoordinate(coordinates[i].x,mostTop)
                                });
                    i += 4;// new MyIndex(i + 4, coordinates.Count).get();
                }
                //@{
                else if (coordinates[next].y >= top_down)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostDown),
                                    new OneCoordinate(mostLeft,mostDown),
                                    new OneCoordinate(mostLeft,mostTop)
                                });
                    i += 3;
                }
                else if (coordinates[next].y > down_top)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostDown),
                                    new OneCoordinate(mostLeft,mostDown),
                                    new OneCoordinate(mostLeft,coordinates[next].y)
                                });
                    i += 3;
                }
                //@}
                else if (coordinates[next].y > mostDown)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostDown),
                                    new OneCoordinate(mostLeft,mostDown)
                                });
                    i += 2;
                }
            }
            else if (coordinates[i].y >= mostTop)
            {
                if (coordinates[next].y <= mostDown)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostTop),
                                    new OneCoordinate(mostLeft,mostTop),
                                    new OneCoordinate(mostLeft,mostDown),
                                    new OneCoordinate(coordinates[i].x,mostDown)
                                });
                    i += 4;
                }
                //@{
                else if (coordinates[next].y <= down_top)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostTop),
                                    new OneCoordinate(mostLeft,mostTop),
                                    new OneCoordinate(mostLeft,mostDown)
                                });
                    i += 3;
                }
                else if (coordinates[next].y < top_down)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostTop),
                                    new OneCoordinate(mostLeft,mostTop),
                                    new OneCoordinate(mostLeft,coordinates[next].y)
                                });
                    i += 3;
                }
                //@}
                else if (coordinates[next].y < mostTop)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostTop),
                                    new OneCoordinate(mostLeft,mostTop)
                                });
                    i += 2;
                }
            }
            else
            {
                if (coordinates[next].y <= mostDown)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostLeft,coordinates[i].y),
                                    new OneCoordinate(mostLeft,mostDown),
                                    new OneCoordinate(coordinates[i].x,mostDown)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 2;
                }
                //@{
                else if (coordinates[next].y <= down_top)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostLeft,coordinates[i].y),
                                    new OneCoordinate(mostLeft,mostDown)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 1;
                }
                //@}
                else if (coordinates[next].y >= mostTop)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostLeft,coordinates[i].y),
                                    new OneCoordinate(mostLeft,mostTop),
                                    new OneCoordinate(coordinates[i].x,mostTop)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 2;
                }
                //@{
                else if (coordinates[next].y >= top_down)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostLeft,coordinates[i].y),
                                    new OneCoordinate(mostLeft,mostTop)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 1;
                }
                //@}
                else
                {
                    coordinates[i].x = mostLeft;
                    coordinates[next].x = mostLeft;
                    i += 1;
                }
            }
            return i;
        }
        int helpSawdustCalculateLeft(double mostRight, double right_left, double mostLeft, double left_right, double mostTop, double top_down, double mostDown, double down_top, int i, int next)
        {
            if (coordinates[i].y <= mostDown)
            {
                if (coordinates[next].y >= mostTop)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostDown),
                                    new OneCoordinate(mostRight,mostDown),
                                    new OneCoordinate(mostRight,mostTop),
                                    new OneCoordinate(coordinates[i].x,mostTop)
                                });
                    i += 4;// new MyIndex(i + 4, coordinates.Count).get();
                }
                //@{
                else if (coordinates[next].y >= top_down)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostDown),
                                    new OneCoordinate(mostRight,mostDown),
                                    new OneCoordinate(mostRight,mostTop)
                                });
                    i += 3;
                }
                else if (coordinates[next].y > down_top)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostDown),
                                    new OneCoordinate(mostRight,mostDown),
                                    new OneCoordinate(mostRight,coordinates[next].y)
                                });
                    i += 3;
                }
                //@}
                else if (coordinates[next].y > mostDown)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostDown),
                                    new OneCoordinate(mostRight,mostDown)
                                });
                    i += 2;
                }
            }
            else if (coordinates[i].y >= mostTop)
            {
                if (coordinates[next].y <= mostDown)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostTop),
                                    new OneCoordinate(mostRight,mostTop),
                                    new OneCoordinate(mostRight,mostDown),
                                    new OneCoordinate(coordinates[i].x,mostDown)
                                });
                    i += 4;
                }
                //@{
                else if (coordinates[next].y <= down_top)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostTop),
                                    new OneCoordinate(mostRight,mostTop),
                                    new OneCoordinate(mostRight,mostDown)
                                });
                    i += 3;
                }
                else if (coordinates[next].y < top_down)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostTop),
                                    new OneCoordinate(mostRight,mostTop),
                                    new OneCoordinate(mostRight,coordinates[next].y)
                                });
                    i += 3;
                }
                //@}
                else if (coordinates[next].y < mostTop)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(coordinates[i].x,mostTop),
                                    new OneCoordinate(mostRight,mostTop)
                                });
                    i += 2;
                }
            }
            else
            {
                if (coordinates[next].y <= mostDown)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostRight,coordinates[i].y),
                                    new OneCoordinate(mostRight,mostDown),
                                    new OneCoordinate(coordinates[i].x,mostDown)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 2;
                }
                //@{
                else if (coordinates[next].y <= down_top)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostRight,coordinates[i].y),
                                    new OneCoordinate(mostRight,mostDown)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 1;
                }
                //@}
                else if (coordinates[next].y >= mostTop)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostRight,coordinates[i].y),
                                    new OneCoordinate(mostRight,mostTop),
                                    new OneCoordinate(coordinates[i].x,mostTop)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 2;
                }
                //@{
                else if (coordinates[next].y >= top_down)
                {
                    coordinates.InsertRange(next, new OneCoordinate[] {
                                    new OneCoordinate(mostRight,coordinates[i].y),
                                    new OneCoordinate(mostRight,mostTop)
                                });
                    if (next == 0)
                        coordinates.RemoveAt(coordinates.Count - 1);
                    else
                        coordinates.RemoveAt(i);
                    i += 1;
                }
                //@}
                else
                {
                    coordinates[i].x = mostRight;
                    coordinates[next].x = mostRight;
                    i += 1;
                }
            }
            return i;
        }
    }
    class onePlate
    {
        public OneCoordinate[] plate = new OneCoordinate[4];

        public onePlate(double length, double width)
        {
            if (width > length)
            {
                length += width;
                width = length - width;
                length -= width;
            }

            plate[0] = new OneCoordinate(0, 0);
            plate[1] = new OneCoordinate(length, 0);
            plate[2] = new OneCoordinate(length, width);
            plate[3] = new OneCoordinate(0, width);
        }
        public onePlate(string str)
        {
            string[] str_arr = str.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
            double length, width;
            try
            {
                width = double.Parse(str_arr[0]);
                length = double.Parse(str_arr[1]);
            }
            catch
            {
                throw new Exception("the plate input is ERROR");
            }
            if (width > length)
            {
                length += width;
                width = length - width;
                length -= width;
            }

            plate[0] = new OneCoordinate(0, 0);
            plate[1] = new OneCoordinate(length, 0);
            plate[2] = new OneCoordinate(length, width);
            plate[3] = new OneCoordinate(0, width);

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
        public piece(string str)
        {
            string[] str_arr = str.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
            double length, width;
            try
            {
                width = double.Parse(str_arr[0]);
                length = double.Parse(str_arr[1]);
            }
            catch
            {
                throw new Exception("the piece input is ERROR");
            }
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
        public piece(piece p)
        {
            Length = p.Length;
            Width = p.Width;
        }
        public void turn()
        {
            double temp = Length;
            Length = Width;
            Width = temp;

        }

        public override string ToString()
        {
            string str = string.Format("{2}\"hight\":\"{0}\",\"width\":\"{1}\"{3}", Length, Width, "{", "}");
            return str;
            //  return "(" + Length + " x " + Width + ")";
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

﻿/*////////////////////////////////////////////////////////////////////////////////////////////////
 * This Program been writen in Visual Studio Express 2012.
 * 
 * Brief Overview
 * Current Features:
 *      Manuel Selection of Generation point
 *      Drawing of Points
 *      Drawing of Lines
 *      Selection of either having points or lines ON or OFF but not both OFF at the same time
 *      Remove Extra Lines After Creation
 *      
 * TODO:
 *      Prevent the Creation of Extra Lines
 *      Dynamic Search Algoritm
 * 
 * Expanded Overview
 * This Program creates hexagons at "Random" through choosing a "Random" path.
 * There is a drawing point which originates by defualt at the middle of the form althuogh this can
 * be change to any were through double clicking any where on the picturebox. At generation the point
 * chooses a "Random" direction in which to travel from the start_directions list. The point is then
 * added to the points list. The point then travels 30 pixels and changes direction again. This
 * continues until the point is off of the picurebox, in which case it is then destroyed and a new 
 * point created at the generation location.
/*//////////////////////////////////////////////////////////////////////////////////////////////////

/*Flow Diagrams//////////////////////////////////////////////////////////////
 *                               ----------------
 *                            -->| Point Select | <--------------
 *                            |  ----------------               |
 *                            |          |                      |
 *                            |         \/                      |
 *                            |  ---------------------          |
 *                            |  | Is point 30 units |       ------
 *                            |  | from were it      | ----> | No |
 *                            |  | started the line  |       ------
 *                            |  ---------------------
 *                            |             |
 *                            |            \/
 *                            |           -------
 *                            |           | Yes |
 *                            |           -------
 *                            |              |
 *                            |             \/
 *                            |          --------------------
 *                            -----------| Change Direction |
 *                                       --------------------
/*///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Hexagons
{
    public partial class Form1 : Form
    {
        //List of Drawing points
        public List<point> points = new List<point>() { };
        //List of Directions in Which the points can start when generated
        public List<start> start_directions = new List<start>()
        {
            new start { Change_X = -2, Change_Y = 2},
            new start { Change_X = -2, Change_Y = -2},
            new start { Change_X = 2, Change_Y = 0},
            new start { Change_X = 2, Change_Y = 0}
        };
        //List of Generated Lines
        public List<line> lines = new List<line>() { };

        //Whether the User wants the Lines On
        //By Default OFF
        public bool Lines_ON = false;
        
        //Whether the User wants the Points On
        //By Default ON
        public bool Points_ON = true;


        //Whether Generation point has been generated manuelly by the user
        public bool GXYManuel = false;
        //Point generation cordinates
        public int GX;
        public int GY;

        public Form1()
        {
            InitializeComponent();
            GX = pictureBox1.Height >> 1;
            GY = pictureBox1.Width >> 1;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //Check and see if the User has the Lines Turned on
            if (Lines_ON)
            {
                //If so Draw the stored Lines
                foreach (line var in lines)
                {
                    e.Graphics.DrawLine(new Pen(Color.Blue, 2), var.X1, var.Y1, var.X2, var.Y2);
                }
            }
            for (int i = 0; i < points.Count; i++)
            {
                //Check to see if the points are within the bounds of the Picturebox
                if (points[i].Present_X >= 0 &&
                    points[i].Present_X <= pictureBox1.Width &&
                    points[i].Present_Y >= 0 &&
                    points[i].Present_Y <= pictureBox1.Height)
                {
                    //Check and see if the User has the Lines Turned on
                    if (Lines_ON)
                    {
                        //If so Draw a line behind the point
                        e.Graphics.DrawLine(new Pen(Color.Blue, 2), points[i].Last_X, points[i].Last_Y, points[i].Present_X, points[i].Present_Y);
                    }

                    //Check and see if the User has the Points Turned on
                    if (Points_ON)
                    {
                        //If so Draw the points
                        e.Graphics.FillRectangle(new SolidBrush(Color.OrangeRed), points[i].Present_X, points[i].Present_Y, 2F, 2F);
                    }
                }
                //If the Point is outside of the bounds of the screen Destroy it
                else
                {
                    lines.Add(new line { X1 = points[i].Last_X, X2 = points[i].Present_X, Y1 = points[i].Last_Y, Y2 = points[i].Present_Y });
                    points.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //Resets the drawing and restarts when the Size of the Form is Changed
            points.Clear();
            lines.Clear();
            if(!GXYManuel)
            {
                GX = pictureBox1.Width >> 1;
                GY = pictureBox1.Height >> 1;
            }
        }
        
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            //Determines were the mouse was clicked and setting points to be generated there
            Point mouse = new Point(MousePosition.X, MousePosition.Y);
            mouse = PointToClient(mouse);
            GX = mouse.X;
            GY = mouse.Y - 25;
            GXYManuel = true;
        }
        
        //Timers///////////////////////////////////////////////////////
        private void Refresh_Timer_Tick(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            for ( int i = 0; i < points.Count; i++)
            {
                points[i].Present_X += points[i].Change_X;
                points[i].Present_Y += points[i].Change_Y;
                Check_Direction(i);
            }

            if (points.Count < 50)
            {
                generate();
            }

        }
        private void Check_Timer_Tick(object sender, EventArgs e)
        {
            Check_Overlap();
            /*Thread.MemoryBarrier();
            Thread.BeginCriticalRegion();
            Thread thread = new Thread(new ThreadStart(Check_Overlap));
            thread.Start();*/
        }
        ///////////////////////////////////////////////////////////////

        //MenuStrip Items//////////////////////////////////////////////////////////
        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lines.Clear();
            points.Clear();
        }
        private void dotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Points_ON && Lines_ON)
            {
                Points_ON = false;
            }
            else if (Points_ON && !Lines_ON)
            {
                Points_ON = false;
                Lines_ON = true;
                linesToolStripMenuItem.Checked = true;
            }
            else
            {
                Points_ON = true;
            }
        }
        private void linesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Lines_ON && Points_ON)
            {
                Lines_ON = false;
            }
            else if (Lines_ON && !Points_ON)
            {
                Lines_ON = false;
                Points_ON = true;
                dotsToolStripMenuItem.Checked = true;
            }
            else
            {
                Lines_ON = true;
            }
        }
        ///////////////////////////////////////////////////////////////////////////

        //Personal Methods/////////////////////////
        private void Check_Direction(int Index)
        {
            if (math.Distance(points[Index].Present_X, points[Index].Present_Y, points[Index].Last_X, points[Index].Last_Y) >= 30)
            {
                Random random = new Random();
                double choice = random.NextDouble();
                switch ((int)points[Index].Change_Y)
                {
                    case 2:
                        if (choice >= 0.5)
                        {
                            points[Index].Change_Y -= 2;
                        }
                        else
                        {
                            points[Index].Change_X *= -1;
                        }
                        break;
                    case 0:
                        if (choice >= 0.5)
                        {
                            points[Index].Change_Y += 2;
                        }
                        else
                        {
                            points[Index].Change_Y -= 2;
                        }
                        break;
                    case -2:
                        if (choice >= 0.5)
                        {
                            points[Index].Change_Y += 2;
                        }
                        else
                        {
                            points[Index].Change_X *= -1;
                        }
                        break;

                }
                lines.Add(new line { X1 = points[Index].Last_X, X2 = points[Index].Present_X, Y1 = points[Index].Last_Y, Y2 = points[Index].Present_Y });
                points[Index].Last_X = points[Index].Present_X;
                points[Index].Last_Y = points[Index].Present_Y;
                //Prevent_Overlap(Index);
            }
            else
            {

            }
        }
        private void Check_Overlap()
        {
            for (int i1 = 0; i1 < lines.Count; i1++)
            {
                for (int i2 = 0; i2 < lines.Count; i2++)
                {
                    if (i1 != i2 &&
                        lines[i1].X1 == lines[i2].X1 &&
                        lines[i1].Y1 == lines[i2].Y1 &&
                        lines[i1].X2 == lines[i2].X2 &&
                        lines[i1].Y2 == lines[i2].Y2)
                    {
                        lines.RemoveAt(i2);
                        i2--;
                        //For Testing Purposes to see if prevention algorithm works
                        //MessageBox.Show("Detected a Copy");
                    }
                    else if (i1 != i2 &&
                        lines[i1].X1 == lines[i2].X2 &&
                        lines[i1].Y1 == lines[i2].Y2 &&
                        lines[i1].X2 == lines[i2].X1 &&
                        lines[i1].Y2 == lines[i2].Y1)
                    {
                        lines.RemoveAt(i2);
                        i2--;
                        //MessageBox.Show("Detected a Copy");
                    }
                }
                if (i1 >= lines.Count / 2)
                {
                    break;
                }
            }
        }
        private void Prevent_Overlap(int Index)
        {
            Point point = math.Next_Position(points[Index].Last_X, points[Index].Last_Y, points[Index].Change_X, points[Index].Change_Y, 30);
            for (int i = 0; i < lines.Count; i++)
            {
                //Checking to see if there is a line in the same direction as travel
                if (lines[i].X1 == points[Index].Last_X
                    && lines[i].Y1 == points[Index].Last_Y
                    && lines[i].X2 == point.X
                    && lines[i].Y2 == point.Y)
                {
                    points[Index].add = false;
                    break;
                }
                //Check to see if there is a line in the opposite Direction as travel
                else if (lines[i].X2 == points[Index].Last_X
                        && lines[i].Y2 == points[Index].Last_Y
                        && lines[i].X1 == point.X
                        && lines[i].Y1 == point.Y)
                {
                    points[Index].add = false;
                    break;
                }
                else if (i == lines.Count - 1)
                {
                    points[Index].add = true;
                }
            }
        }
        private void generate()
        {
            Random random = new Random();
            int change = random.Next(start_directions.Count - 1);
            points.Add(new point
            {
                Present_X = GX,
                Present_Y = GY,
                Last_X = GX,
                Last_Y = GY,
                Change_X = start_directions[change].Change_X,
                Change_Y = start_directions[change].Change_Y
            });
            Prevent_Overlap(points.Count -1);
        }
        ///////////////////////////////////////////
    }

    public class math
    {
        public static double Distance(double X1, double Y1, double X2, double Y2)
        {
            //This method uses the Distance formula to calculate the current distance of the drawing point
            //from the last turn.
            double distance = 0;
            double X = X2 - X1;
            double Y = Y2 - Y1;
            X *= X;
            Y *= Y;
            distance = X + Y;
            distance = Math.Sqrt(distance);
            return distance;
        }
        
        public static Point Next_Position(double X, double Y, double Change_X, double Change_Y, double Distance)
        {
            Point point = new Point();
            if (Change_Y == 0 && Change_X > 0)
            {
                point.X = Convert.ToInt32(X + 30);
                point.Y = Convert.ToInt32(Y);
            }
            else if (Change_Y == 0 && Change_X < 0)
            {
                point.X = Convert.ToInt32(X - 30);
                point.Y = Convert.ToInt32(Y);
            }
            else
            {
                double hypotineos = Distance * Distance;
                double Change = Math.Sqrt(hypotineos) / 2;
                if (Change_X > 0 && Change_Y > 0)
                {
                    point.X = Convert.ToInt32(X + Change);
                    point.Y = Convert.ToInt32(Y + Change);
                }
                else if (Change_X > 0 && Change_Y < 0)
                {
                    point.X = Convert.ToInt32(X + Change);
                    point.Y = Convert.ToInt32(Y - Change);
                }
                else if (Change_X < 0 && Change_Y > 0)
                {
                    point.X = Convert.ToInt32(X - Change);
                    point.Y = Convert.ToInt32(Y + Change);
                }
                else if (Change_X < 0 && Change_Y < 0)
                {
                    point.X = Convert.ToInt32(X - Change);
                    point.Y = Convert.ToInt32(Y - Change);
                }
            }
            return point;
        }
    }
    
    public class start
    {
        // Change in the X direction
        public float Change_X { get; set; }
        // Change in the Y Direction
        public float Change_Y { get; set; }

        public start()
        {

        }
    }
    
    public class point
    {
        //The Current Drawing Point
        public float Present_X { get; set; }
        public float Present_Y { get; set; }

        //The Last point at which it turned
        public float Last_X { get; set; }
        public float Last_Y { get; set; }

        //The direction and speed at which it is moving
        public float Change_X { get; set; }
        public float Change_Y { get; set; }

        //Whether or not the line generated will be added to the list
        public bool add { get; set; }

        public point()
        {
            add = true;
        }
    }

    public class line
    {
        // Point one
        public float X1 { get; set; }
        public float Y1 { get; set; }

        // Point Two
        public float X2 { get; set; }
        public float Y2 { get; set; }

        public line()
        {

        }
    }
}
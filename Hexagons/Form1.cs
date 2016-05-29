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
        public List<point> points = new List<point>() { };
        public List<start> start_directions = new List<start>()
        {
            //new start { Change_X = 2, Change_Y = 0},
            new start { Change_X = -2, Change_Y = 2},
            new start { Change_X = -2, Change_Y = -2},
            new start { Change_X = 2, Change_Y = 0},
        };
        public List<line> lines = new List<line>() { };
        public List<location> indexs = new List<location>() { };

        public bool Lines_ON = false;
        public bool Points_ON = true;
        public bool Run_Done = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if(Run_Done)
            {
                for (int i = 0; i < indexs.Count; i++)
                {
                    lines.RemoveAt(indexs[i].index);
                }
                indexs.Clear();
                Run_Done = false;
            }
            if (Lines_ON)
            {
                foreach (line var in lines)
                {
                    e.Graphics.DrawLine(new Pen(Color.Blue, 2), var.X1, var.Y1, var.X2, var.Y2);
                }
            }
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Present_X >= 0 &&
                    points[i].Present_X <= pictureBox1.Width &&
                    points[i].Present_Y >= 0 &&
                    points[i].Present_Y <= pictureBox1.Height)
                {
                    if (Lines_ON)
                    {
                        e.Graphics.DrawLine(new Pen(Color.Blue, 2), points[i].Last_X, points[i].Last_Y, points[i].Present_X, points[i].Present_Y);
                    }

                    if (Points_ON)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.OrangeRed), points[i].Present_X, points[i].Present_Y, 2F, 2F);
                    }
                    Check_Direction();
                }
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
            points.Clear();
            lines.Clear();
        }

        private void Refresh_Timer_Tick(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            foreach (point change in points)
            {
                change.Present_X += change.Change_X;
                change.Present_Y += change.Change_Y;
            }

            if (points.Count < 10000)
            {
                generate();
            }

        }

        private void Check_Timer_Tick(object sender, EventArgs e)
        {
            //Check_Overlap();
            Thread.MemoryBarrier();
            Thread.BeginCriticalRegion();
            Thread thread = new Thread(new ThreadStart(Check_Overlap));
            thread.Start();
        }

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

        private void Check_Direction()
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (math.Distance(points[i].Present_X, points[i].Present_Y, points[i].Last_X, points[i].Last_Y) >= 30)
                {
                    Random random = new Random();
                    double choice = random.NextDouble();
                    switch ((int)points[i].Change_Y)
                    {
                        case 2:
                            if (choice >= 0.5)
                            {
                                points[i].Change_Y -= 2;
                            }
                            else
                            {
                                points[i].Change_X *= -1;
                            }
                            break;
                        case 0:
                            if (choice >= 0.5)
                            {
                                points[i].Change_Y += 2;
                            }
                            else
                            {
                                points[i].Change_Y -= 2;
                            }
                            break;
                        case -2:
                            if (choice >= 0.5)
                            {
                                points[i].Change_Y += 2;
                            }
                            else
                            {
                                points[i].Change_X *= -1;
                            }
                            break;

                    }
                    lines.Add(new line { X1 = points[i].Last_X, X2 = points[i].Present_X, Y1 = points[i].Last_Y, Y2 = points[i].Present_Y });
                    points[i].Last_X = points[i].Present_X;
                    points[i].Last_Y = points[i].Present_Y;

                }
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
                        indexs.Add(new location { index = i2 });
                        /*lines.RemoveAt(i2);
                        i2--;*/
                    }
                    else if (i1 != i2 &&
                        lines[i1].X1 == lines[i2].X2 &&
                        lines[i1].Y1 == lines[i2].Y2 &&
                        lines[i1].X2 == lines[i2].X1 &&
                        lines[i1].Y2 == lines[i2].Y1)
                    {
                        indexs.Add(new location { index = i2 });
                        /*lines.RemoveAt(i2);
                        i2--;*/
                    }
                }
                if (i1 >= lines.Count / 2)
                {
                    break;
                }
            }
            if (indexs.Count < lines.Count)
            {
                for (int i1 = 0; i1 < indexs.Count; i1++)
                {
                    for (int i2 = 0; i2 < indexs.Count; i2++)
                    {
                        if (i1 != i2 && indexs[i1].index == indexs[i2].index)
                        {
                            indexs.RemoveAt(i2);
                            i2--;
                        }
                    }
                }
            }
            else
            {
                indexs.Clear();
            }
            Run_Done = true;
        }

        private void generate()
        {
            Random random = new Random();
            int change = random.Next(1, 3);
            points.Add(new point
            {
                Present_X = pictureBox1.Width >> 1,
                Present_Y = pictureBox1.Height >> 1,
                Last_X = pictureBox1.Width >> 1,
                Last_Y = pictureBox1.Height >> 1,
                Change_X = start_directions[change - 1].Change_X,
                Change_Y = start_directions[change - 1].Change_Y
            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }

    public class math
    {
        public static double Distance(double X1, double Y1, double X2, double Y2)
        {
            double distance = 0;
            double X = X2 - X1;
            double Y = Y2 - Y1;
            X *= X;
            Y *= Y;
            distance = X + Y;
            distance = Math.Sqrt(distance);
            return distance;
        }
    }
    
    public class start
    {
        public float Change_X { get; set; }
        public float Change_Y { get; set; }

        public start()
        {

        }
    }
    
    public class point
    {
        public float Present_X { get; set; }
        public float Present_Y { get; set; }

        public float Last_X { get; set; }
        public float Last_Y { get; set; }

        public float Change_X { get; set; }
        public float Change_Y { get; set; }

        public point()
        {

        }
    }

    public class line
    {
        public float X1 { get; set; }
        public float Y1 { get; set; }

        public float X2 { get; set; }
        public float Y2 { get; set; }

        public line()
        {

        }
    }

    public class location
    {
        public int index { get; set; }

        public location()
        {

        }
    }
}

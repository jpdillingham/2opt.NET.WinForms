using _2opt.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        #region Private Fields

        private static bool cancel = false;
        private static int iterationCount = 0;
        private static int maxUnproductiveIterations = 1000;
        private static List<System.Drawing.Point> origPoints;
        private static int pointCount = 5;
        private static int previousIntersectionCount = 0;
        private static Random rng = new Random();
        private static int unproductiveIterationCount = 0;
        private static int xLim = 10;
        private static int yLim = 10;
        private static List<Tuple<Line, Line>> IntersectingLines { get; set; }
        private static List<Line> Lines { get; set; }
        private static List<_2opt.NET.Point> Points { get; set; }

        #endregion Private Fields

        #region Public Constructors

        public Form1()
        {
            InitializeComponent();
            this.Paint += Form1_Paint;

            button2_Click(null, null);
        }

        #endregion Public Constructors

        #region Private Methods

        private void button1_Click(object sender, EventArgs e)
        {
            cancel = false;

            while (!cancel)
            {
                try
                {
                    Mutate();
                }
                catch
                {
                    break;
                }

                Refresh();

                Application.DoEvents();

                System.Threading.Thread.Sleep(10);
            }
        }

        private void DrawPoly(Graphics graphics, List<System.Drawing.Point> points)
        {
            graphics.FillPolygon(new SolidBrush(Color.LightBlue), points.ToArray());
            graphics.DrawPolygon(new Pen(Color.Gray), points.ToArray());

            foreach (System.Drawing.Point point in points)
            {
                graphics.FillEllipse(new SolidBrush(Color.Black), new Rectangle(point, new System.Drawing.Size(5, 5)));
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            List<System.Drawing.Point> points = new List<System.Drawing.Point>();

            foreach (_2opt.NET.Point point in Points)
            {
                points.Add(point.GetDrawingPoint());
            }

            DrawPoly(e.Graphics, points);

            if (origPoints != null)
            {
                DrawPoly(e.Graphics, origPoints);
            }
        }

        private void Mutate()
        {
            Lines = GetLinesFromPoints(Points);
            IntersectingLines = GetIntersectingLines(Lines);

            if (IntersectingLines.Count == 0)
            {
                throw new Exception();
            }

            Points = MutateIntersectingLines(Points, IntersectingLines);
            Refresh();
        }

        #endregion Private Methods

        private static List<Tuple<Line, Line>> GetIntersectingLines(List<Line> lines)
        {
            var intersectingLines = new List<Tuple<Line, Line>>();

            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = i + 1; j < lines.Count; j++)
                {
                    if (lines[i].Points[0] == lines[j].Points[0] || lines[i].Points[0] == lines[j].Points[1] || lines[i].Points[1] == lines[j].Points[0] || lines[i].Points[1] == lines[j].Points[1])
                    {
                        continue;
                    }

                    if (Utility.LineSegementsIntersect(Lines[i].Points[0], Lines[i].Points[1], Lines[j].Points[0], Lines[j].Points[1]))
                    {
                        var intersectingPair = new Tuple<Line, Line>(Lines[i], Lines[j]);
                        intersectingLines.Add(intersectingPair);
                    }
                }
            }

            return intersectingLines;
        }

        private static List<Line> GetLinesFromPoints(List<_2opt.NET.Point> points)
        {
            var lines = new List<Line>();

            for (int i = 0; i < points.Count; i++)
            {
                var line = new Line()
                {
                    Points = new[] { points[i], points[i + 1 == points.Count ? 0 : i + 1] }
                };

                lines.Add(line);
            }

            return lines;
        }

        private static List<_2opt.NET.Point> GetRandomizedPoints(int count, int xlim, int ylim)
        {
            var points = new List<_2opt.NET.Point>();

            while (points.Count < count)
            {
                var point = new _2opt.NET.Point(rng.Next(xlim), rng.Next(ylim));

                if (!points.Contains(point))
                {
                    points.Add(point);
                }
            }

            return points;
        }

        private static List<_2opt.NET.Point> MutateIntersectingLines(List<_2opt.NET.Point> points, List<Tuple<Line, Line>> intersectingLines)
        {
            var pairIndex = rng.Next(intersectingLines.Count);

            Tuple<Line, Line> pair = intersectingLines[pairIndex];

            Debug.WriteLine($"Intersecting pair: {pair.Item1} {pair.Item2}");

            Line line1;
            Line line2;

            if (rng.Next(1) == 1)
            {
                line1 = pair.Item1;
                line2 = pair.Item2;
            }
            else
            {
                line1 = pair.Item2;
                line2 = pair.Item1;
            }

            _2opt.NET.Point left = line1.Points[0];
            _2opt.NET.Point right = line2.Points[1];

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i] == left)
                {
                    points[i] = right;
                }
                else if (points[i] == right)
                {
                    points[i] = left;
                }
            }

            return points;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Points = GetRandomizedPoints(Int32.Parse(textBox1.Text), 300, 300);

            origPoints = new List<System.Drawing.Point>();

            foreach (_2opt.NET.Point point in Points)
            {
                origPoints.Add(new System.Drawing.Point(point.X + 400, point.Y));
            }

            Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cancel = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Mutate();
            Refresh();
        }
    }
}
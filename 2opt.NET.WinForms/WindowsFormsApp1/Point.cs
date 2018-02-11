using System;
using System.Collections.Generic;
using System.Text;

namespace _2opt.NET
{
    public class Point
    {
        #region Public Fields

        public int X;
        public int Y;

        #endregion Public Fields

        #region Public Constructors

        public Point(int x, int y)
        {
            X = x; Y = y;
        }

        #endregion Public Constructors

        #region Public Methods

        public static Point operator -(Point v, Point w)
        {
            return new Point(v.X - w.X, v.Y - w.Y);
        }

        public static int operator *(Point v, Point w)
        {
            return v.X * w.X + v.Y * w.Y;
        }

        public static Point operator *(Point v, int mult)
        {
            return new Point(v.X * mult, v.Y * mult);
        }

        public static Point operator *(int mult, Point v)
        {
            return new Point(v.X * mult, v.Y * mult);
        }

        public static Point operator +(Point v, Point w)
        {
            return new Point(v.X + w.X, v.Y + w.Y);
        }

        public double Cross(Point v)
        {
            return X * v.Y - Y * v.X;
        }

        public override bool Equals(object obj)
        {
            var v = (Point)obj;
            return (X - v.X).IsZero() && (Y - v.Y).IsZero();
        }

        public System.Drawing.Point GetDrawingPoint()
        {
            return new System.Drawing.Point(X, Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        #endregion Public Methods
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace _2opt.NET
{
    public static class Utility
    {
        #region Private Fields

        private const double Epsilon = 1e-10;

        #endregion Private Fields

        #region Public Methods

        public static string GetSQL(List<Point> points)
        {
            string sql = "SELECT geometry::STGeomFromText('POLYGON((";

            foreach (Point point in points)
            {
                sql += $"{point.X} {point.Y}, ";
            }

            sql += $"{points[0].X} {points[0].Y}))', 4326)";

            return sql;
        }

        public static bool IsZero(this double d)
        {
            return Math.Abs(d) < Epsilon;
        }

        public static bool IsZero(this int d)
        {
            return Math.Abs(d) < Epsilon;
        }

        /// <summary>
        ///     Test whether two line segments intersect. If so, calculate the intersection point. <see cref="http://stackoverflow.com/a/14143738/292237"/>
        /// </summary>
        /// <param name="p">Vector to the start point of p.</param>
        /// <param name="p2">Vector to the end point of p.</param>
        /// <param name="q">Vector to the start point of q.</param>
        /// <param name="q2">Vector to the end point of q.</param>
        /// <param name="intersection">The point of intersection, if any.</param>
        /// <param name="considerOverlapAsIntersect">Do we consider overlapping lines as intersecting?</param>
        /// <returns>True if an intersection point was found.</returns>
        public static bool LineSegementsIntersect(Point p, Point p2, Point q, Point q2,
            bool considerCollinearOverlapAsIntersect = false)
        {
            var r = p2 - p;
            var s = q2 - q;
            var rxs = r.Cross(s);
            var qpxr = (q - p).Cross(r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (rxs.IsZero() && qpxr.IsZero())
            {
                // 1. If either 0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s then the two lines are overlapping,
                if (considerCollinearOverlapAsIntersect)
                    if ((0 <= (q - p) * r && (q - p) * r <= r * r) || (0 <= (p - q) * s && (p - q) * s <= s * s))
                        return true;

                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s then the two lines are collinear but
                // disjoint. No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (rxs.IsZero() && !qpxr.IsZero())
                return false;

            // t = (q - p) x s / (r x s)
            var t = (q - p).Cross(s) / rxs;

            // u = (q - p) x r / (r x s)

            var u = (q - p).Cross(r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1 the two line segments meet at the point p + t r = q + u s.
            if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }

        public static void ParseArg(string[] args, int index, out int target, string targetName, int def)
        {
            try
            {
                target = Int32.Parse(args[index]);
            }
            catch
            {
                Console.WriteLine($"WARN: invalid parameter for {targetName}; defaulting to {def}.");
                target = def;
            }
        }

        #endregion Public Methods
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.PropertyGridInternal;

namespace INFOIBV
{
    public class LineSegment
    {
        /// <summary>
        /// The first point of this line segment
        /// </summary>
        public Point Point1 { get; }

        /// <summary>
        /// The second point of this line segment
        /// </summary>
        public Point Point2 { get; }

        /// <summary>
        /// The r value of this line segment
        /// </summary>
        public double R { get; }

        /// <summary>
        /// The theta value of this line segment
        /// </summary>
        public double Theta { get; }
        
        /// <summary>
        /// The length of the line
        /// </summary>
        public double Length { get; }

        /// <summary>
        /// Create a line segment
        /// </summary>
        /// <param name="point1"> The first point of this line segment</param>
        /// <param name="point2"> The second point of this line segment</param>
        /// <param name="r"> The r value of this line segment</param>
        /// <param name="theta"> The theta value of this line segment</param>
        public LineSegment(Point point1, Point point2, double r, double theta)
        {
            if (point1.Y < point2.Y || point1.Y == point1.Y && point1.X < point2.X) // Make sure the top or left most point is always Point1
            {
                Point1 = point1;
                Point2 = point2;
            }
            else
            {
                Point1 = point2;
                Point2 = point1;
            }
            R = r;
            Theta = theta;
            Length = Math.Sqrt(Math.Pow((point2.Y - point1.Y), 2) + Math.Pow((point2.X - point1.X), 2));
        }

        /// <summary>
        /// Checks whether this line segment is similar to the given line segment, optionally within the specified margins
        /// </summary>
        /// <param name="otherLine"> The LineSegment for comparison</param>
        /// <param name="xMargin"> The margin for the x values of the points</param>
        /// <param name="yMargin"> The margin for the y values of the points</param>
        /// <param name="rMargin"> The margin for the r values</param>
        /// <param name="thetaMargin"> The margin for the theta values</param>
        /// <returns> true if the line segments are similar, optionally within the specified margins</returns>
        public bool IsSimilarTo(LineSegment otherLine, int xMargin = 2, int yMargin = 10, double rMargin = 10d,
            double thetaMargin = 10d)
        {
            // bool denoting whether the point1s are similar
            bool p1Similar = Math.Abs(this.Point1.X - otherLine.Point1.X) < xMargin
                             && Math.Abs(this.Point1.Y - otherLine.Point1.Y) < yMargin;
            
            // bool denoting whether the point2s are similar
            bool p2Similar = Math.Abs(this.Point2.X - otherLine.Point2.X) < xMargin
                             && Math.Abs(this.Point2.Y - otherLine.Point2.Y) < yMargin;
            
            // bool denoting whether the theta values are similar
            bool thetaSimilar = Math.Abs(this.Theta - otherLine.Theta) < thetaMargin
                                || Math.Abs(Math.Abs(this.Theta - 720d) - otherLine.Theta) < thetaMargin; //accounting for the 180degree wraparound, favoring the values closer to 0
                                
            // bool denoting whether the r values are similar
            bool rSimilar()
            {
                if (this.R > 0 && otherLine.R > 0)
                    return Math.Abs(this.R - otherLine.R) < rMargin;
                if (this.R < 0 && otherLine.R < 0)
                    return Math.Abs(-this.R + otherLine.R) < rMargin;
                if (this.R < 0 && otherLine.R > 0)
                    return this.R > -(rMargin / 2)
                           && otherLine.R < rMargin / 2;
                if (this.R > 0 && otherLine.R < 0)
                    return this.R < rMargin / 2
                           && otherLine.R > -(rMargin / 2);
                return false;
            }

            // return true if either both points are similar or if the rTheta pair is similar
            return p1Similar && p2Similar || thetaSimilar && rSimilar();
        }

    }
}
using System;
using System.Drawing;

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
        /// Create a line segment
        /// </summary>
        /// <param name="point1"> The first point of this line segment</param>
        /// <param name="point2"> The second point of this line segment</param>
        /// <param name="r"> The r value of this line segment</param>
        /// <param name="theta"> The theta value of this line segment</param>
        public LineSegment(Point point1, Point point2, double r, double theta)
        {
            Point1 = point1;
            Point2 = point2;
            R = r;
            Theta = theta;
        }

        /// <summary>
        /// Checks whether this line segment is similar to the given line segment, within the specified margins
        /// </summary>
        /// <param name="otherLine"> The LineSegment for comparison</param>
        /// <param name="rMargin"> The margin for the r values</param>
        /// <param name="thetaMargin"> The margin for the theta values</param>
        /// <returns> true if the line segments are similar within the specified margins</returns>
        public bool IsSimilarTo(LineSegment otherLine, double rMargin, double thetaMargin)
        {
            return Math.Abs(this.Theta - otherLine.Theta) < thetaMargin
                   && Math.Abs(this.R - otherLine.R) < rMargin;
        }

    }
}
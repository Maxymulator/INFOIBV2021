using System;
using System.Drawing;

namespace INFOIBV
{
    public class Circle
    {
        /// <summary>
        /// The center point of the circle
        /// </summary>
        public Point Center { get; }
        
        /// <summary>
        /// The radius of the circle
        /// </summary>
        public double Radius { get; }
        
        /// <summary>
        /// Initialise the circle
        /// </summary>
        /// <param name="center"> The center point of the circle</param>
        /// <param name="radius"> The radius of the circle</param>
        public Circle(Point center, double radius)
        {
            this.Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Check if the given point is on this circle, respecting a given margin
        /// </summary>
        /// <param name="p"> The point to check</param>
        /// <param name="margin"> The distance the point can be removed from the circle while still being counted as being on the circle</param>
        /// <returns> True if the given point is on the circle, respecting the given margin </returns>
        public bool isPointOnCircle(Point p, double margin)
        {
            return Math.Abs(
                Math.Sqrt(Math.Pow(p.X - Center.X, 2) + Math.Pow(p.Y - Center.Y, 2)) 
                - Radius) < margin;
        }

    }
}
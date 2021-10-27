﻿using System;

namespace INFOIBV
{
    public class HPGlasses
    {
        /// <summary>
        /// The first circle of the HPGlasses
        /// </summary>
        public Circle Circle1 { get; }

        /// <summary>
        /// The second circle of the HPGlasses
        /// </summary>
        public Circle Circle2 { get; }

        /// <summary>
        /// The line segment connecting the two circles
        /// </summary>
        public LineSegment NoseBridge { get; }

        /// <summary>
        /// The first ear piece of the glasses
        /// </summary>
        public LineSegment EarPiece1 { get; }

        /// <summary>
        /// The second ear piece of the glasses
        /// </summary>
        public LineSegment EarPiece2 { get; }

        /// <summary>
        /// Initialise this pair of HP Glasses
        /// </summary>
        /// <param name="circle1"> The first circle of the HPGlasses</param>
        /// <param name="circle2"> The second circle of the HPGlasses</param>
        /// <param name="noseBridge"> The line segment connecting the two circles</param>
        /// <param name="earPiece1"> The first ear piece of the glasses</param>
        /// <param name="earPiece2"> The second ear piece of the glasses</param>
        public HPGlasses(Circle circle1 = null, Circle circle2 = null, LineSegment noseBridge = null, LineSegment earPiece1 = null,
            LineSegment earPiece2 = null)
        {
            Circle1 = circle1;
            Circle2 = circle2;
            NoseBridge = noseBridge;
            EarPiece1 = earPiece1;
            EarPiece2 = earPiece2;
        }

        /// <summary>
        /// Check if the line segment is parallel to the line between the centers of the circles
        /// </summary>
        public bool LineSegmentIsParallelToCircles()
        {
            double c1X = Circle1.Center.X;
            double c1Y = Circle1.Center.Y;
            double c2X = Circle2.Center.X;
            double c2Y = Circle2.Center.Y;
            double slopeCircleLine = (c2Y - c1Y) / (c2X - c1X);
            
            double ls1X = NoseBridge.Point1.X;
            double ls1Y = NoseBridge.Point1.Y;
            double ls2X = NoseBridge.Point2.X;
            double ls2Y = NoseBridge.Point2.Y;
            double slopeNoseBridge = (ls2Y - ls1Y) / (ls2X - ls1X);

            double margin = 0.5d;
            
            if (slopeCircleLine > 0 && slopeNoseBridge > 0)
                return Math.Abs(slopeCircleLine - slopeNoseBridge) < margin;
            if (slopeCircleLine < 0 && slopeNoseBridge < 0)
                return Math.Abs(-slopeCircleLine + slopeNoseBridge) < margin;
            if (slopeCircleLine < 0 && slopeNoseBridge > 0)
                return slopeCircleLine > -(margin / 2)
                       && slopeNoseBridge < margin / 2;
            if (slopeCircleLine > 0 && slopeNoseBridge < 0)
                return slopeCircleLine < margin / 2
                       && slopeNoseBridge > -(margin / 2);
            return false;
        }

        /// <summary>
        /// Get the certainty percentage of this set of HP Glasses
        /// </summary>
        /// <returns> A value between 0 - 100 depicting how sure the program is that these are HP Glasses</returns>
        public int GetCertainty()
        {
            int percentage = 0;
            if (Circle1 is not null)
                percentage += 10;
            if (Circle2 is not null)
                percentage += 10;
            if (NoseBridge is not null)
            {
                percentage += 20;
                if (LineSegmentIsParallelToCircles())
                    percentage += 30;
            }
            if (EarPiece1 is not null)
                percentage += 15;
            if (EarPiece2 is not null)
                percentage += 15;

            return percentage;
        }

    }
}
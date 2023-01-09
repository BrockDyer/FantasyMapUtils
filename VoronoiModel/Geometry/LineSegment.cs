﻿using System;
using Microsoft.Maui.Graphics;

namespace VoronoiModel.Geometry
{
    /// <summary>
    /// Represents a straight line segment between two points.
    /// </summary>
	public class LineSegment : ISegment
	{
		readonly Point start;
		readonly Point end;

        /// <summary>
        /// Construct a new line segment from two
        /// <see cref="Point">Point Vectors</see>.
        /// </summary>
        /// <param name="start">The starting point vector.</param>
        /// <param name="end">The ending point vector.</param>
        public LineSegment(Point start, Point end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Construct a new line segment from several decimal 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public LineSegment(Decimal x1, Decimal y1, Decimal x2, Decimal y2)
            : this(new Point(x1, y1), new Point(x2, y2)) { }

        //public void DrawSegment(ICanvas canvas)
        //{
        //    var point1 = new PointF((float)start.GetX(), (float)start.GetY());
        //    var point2 = new PointF((float)end.GetX(), (float)end.GetY());
        //    canvas.DrawLine(point1, point2);
        //}
    }
}


﻿using System;
using Microsoft.Maui.Graphics;
using VoronoiModel;
using VoronoiModel.Geometry;
using VoronoiModel.PlanarSubdivision;

namespace VoronoiModel.Services
{
	public interface IVoronoiService
	{
		/// <summary>
		/// Add a point to the voronoi model.
		/// </summary>
		/// <param name="point">The point to add.</param>
        public void AddPoint(VoronoiPoint point);

		/// <summary>
		/// Get a list of the voronoi points in the model.
		/// </summary>
		public List<VoronoiPoint> GetPoints();

		/// <summary>
		/// Initialize the points using a uniformly random distribution.
		/// </summary>
		/// <param name="minX">The minimum x value of the search space.</param>
		/// <param name="minY">The minimum y value of the search space.</param>
		/// <param name="maxX">The maximum x value of the search space.</param>
		/// <param name="maxY">The maximum y value of the search space.</param>
        public void InitPoints(double minX, double minY, double maxX,
			double maxY);

		/// <summary>
		/// Initialize the points given a list of points.
		/// </summary>
		/// <param name="points">The list of points.</param>
		public void InitPoints(params Point2D[] points);

		public void InitBounds(double minX, double minY, double maxX, double maxY);

        /// <summary>
        /// Compute the voronoi cells for each point.
        /// </summary>
        public void ComputeVoronoi();

		/// <summary>
		/// Generate a uniformly random saple of points within the voronoi cell
		/// of the given centroid.
		/// </summary>
		/// <param name="point">A centroid in the voronoi model.</param>
		/// <param name="sampleSize">The number of points to sample.</param>
		/// <returns></returns>
		public List<Point2D> SamplePointsWithinCell(Point2D point, int sampleSize);

		/// <summary>
		/// Score the voronoi model according to a fitness function.
		/// </summary>
		/// <param name="vector">The vector to score.</param>
		/// <returns>The score of the model.</returns>
		public double Score(List<double> vector);

		/// <summary>
		/// For debuggin purposes.
		/// <br></br>
		/// TODO: Remove.
		/// </summary>
		public void PrintPoints();

		/// <summary>
		/// Visualize the Voronoi Model.
		/// </summary>
		/// <param name="canvas">The canvas to draw to.</param>
		public void Visualize(ICanvas canvas);
	}
}


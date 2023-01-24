using System.Diagnostics;
using Microsoft.Maui.Graphics;
using VoronoiModel.FortuneVoronoi;
using VoronoiModel.PlanarSubdivision;
using VoronoiModel.Services;
using VoronoiModel.Geometry;

namespace VoronoiModel
{
	public class VoronoiService : IVoronoiService
	{
        private readonly List<VoronoiPoint> _points = new();
        private readonly Random _random = new();

        private Dcel? _dcel;

        public void AddPoint(VoronoiPoint point)
        {
            _points.Add(point);
            Debug.WriteLine($"Added point {point}");
        }

        public void ComputeVoronoi()
        {
            throw new NotImplementedException();
        }

        public List<VoronoiPoint> GetPoints()
        {
            return _points;
        }

        public void InitPoints(double minX, double minY, double maxX, double maxY)
        {
            foreach (var point in _points)
            {
                point.X = _random.NextDouble() * maxX + minX;
                point.Y = _random.NextDouble() * maxY + minY;
            }

            _dcel = Algorithm.ComputeVoronoi(new Point2D(minX, minY), new Point2D(maxX, maxY), 
                _points.Select(p => new Point2D(p.X.GetValueOrDefault(0), p.Y.GetValueOrDefault(0))));
        }

        public void PrintPoints()
        {
            foreach (var p in _points)
            {
                Debug.WriteLine(p);
            }
        }

        public List<Tuple<double, double>> SamplePointsWithinCell(VoronoiPoint point, int sampleSize)
        {
            throw new NotImplementedException();
        }

        public void Visualize(ICanvas canvas)
        {
            _dcel?.Visualize(canvas);
        }
    }
}


using System;
using System.Diagnostics;
using VoronoiModel.Services;

namespace VoronoiModel
{
	public class VoronoiService : IVoronoiService
	{
        private readonly List<VoronoiPoint> points = new List<VoronoiPoint>();
        private Random random = new Random();

        public void AddPoint(VoronoiPoint point)
        {
            points.Add(point);
            Debug.WriteLine(string.Format("Added point {0}", point));
        }

        public void ComputeVoronoi()
        {
            throw new NotImplementedException();
        }

        public List<VoronoiPoint> GetPoints()
        {
            return points;
        }

        public void InitPoints(decimal minX, decimal minY, decimal maxX, decimal maxY)
        {
            foreach (var point in points)
            {
                point.X = (decimal)random.NextDouble() * maxX + minX;
                point.Y = (decimal)random.NextDouble() * maxY + minY;
            }
        }

        public void PrintPoints()
        {
            foreach (var p in points)
            {
                Debug.WriteLine(p);
            }
        }

        public List<Tuple<decimal, decimal>> SamplePointsWithinCell(VoronoiPoint point, int sampleSize)
        {
            throw new NotImplementedException();
        }
    }
}


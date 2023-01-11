using System;
namespace VoronoiModel.Geometry
{
	public class Vector
	{
		private readonly List<double> elements;

		public int Size { get; }

		public Vector(params double[] elements) : this(elements.ToList()) { }

		public Vector(List<double> elements)
		{
			this.elements = elements;
			Size = elements.Count;
		}

		public Vector(Point2D p, Point2D q) : this(q.X - p.X, q.Y - p.Y) { }

		/// <summary>
		/// Get an element of the vector.
		/// </summary>
		/// <param name="i">The index to get.</param>
		/// <returns>The element at the index.</returns>
		public double Get(int i)
		{
			return elements[i];
		}

		public double Dot(Vector other)
		{
			EnforceSameSize(other);
			var sum = 0d;
			for (int i = 0; i < Size; i++)
			{
				sum += (elements[i] * other.elements[i]);
			}
			return sum;
		}

		public Vector Cross2D(Vector other)
		{
			if (Size != 2)
				throw new InvalidOperationException(string.Format("{0} is only supported for 2D vectors.", nameof(Cross2D)));
			EnforceSameSize(other);

			var x1 = elements[0];
			var y1 = elements[1];
			var z1 = 0d;

			var x2 = other.elements[0];
			var y2 = other.elements[1];
			var z2 = 0d;

			var rx = (y1 * z2) - (z1 * y2);
			var ry = (z1 * x2) - (x1 * z2);
			var rz = (x1 * y2) - (y1 * x2);

			return new Vector(rx, ry, rz);
		}

		public Vector Add(Vector other)
		{
			EnforceSameSize(other);
			var result = new List<double>();
			for (int i = 0; i < Size; i++)
			{
				result.Add(elements[i] + other.elements[i]);
			}
			return new Vector(result);
		}

		public Vector Subtract(Vector other)
		{
            EnforceSameSize(other);
            var result = new List<double>();
            for (int i = 0; i < Size; i++)
            {
                result.Add(elements[i] - other.elements[i]);
            }
            return new Vector(result);
        }

		public double Magnitude()
		{
			var squaredSum = 0d;
			foreach(var elem in elements)
			{
				squaredSum += elem * elem;
			}
			return (double)(Math.Sqrt((double)squaredSum));
		}

		public Vector Normalize()
		{
			var result = new List<double>();
			var magnitude = Magnitude();
			foreach(var elem in elements)
			{
				result.Add(elem / magnitude);
			}
			return new Vector(result);
		}

		private void EnforceSameSize(Vector other)
		{
            if (Size != other.Size) throw new InvalidOperationException(
				"Cannot perform operation on different length vectors.");
        }

		public bool IsZero()
		{
			foreach(var elem in elements)
			{
				if(!Utils.AreClose(elem, 0))
				{
					return false;
				}
			}

			return true;
		}
	}
}


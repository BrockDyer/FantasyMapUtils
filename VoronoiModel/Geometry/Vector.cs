namespace VoronoiModel.Geometry
{
	public class Vector
	{
		private readonly List<double> _elements;

		private int Size { get; }

		public Vector(params double[] elements) : this(elements.ToList()) { }

		private Vector(List<double> elements)
		{
			this._elements = elements;
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
			return _elements[i];
		}

		public double Dot(Vector other)
		{
			EnforceSameSize(other);
			var sum = 0d;
			for (int i = 0; i < Size; i++)
			{
				sum += (_elements[i] * other._elements[i]);
			}
			return sum;
		}

		public Vector Cross2D(Vector other)
		{
			if (Size != 2)
				throw new InvalidOperationException($"{nameof(Cross2D)} is only supported for 2D vectors.");
			EnforceSameSize(other);

			var x1 = _elements[0];
			var y1 = _elements[1];
			const double z1 = 0d;

			var x2 = other._elements[0];
			var y2 = other._elements[1];
			const double z2 = 0d;

			var rx = (y1 * z2) - (z1 * y2);
			var ry = (z1 * x2) - (x1 * z2);
			var rz = (x1 * y2) - (y1 * x2);

			return new Vector(rx, ry, rz);
		}

		public Vector Add(Vector other)
		{
			EnforceSameSize(other);
			var result = new List<double>();
			for (var i = 0; i < Size; i++)
			{
				result.Add(_elements[i] + other._elements[i]);
			}
			return new Vector(result);
		}

		public Vector Subtract(Vector other)
		{
            EnforceSameSize(other);
            var result = new List<double>();
            for (var i = 0; i < Size; i++)
            {
                result.Add(_elements[i] - other._elements[i]);
            }
            return new Vector(result);
        }

		public double Magnitude()
		{
			var squaredSum = _elements.Sum(elem => elem * elem);
			return Math.Sqrt(squaredSum);
		}

		public Vector Normalize()
		{
			var magnitude = Magnitude();
			var result = _elements.Select(elem => elem / magnitude).ToList();
			return new Vector(result);
		}

		private void EnforceSameSize(Vector other)
		{
            if (Size != other.Size) throw new InvalidOperationException(
				"Cannot perform operation on different length vectors.");
        }

		public bool IsZero()
		{
			return _elements.All(elem => Utils.AreClose(elem, 0));
		}
	}
}


using System;
namespace VoronoiModel.Geometry
{
    /// <summary>
    /// A 2D or 3D point (determined by number of elements in xyz).
    /// </summary>
	public class Point
	{
		readonly List<Decimal> xyz;
        /// <summary>
        /// The traditional X value of the vector. This is the first element in
        /// the vector.
        /// </summary>
        /// <returns>The first value in the vector.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public Decimal X { get => xyz[0]; }
        /// <summary>
        /// The traditional Y value of the vector. This is the second element in
        /// the vector.
        /// </summary>
        /// <returns>The second element in the vector.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public Decimal Y { get => xyz[1]; }
        /// <summary>
		/// The traditional Z value of the vector. This is the third argument in
		/// the vector.
		/// </summary>
		/// <returns>The third element in the vector.</returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public Decimal Z { get => xyz[2]; }

		public Point(params Decimal[] xyz)
		{
			this.xyz = xyz.ToList();
		}

        public override bool Equals(object? obj)
        {
            if (obj is Point p2)
            {
                if (p2.xyz.Count != xyz.Count) return false;
                for (var i = 0; i < xyz.Count; i++)
                {
                    if (!p2.xyz[i].Equals(xyz[i])) return false;
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var i in xyz)
            {
                hash *= 33;
                hash += i.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            if (xyz.Count >= 3)
            {
                return string.Format("({0}, {1}, {2})", X, Y, Z);
            } else
            {
                return string.Format("({0}, {1})", X, Y);
            }
        }
    }
}


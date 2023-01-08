using System;
namespace VoronoiModel.Geometry
{
	public class Vector
	{
		readonly List<Decimal> X;

		public Vector(params Decimal[] xs)
		{
			X = xs.ToList();
		}

		/// <summary>
		/// The traditional X value of the vector. This is the first element in
		/// the vector.
		/// </summary>
		/// <returns>The first value in the vector.</returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public Decimal GetX()
		{
			return X[0];
		}

		/// <summary>
		/// The traditional Y value of the vector. This is the second element in
		/// the vector.
		/// </summary>
		/// <returns>The second element in the vector.</returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public Decimal GetY()
		{
			return X[1];
		}

		/// <summary>
		/// The traditional Z value of the vector. This is the third argument in
		/// the vector.
		/// </summary>
		/// <returns>The third element in the vector.</returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public Decimal GetZ()
		{
			return X[2];
		}

		/// <summary>
		/// Get the ith element of the vector.
		/// </summary>
		/// <param name="i">The index of the vector to get.</param>
		/// <returns>The ith element of the vector.</returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public Decimal Get(int i)
		{
			return X[i];
		}


        public override bool Equals(object? obj)
        {
            return obj is Vector vector &&
                   EqualityComparer<List<decimal>>.Default.Equals(X, vector.X);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X);
        }
    }
}


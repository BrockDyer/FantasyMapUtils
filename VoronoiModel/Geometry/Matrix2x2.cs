using System;
namespace VoronoiModel.Geometry
{
    /// <summary>
    /// | a b |<br></br>
    /// | c d |
    /// </summary>
	public class Matrix2x2
	{
		double A { get; }
		double B { get; }
		double C { get; }
		double D { get; }

        public Matrix2x2(double a, double b, double c, double d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        /// <summary>
        /// Compute the determinant of this matrix.
        /// </summary>
        /// <returns></returns>
        public double Det()
        {
            return (A * D) - (B * C);
        }

        /// <summary>
        /// Construct the adjoint of this matrix.
        /// </summary>
        /// <returns>The adjoint matrix.</returns>
        public Matrix2x2 Adjoint()
        {
            return new Matrix2x2(D, -1 * B, -1 * C, A);
        }

        /// <summary>
        /// Add another matrix to this one.
        /// </summary>
        /// <param name="other">Another matrix.</param>
        /// <returns>The resulting sum.</returns>
        public Matrix2x2 Add(Matrix2x2 other)
        {
            return new Matrix2x2(A + other.A, B + other.B, C + other.C, D + other.D);
        }

        /// <summary>
        /// Add a scalar constant to this matrix.
        /// </summary>
        /// <param name="scalar">The value to add.</param>
        /// <returns>The resulting sum.</returns>
        public Matrix2x2 Add(double scalar)
        {
            return new Matrix2x2(A + scalar, B + scalar, C + scalar, D + scalar);
        }

        /// <summary>
        /// Multiply this matrix by another and return the result.
        /// </summary>
        /// <param name="other">The other matrix.</param>
        /// <returns>The resulting product.</returns>
        public Matrix2x2 Multiply(Matrix2x2 other)
        {
            var aPrime = (A * other.A) + (B * other.C);
            var bPrime = (A * other.B) + (B * other.D);
            var cPrime = (C * other.A) + (D * other.C);
            var dPrime = (C * other.B) + (D * other.D);
            return new Matrix2x2(aPrime, bPrime, cPrime, dPrime);
        }

        /// <summary>
        /// Multiple this matrix by a scalar.
        /// </summary>
        /// <param name="scalar">The value to multiply by.</param>
        /// <returns>The resulting product.</returns>
        public Matrix2x2 Multiply(double scalar)
        {
            return new Matrix2x2(A * scalar, B * scalar, C * scalar, D * scalar);
        }

        /// <summary>
        /// Multiply this vector by another vector.
        /// </summary>
        /// <param name="v">The vector to multiply. Must be a vector in R2.</param>
        /// <returns>The resultant vector.</returns>
        public Vector Multiply(Vector v)
        {
            Vector ab = new Vector(A, B);
            Vector cd = new Vector(C, D);

            return new Vector(ab.Dot(v), cd.Dot(v));
        }

        /// <summary>
        /// Compute the inverse of this matrix if it exists.
        /// </summary>
        /// <returns>The inverse matrix. Or null if it does not exist.</returns>
        public Matrix2x2? Inverse()
        {
            var det = Det();

            // Inverse does not exist if determinant is 0.
            if (Utils.AreClose(det, 0)) return null;

            return Adjoint().Multiply(1 / det);
        }
    }
}


namespace OptimizationLib;

/// <summary>
/// An interface that defines the optimization routine.
/// </summary>
public interface IOptimizationAlgorithm
{

    /// <summary>
    /// Perform the optimization.
    /// </summary>
    /// <param name="random">The RNG.</param>
    /// <param name="minBounds">A vector (list) containing the minimum values for each dimension.</param>
    /// <param name="maxBounds">A vector (list) containing the maximum values for each dimension.</param>
    /// <param name="score">A function delegate that computes the score of a solution vector.</param>
    /// <returns>The solution that was found.</returns>
    List<double> Solve(Random random, List<double> minBounds, List<double> maxBounds, Score score);

    /// <summary>
    /// Generate an initial vector using the supplied random number generator and bounded by the specified bounds.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="minBounds">The vector of minimum bounds.</param>
    /// <param name="maxBounds">The vector of maximum bounds.</param>
    /// <returns>The generated vector as a list.</returns>
    public static List<double> GenerateInitial(Random random, List<double> minBounds, List<double> maxBounds)
    {
        var dim = minBounds.Count;
        var result = new List<double>();
        for (var i = 0; i < dim; i += 1)
        {
            var minValue = minBounds[i];
            var maxValue = maxBounds[i];
            var differenceHigh = maxValue - minValue;
            var differenceLow = minValue - 0;
            var value = random.NextDouble() * differenceHigh + differenceLow;
            result.Add(value);
        }

        return result;
    }

    /// <summary>
    /// Clip the vector between the minimum and maximum bounds. This modifies the vector in place.
    /// </summary>
    /// <param name="minBounds">The minimum bound for each dimension.</param>
    /// <param name="maxBounds">The maximum bound for each dimension.</param>
    /// <param name="vector">The vector to clip.</param>
    public static void Clip(List<double> minBounds, List<double> maxBounds, List<double> vector)
    {
        for (var i = 0; i < vector.Count; i++)
        {
            var min = minBounds[i];
            var max = maxBounds[i];
            var value = vector[i];
            value = value < min ? min : value;
            value = value > max ? max : value;
            vector[i] = value;
        }
    }
}
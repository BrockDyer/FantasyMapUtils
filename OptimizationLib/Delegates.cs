namespace OptimizationLib;

/// <summary>
/// Function delegate to compute a fitness/score value for a given solution vector.
/// </summary>
public delegate double Score(List<double> value);
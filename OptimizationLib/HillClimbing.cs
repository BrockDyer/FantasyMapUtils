namespace OptimizationLib;

public class HillClimbing : IOptimizationAlgorithm
{
    private const string IterKey = "max_iterations";
    private const string StepSizeKey = "step_size";
    private const string AccelerationKey = "acceleration";

    private readonly OptimizationParameters _hyperParameters;

    public HillClimbing(int maxIterations, double stepSize, double acceleration)
    {
        _hyperParameters = new OptimizationParameters();
        _hyperParameters.SetParameter(IterKey, maxIterations);
        _hyperParameters.SetParameter(StepSizeKey, stepSize);
        _hyperParameters.SetParameter(AccelerationKey, acceleration);
    }
    
    public List<double> Solve(Random random, List<double> minBounds, List<double> maxBounds, Score score)
    {
        var maxIterations = _hyperParameters.GetParameter<int>("max_iterations");
        var stepSize = _hyperParameters.GetParameter<double>("step_size");
        var acceleration = _hyperParameters.GetParameter<double>("acceleration");
        
        var solution = IOptimizationAlgorithm.GenerateInitial(random, minBounds, maxBounds);
        var candidates = new[]
        {
            -acceleration,
            acceleration,
            -1 / acceleration,
            1 / acceleration
        };

        var iteration = 0;
        var bestScore = score(solution);
        while (true)
        {

            for (var i = 0; i < solution.Count; i++)
            {
                var current = solution[i];
                var candidate = new List<double>(solution);
                var min = minBounds[i];
                var max = maxBounds[i];
                
                foreach (var direction in candidates)
                {
                    var step = stepSize * direction;
                    var value = current + step;
                    value = value < min ? min : value;
                    value = value > max ? max : value;
                    candidate[i] = value;

                    var candidateScore = score(candidate);
                    var solutionScore = score(solution);
                    if (candidateScore > solutionScore)
                    {
                        solution = candidate;
                    }
                }
            }

            // We have converged to a local optima
            if (Math.Abs(score(solution) - bestScore) < 0.00001 || iteration > maxIterations)
            {
                Console.WriteLine($"Solution found after {iteration} iterations.");
                return solution;
            }

            bestScore = score(solution);
            iteration += 1;
        }
    }
}
namespace OptimizationLib;

public class StochasticHillClimbing : IOptimizationAlgorithm
{
    private const string IterKey = "max_iterations";
    private const string StepSizeKey = "step_size";
    private const string AccelerationKey = "acceleration";

    private readonly OptimizationParameters _hyperParameters;

    public StochasticHillClimbing(int maxIterations, double stepSize, double acceleration)
    {
        _hyperParameters = new OptimizationParameters();
        _hyperParameters.SetParameter(IterKey, maxIterations);
        _hyperParameters.SetParameter(StepSizeKey, stepSize);
        _hyperParameters.SetParameter(AccelerationKey, acceleration);
    }
    
    private static List<double> StochasticClimb(Random random, List<double> minBounds, List<double> maxBounds, double stepSize,
        double acceleration, Score score, int maxIterations)
    {
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
            var uphillMoves = new List<List<double>>();
            
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
                        uphillMoves.Add(candidate);
                    }
                }
            }

            // TODO: make use of score as a weight.
            var nextSolutionIndex = random.Next(uphillMoves.Count);
            if (nextSolutionIndex < uphillMoves.Count)
                solution = uphillMoves[nextSolutionIndex];

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
    
    public List<double> Solve(Random random, List<double> minBounds, List<double> maxBounds, Score score)
    {
        var bestSolution = IOptimizationAlgorithm.GenerateInitial(random, minBounds, maxBounds);
        var iterations = _hyperParameters.GetParameter<int>("max_iterations");
        var stepSize = _hyperParameters.GetParameter<double>("step_size");
        var acceleration = _hyperParameters.GetParameter<double>("acceleration");

        for (var i = 0; i < iterations; i++)
        {
            var solution = StochasticClimb(random, minBounds, maxBounds, stepSize, acceleration, score, iterations);
            if (score(solution) > score(bestSolution))
            {
                bestSolution = solution;
            }
        }

        return bestSolution;
    }
}
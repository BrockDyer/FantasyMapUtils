namespace OptimizationLib.Tests;

public class StochasticHillClimbingTest
{

    private static double Poly2_2D(List<double> vector)
    {
        return -Math.Pow(vector[0], 2);
    }

    [Test]
    public void Test1()
    {
        var optimizer = new StochasticHillClimbing(100, 0.1, 1.2);
        var solution = optimizer.Solve(new Random(17),
            new List<double>(new[] { -10d }),
            new List<double>(new[] { 10d }),
            Poly2_2D);
        
        Console.WriteLine($"Best solution: {solution[0]}");
        Assert.That(Math.Abs(solution[0] - 0), Is.LessThan(0.001));
    }
}
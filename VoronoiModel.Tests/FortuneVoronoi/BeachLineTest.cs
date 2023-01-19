using VoronoiModel.FortuneVoronoi;
using VoronoiModel.Geometry;

namespace VoronoiModel.Tests.FortuneVoronoi;

[TestFixture]
public class BeachLineTest
{
    [Test]
    public void TestComputeBreakpoint()
    {
        // Setup
        var pNeg1 = new Point2D(-1, 0);
        var pOne = new Point2D(1, 0);
        var pZero = new Point2D(0, -1);
        var pPiOver3 = new Point2D(Math.Cos(Math.PI / 3), Math.Sin(Math.PI / 3));

        var expectedCenter = new Point2D(0, 0);

        const int sweepLineY = 1;
        
        // Perform operation
        var test1 = BeachLine.ComputeBreakpoint(pNeg1, pOne, sweepLineY);
        var test2 = BeachLine.ComputeBreakpoint(pNeg1, pZero, sweepLineY);
        var testHorizontal = BeachLine.ComputeBreakpoint(pOne, pNeg1, sweepLineY);
        var test3 = BeachLine.ComputeBreakpoint(pOne, pPiOver3, sweepLineY);

        const string failureMessageTemplate = "Incorrect center for circle through {0} and {1} tangent to horizontal line at {2}";
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(test1, Is.EqualTo(expectedCenter), failureMessageTemplate, pNeg1, pOne, sweepLineY);
            Assert.That(test2, Is.EqualTo(expectedCenter), failureMessageTemplate, pNeg1, pZero, sweepLineY);
            Assert.That(test3, Is.EqualTo(expectedCenter), failureMessageTemplate, pOne, pPiOver3, sweepLineY);
            Assert.That(testHorizontal, Is.EqualTo(expectedCenter), failureMessageTemplate, pOne, pNeg1, sweepLineY);
        });
    }
}
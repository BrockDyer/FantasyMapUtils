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
        var expectedPiOver3 = new Point2D(0.8543, 0.488);

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
            const double tolerance = 0.1;
            Assert.That(test3.X, Is.InRange(expectedPiOver3.X - tolerance, expectedPiOver3.X + tolerance), 
                "Incorrect x center for circle passing through {0} and {1} tangent to horizontal line at {2}", pOne, 
                pPiOver3, sweepLineY);
            Assert.That(test3.Y, Is.InRange(expectedPiOver3.Y - tolerance, expectedPiOver3.Y + tolerance), 
                "Incorrect y center for circle passing through {0} and {1} tangent to horizontal line at {2}", pOne, 
                pPiOver3, sweepLineY);
            Assert.That(testHorizontal, Is.EqualTo(expectedCenter), failureMessageTemplate, pOne, pNeg1, sweepLineY);
        });
    }
}
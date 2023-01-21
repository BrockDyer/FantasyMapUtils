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
        var a = new Point2D(1, 0);
        var b = new Point2D(3, 1);
        const int y = 2;

        var leftExpected = new Point2D(1.83772233983, 0.824555320337);
        var rightExpected = new Point2D(8.16227766017, -11.8245553203);
        
        // Perform
        var leftBreakpoint = BeachLine.ComputeBreakpoint(a, b, y);
        var rightBreakpoint = BeachLine.ComputeBreakpoint(b, a, y);

        bool PointsAreClose(Point2D p1, Point2D p2)
        {
            return Utils.AreClose(p1.X, p2.X) && Utils.AreClose(p1.Y, p2.Y);
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(PointsAreClose(leftBreakpoint, leftExpected), Is.True, $"Left breakpoint {leftBreakpoint} was not as expected {leftExpected}");
            Assert.That(PointsAreClose(rightBreakpoint, rightExpected), Is.True, $"Right breakpoint {rightBreakpoint} was not as expected {rightExpected}");
        });
    }

    private readonly Point2D _p1 = new(1, 0);
    private readonly Point2D _p2 = new(3, 1);
    private readonly Point2D _p3 = new(2, 2);

    private BeachLine _beachLine = null!;

    [SetUp]
    public void Setup()
    {
        _beachLine = new BeachLine(_p1);
    }

    [Test]
    public void TestInsertAndSplit()
    {
        // Perform
        var initialEntry = new BeachLineEntry(0, _p1);
        _beachLine.InsertAndSplit(initialEntry, _p2);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_beachLine, Has.Count.EqualTo(3), $"Incorrect count in beach line after adding {_p2}");
            Assert.That(_beachLine.ToString(), Is.EqualTo($"<{_p1}, {_p2}, {_p1}>"), $"Beach line representation is incorrect after adding {_p2}");
        });
        
        // Perform
        var secondEntry = new BeachLineEntry(1, _p2);
        _beachLine.InsertAndSplit(secondEntry, _p3);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_beachLine, Has.Count.EqualTo(5), $"Incorrect count in beach line after adding {_p3}");
            Assert.That(_beachLine.ToString(), Is.EqualTo($"<{_p1}, {_p2}, {_p3}, {_p2}, {_p1}>"), $"Incorrect representation after adding {_p3}");
        });
    }

    [Test]
    public void TestSearch()
    {
        // Perform
        var result = _beachLine.Search(_p2.Y, _p2);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Index, Is.EqualTo(0), "Result index is incorrect");
            Assert.That(result.Site, Is.EqualTo(_p1), "Incorrect site in entry");
        });
        
        // Setup
        _beachLine.InsertAndSplit(new BeachLineEntry(0, _p1), _p2);
        var test1 = new Point2D(1, 2); // Above p1
        var test2 = new Point2D(1.8377, 2); // Above first breakpoint
        var test3 = new Point2D(2.5, 2); // Above p2
        var test4 = new Point2D(8.1623, 2); // Above second breakpoint
        var test5 = new Point2D(9, 2); // Above p1 again

        // Perform
        var result1 = _beachLine.Search(test1.Y, test1);
        var result2 = _beachLine.Search(test2.Y, test2);
        var result3 = _beachLine.Search(test3.Y, test3);
        var result4 = _beachLine.Search(test4.Y, test4);
        var result5 = _beachLine.Search(test5.Y, test5);

        void AssertResult(BeachLineEntry testResult, int testIndex, int expectedIndex, Point2D expectedSite)
        {
            Assert.That(testResult.Index, Is.EqualTo(expectedIndex), $"Result {testIndex} index was wrong");
            Assert.That(testResult.Site, Is.EqualTo(expectedSite), $"Result {testIndex} site was wrong");
        }
        
        // Assert
        Assert.Multiple(() =>
        {
            AssertResult(result1, 1, 0, _p1);
            AssertResult(result2, 2, 0, _p1);
            AssertResult(result3, 3, 1, _p2);
            AssertResult(result4, 4, 2, _p1);
            AssertResult(result5, 5, 2, _p1);
        });
    }

    [Test]
    public void TestDelete()
    {
        // Setup
        TestInsertAndSplit();
        
        // Perform
        _beachLine.Delete(new BeachLineEntry(1, _p2));
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_beachLine, Has.Count.EqualTo(4));
            Assert.That(_beachLine.ToString(), Is.EqualTo($"<{_p1}, {_p3}, {_p2}, {_p1}>"));
        });
    }
}
using System;
using VoronoiModel.Geometry;

namespace VoronoiModel.Tests.Geometry
{
	[TestFixture]
	public class LineSegment2DTest
	{
		/// <summary>
		/// (0, 0) -> (1, 1)
		/// </summary>
		LineSegment2D segment1;

		/// <summary>
		/// (0, 1) -> (1, 0)
		/// </summary>
		LineSegment2D segment2;

		/// <summary>
		/// (0, 2) -> (1, 2)
		/// </summary>
		LineSegment2D segment3;

		[OneTimeSetUp]
		public void Setup()
		{
            Point2D p00 = new Point2D(0, 0);
            Point2D p11 = new Point2D(1, 1);

            Point2D p01 = new Point2D(0, 1);
            Point2D p10 = new Point2D(1, 0);

			segment1 = new LineSegment2D(p00, p11);
			segment2 = new LineSegment2D(p01, p10);

			segment3 = new LineSegment2D(new Point2D(0, 2), new Point2D(1, 2));
		}

		[Test]
		public void TestIntersectionWith()
        {
            // Perform operation
            var intersection12 = segment1.IntersectionWith(segment2);
			var intersection21 = segment2.IntersectionWith(segment1);
			var intersection13 = segment1.IntersectionWith(segment3);

			// (1, 2) -> (2, 1)
			var intersectsOutOfRange = new LineSegment2D(new Point2D(1, 2), new Point2D(2, 1));

            // Verify
            Assert.Multiple(() =>
            {
                Assert.That(intersection12, Is.Not.Null, "Intersection 1-2 was null.");
                Assert.That(intersection21, Is.Not.Null, "Intersection 2-1 was null.");
                Assert.That(intersection12, Is.EqualTo(new Point2D(0.5, 0.5)), "Intersection 1-2 not at expected point");
                Assert.That(intersection13, Is.Null, "Intersection 1-3 is not null, but should be.");
				Assert.That(segment1.IntersectionWith(intersectsOutOfRange), Is.Null, "Found an intersection, but is should be out of range.");
            });
        }

        [Test]
		public void TestIntersectsWith()
		{
			Assert.Multiple(() =>
			{
				Assert.That(segment1.IntersectsWith(segment2), Is.True, "Segment 1 should intersect with 2");
				Assert.That(segment2.IntersectsWith(segment1), Is.True, "Segment 2 should intersect with 2");
				Assert.That(segment1.IntersectsWith(segment3), Is.False, "Segment 1 should not intersect with 3");
				Assert.That(segment2.IntersectsWith(segment3), Is.False, "Segment 2 should not intersect with 3");
				Assert.That(segment1.IntersectsWith(segment1), Is.False, "Overlapping segments are not considered as intersecting.");

				// Test horizontal line version
				Assert.That(segment1.IntersectsWith(0.7), Is.True, "Segment 1 should intersect with horizontal line at 0.7");
				Assert.That(segment2.IntersectsWith(0.7), Is.True, "Segment 2 should intersect with horizontal line at 0.7");
				Assert.That(segment3.IntersectsWith(0.7), Is.False, "Segment 3 should not intersect with horizontal line at 0.7");
			});

        }
	}
}


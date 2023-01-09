using System;
using VoronoiModel.PlanarSubdivision;
using VoronoiModel.Geometry;

namespace VoronoiModel.Tests.PlanarSubdivision
{
	[TestFixture]
	public class DCELTest
	{
		readonly Point upperLeft = new Point(0, 100);
		readonly Point lowerRight = new Point(100, 0);
		readonly Point upperRight = new Point(100, 100);
		readonly Point lowerLeft = new Point(0, 0);

		DCEL dcel;

		[SetUp]
		public void Setup()
		{
			dcel = DCEL.Create(upperLeft, lowerRight);
		}

		[Test]
		public void TestAddVertex()
		{
			Assert.Fail();
		}

		[Test]
		public void TestSplitEdge()
		{
            Assert.Fail();
        }

		[Test]
		public void TestSplitFace()
		{
            Assert.Fail();
        }

		[Test]
		public void TestDeleteEdge()
		{
            Assert.Fail();
        }

		[Test]
		public void TestDeleteVertex()
		{
            Assert.Fail();
        }
	}
}


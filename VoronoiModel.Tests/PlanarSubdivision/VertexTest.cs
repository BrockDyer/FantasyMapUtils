using System;
using VoronoiModel.PlanarSubdivision;
using VoronoiModel.Geometry;

namespace VoronoiModel.Tests.PlanarSubdivision
{
	[TestFixture]
	public class VertexTest
	{
		Vertex v;
		HalfEdge emptyEdge;

		Vertex ve;
		Vertex vne1;
		Vertex vne2;
		Vertex vne3;

		[OneTimeSetUp]
		public void Setup()
		{
			emptyEdge = new HalfEdge(new Point2D(0, 0));
			v = emptyEdge.TargetVertex;

            ve = new Vertex(0, 0, emptyEdge);
            vne1 = new Vertex(0, 1, emptyEdge);
            vne2 = new Vertex(1, 0, emptyEdge);
            vne3 = new Vertex(1, 1, emptyEdge);
        }

		// [Test]
		// public void TestEquals()
		// {
		// 	Assert.Multiple(() =>
		// 	{
		// 		Assert.That(v, Is.EqualTo(ve), "v is not equal to ve");
		// 		Assert.That(v, Is.Not.EqualTo(vne1), "v is equal to vne1");
		// 		Assert.That(v, Is.Not.EqualTo(vne2), "v is equal to vne2");
		// 		Assert.That(v, Is.Not.EqualTo(vne3), "v is equalt to vne3");
		// 	});
		// }
  //
		// [Test]
		// public void TestHashCode()
		// {
		// 	var expectedHash = v.GetHashCode();
  //
		// 	Assert.Multiple(() =>
		// 	{
		// 		Assert.That(ve.GetHashCode(), Is.EqualTo(expectedHash),
		// 			"ve hash is not equal to v hash");
		// 		Assert.That(vne1.GetHashCode(), Is.Not.EqualTo(expectedHash),
		// 			"vne1 hash is equal to v hash");
		// 		Assert.That(vne2.GetHashCode(), Is.Not.EqualTo(expectedHash),
		// 			"vne2 hash is equal to v hash");
		// 		Assert.That(vne3.GetHashCode(), Is.Not.EqualTo(expectedHash),
		// 			"vne3 hash is equal to v hash");
		// 	});
  //       }

		[Test]
		public void TestToString()
		{
			var v1 = new Vertex(0, 0, emptyEdge);
			var v2 = new Vertex(0, 1, emptyEdge);

			var format = "({0}, {1})";
			var v1expected = string.Format(format, 0, 0);
			var v2expected = string.Format(format, 0, 1);

			Assert.Multiple(() =>
			{
				Assert.That(v1.ToString(), Is.EqualTo(v1expected), "v1 failed");
				Assert.That(v2.ToString(), Is.EqualTo(v2expected), "v2 failed");
			});
		}
	}
}


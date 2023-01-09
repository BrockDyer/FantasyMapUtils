using System;
using VoronoiModel.PlanarSubdivision;

namespace VoronoiModel.Tests.PlanarSubdivision
{
	[TestFixture]
	public class HalfEdgeTest
	{
		HalfEdge current;
		HalfEdge previous;
		HalfEdge next;
		HalfEdge nextTwin;

		Vertex target;
		Vertex source;
		Vertex nextTarget;

		[OneTimeSetUp]
		public void Setup()
		{
			target = new Vertex(1, 0);
			source = new Vertex(0, 0);
			nextTarget = new Vertex(2, 0);

			current = new HalfEdge(target);
			previous = new HalfEdge(source);
			next = new HalfEdge(nextTarget);
			nextTwin = new HalfEdge(target);

			// Establish linakges
			current.LinkNext(next);
			previous.LinkNext(current);

			// Establish twins
			next.LinkTwin(nextTwin);
		}

		[Test]
		public void TestSourceVertex()
		{
			var currentExpectedSource = source;
			Vertex? previousExpectedSource = null;
			var nextExpectedSource = target;
			Vertex? nextTwinExpectedSource = nextTarget;

			Assert.Multiple(() =>
			{
				//Assert.That(he, Is.EqualTo(h.SourceVertex));
				Assert.That(current.SourceVertex, Is.EqualTo(currentExpectedSource), "current source is wrong");
                //Assert.That(pe, Is.EqualTo(p.SourceVertex));
                Assert.That(previous.SourceVertex, Is.EqualTo(previousExpectedSource), "previous source is wrong");
				//Assert.That(ne, Is.EqualTo(n.SourceVertex));
				Assert.That(next.SourceVertex, Is.EqualTo(nextExpectedSource), "next source is wrong");
				//Assert.That(nte, Is.EqualTo(nt.SourceVertex));
				Assert.That(nextTwin.SourceVertex, Is.EqualTo(nextTwinExpectedSource), "next twin source is wrong");
			});
		}

		[Test]
		public void TestLinkTwin()
		{
			// Setup
			var nextNextTarget = new Vertex(3, 0);
			var nextNext = new HalfEdge(nextNextTarget);
			var nextNextTwin = new HalfEdge(nextTarget);

			// Perform operation
			nextNext.LinkTwin(nextNextTwin);

			// Validate
			Assert.Multiple(() =>
			{
                Assert.That(nextTwin, Is.EqualTo(next.Twin), "next twin's twin is wrong");
                Assert.That(next, Is.EqualTo(nextTwin.Twin), "next's twin is wrong");
                Assert.That(nextNext, Is.EqualTo(nextNextTwin.Twin), "nextNext's twin is wrong");
				Assert.That(nextNextTwin, Is.EqualTo(nextNext.Twin), "nextNextTwin's twin is wrong");
			});
		}

		[Test]
		public void TestLinkNext()
		{
			Assert.Multiple(() =>
			{
				Assert.That(previous, Is.EqualTo(current.Previous), "previous is not current.Previous");
				Assert.That(current, Is.EqualTo(previous.Next), "current is not previous.Next");
				Assert.That(current, Is.EqualTo(next.Previous), "current is not next.Previous");
				Assert.That(next, Is.EqualTo(current.Next), "next is not current.Next");
			});
		}

		[Test]
		public void TestEquals()
		{
			var source = new Vertex(0, 0);
			var target = new Vertex(1, 1);

			var h1 = new HalfEdge(target);
			var h1twin = new HalfEdge(source);
			h1.LinkTwin(h1twin);

			var h2 = new HalfEdge(target);
			var h3 = new HalfEdge(source);
			h3.LinkNext(h2);

			// h1 should equal h2
			// all others should be not equal

			Assert.Multiple(() =>
			{
				Assert.That(h2, Is.EqualTo(h1), "h2 is not equal to h1");
				Assert.That(h3, Is.Not.EqualTo(h1), "h3 is equal to h1");
				Assert.That(h1twin, Is.Not.EqualTo(h1), "h1twin is equal to h1");
				Assert.That(h2, Is.Not.EqualTo(h3), "h2 is equal to h3");
			});
		}

		[Test]
		public void TestHashCode()
		{
            var source = new Vertex(0, 0);
            var target = new Vertex(1, 1);

            var h1 = new HalfEdge(target);
            var h1twin = new HalfEdge(source);
            h1.LinkTwin(h1twin);

            var h2 = new HalfEdge(target);
            var h3 = new HalfEdge(source);
            h3.LinkNext(h2);

            var h1c = h1.GetHashCode();
			var h1tc = h1twin.GetHashCode();
			var h2c = h2.GetHashCode();
			var h3c = h3.GetHashCode();

			Assert.Multiple(() =>
			{
				Assert.That(h2c, Is.EqualTo(h1c), "h2 hash is not equal to h1 hash");
				Assert.That(h3c, Is.Not.EqualTo(h1c), "h3 hash is equal to h1 hash");
				Assert.That(h1tc, Is.Not.EqualTo(h1c), "h1t hash is equal to h1 hash");
				Assert.That(h2c, Is.Not.EqualTo(h3c), "h2 hash is equal to h3 hash");
			});
        }

		[Test]
		public void TestToString()
		{
			var currentExpected = string.Format("{0} -> {1}", source, target);
			//Assert.That(he, Is.EqualTo(h.ToString()));
			Assert.That(current.ToString(), Is.EqualTo(currentExpected));
		}
	}
}


using System;
using System.Diagnostics;
using VoronoiModel.Geometry;
using VoronoiModel.PlanarSubdivision;

namespace VoronoiModel.Tests.PlanarSubdivision
{
    [TestFixture]
    public class FaceTest
    {
        Point2D vul;
        Point2D vur;
        Point2D vlr;
        Point2D vll;

        HalfEdge h1;
        HalfEdge h1t;
        HalfEdge h2;
        HalfEdge h2t;
        HalfEdge h3;
        HalfEdge h3t;
        HalfEdge h4;
        HalfEdge h4t;

        Face f1;
        Face f2;
        Face f3;
        Face f4;

        [OneTimeSetUp]
        public void Setup()
        {
            // Setup vertices
            vul = new Point2D(0, 1);
            vur = new Point2D(1, 1);
            vlr = new Point2D(1, 0);
            vll = new Point2D(0, 0);

            // Initialize half edges
            h1 = new HalfEdge(vur);
            h1t = new HalfEdge(vul);
            h2 = new HalfEdge(vlr);
            h2t = new HalfEdge(vur);
            h3 = new HalfEdge(vll);
            h3t = new HalfEdge(vlr);
            h4 = new HalfEdge(vul);
            h4t = new HalfEdge(vll);

            // Set twins
            h1.LinkTwin(h1t);
            h2.LinkTwin(h2t);
            h3.LinkTwin(h3t);
            h4.LinkTwin(h4t);

            // Establish Next/Previous
            h1.LinkNext(h2);
            h2.LinkNext(h3);
            h3.LinkNext(h4);
            h4.LinkNext(h1);

            f1 = new Face(h1);
            f2 = new Face(h2);
            f3 = new Face(h3);
            f4 = new Face(h4);
        }

        // [Test]
        // public void TestEquals()
        // {
        //     Assert.Multiple(() =>
        //     {
        //         Assert.That(f2, Is.EqualTo(f1), "f2 not equal to f1");
        //         Assert.That(f3, Is.EqualTo(f1), "f3 not equal to f1");
        //         Assert.That(f4, Is.EqualTo(f1), "f4 not equal to f1");
        //         Assert.That(f3, Is.EqualTo(f2), "f3 not eqaul to f2");
        //     });
        // }
        //
        // [Test]
        // public void TestGetHashCode()
        // {
        //     var hash1 = f1.GetHashCode();
        //     var hash2 = f2.GetHashCode();
        //     var hash3 = f3.GetHashCode();
        //     var hash4 = f4.GetHashCode();
        //
        //     Assert.Multiple(() =>
        //     {
        //         Assert.That(hash2, Is.EqualTo(hash1), "hash2 not equal to hash1");
        //         Assert.That(hash3, Is.EqualTo(hash1), "hash3 not equal to hash1");
        //         Assert.That(hash4, Is.EqualTo(hash1), "hash4 not equal to hash1");
        //     });
        // }

        [Test]
        public void TestContainsPoint2D()
        {
            var pointWithin1 = new Point2D(0.5, 0.5);
            var pointWithin2 = new Point2D(0.1, 0.1);
            var pointOn = new Point2D(vur.X / 2, vur.Y);
            var pointOutside = new Point2D(-1, -1);
            
            Assert.Multiple(() =>
            {
                Assert.That(f1.ContainsPoint(pointWithin1), Is.True, "Face 1 does not contain {0}", pointWithin1);
                Assert.That(f1.ContainsPoint(pointWithin2), Is.True, "Face 1 does not contain {0}", pointWithin2);
                Assert.That(f1.ContainsPoint(pointOn), Is.True, "Point {0} on face 1 not recognized as inside face.", pointOn);
                Assert.That(f1.ContainsPoint(pointOutside), Is.False, "Point {0} was inside face 1.", pointOutside);
                
                // Sanity check
                Assert.That(f2.ContainsPoint(pointWithin1), Is.True, "Face 2 did not contain {0}", pointWithin1);
            });
        }

        [Test]
        public void TestGetFaceEdges()
        {
            var fe1 = f1.GetFaceEdges();
            var fe2 = f2.GetFaceEdges();
            var fe3 = f3.GetFaceEdges();
            var fe4 = f4.GetFaceEdges();

            Assert.Multiple(() =>
            {
                Assert.That(fe2, Is.EquivalentTo(fe1), "f2 does not have same edges as f1");
                Assert.That(fe3, Is.EquivalentTo(fe1), "f3 does not have same edges as f1");
                Assert.That(fe4, Is.EquivalentTo(fe1), "f4 does not have same edgse as f1");
                Assert.That(fe2, Is.EquivalentTo(fe4), "f2 does not have same edges as f4");
            });
        }

        [Test]
        public void TestCentroid()
        {
            var expected = new Point2D(0.5, 0.5);
            var c1 = f1.Centroid();
            var c2 = f2.Centroid();

            //TestContext.Progress.WriteLine(string.Format("f1: {0}", f1));

            Assert.Multiple(() =>
            {
                Assert.That(c1, Is.EqualTo(expected), "Centroid computation is wrong");
                Assert.That(c2, Is.EqualTo(c1), "f2 centriod did not match f1 centroid");
            });
        }
    }
}


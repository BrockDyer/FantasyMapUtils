using System;
using VoronoiModel.PlanarSubdivision;
using VoronoiModel.Geometry;

namespace VoronoiModel.Tests.PlanarSubdivision
{
	[TestFixture]
	public class DCELTest
	{
		readonly Point2D upperLeft = new Point2D(0, 100);
		readonly Point2D lowerRight = new Point2D(100, 0);
		readonly Point2D upperRight = new Point2D(100, 100);
		readonly Point2D lowerLeft = new Point2D(0, 0);

		DCEL dcel;

		[SetUp]
		public void Setup()
		{
			dcel = DCEL.Create(upperLeft, lowerRight);
		}

        [Test]
        public void TestGetFaceWithEdge()
        {
            // Perform operation
            var faceByEdge1 = dcel.GetFaceWithEdge(upperLeft, upperRight); // Face 1
            var faceByEdge1Twin = dcel.GetFaceWithEdge(upperRight, upperLeft); // null
            var faceByEdge2 = dcel.GetFaceWithEdge(upperRight, lowerRight); // Face 1
            var faceByEdge2Twin = dcel.GetFaceWithEdge(lowerRight, upperRight); // null
            var faceByEdge3 = dcel.GetFaceWithEdge(lowerRight, lowerLeft); // Face 1
            var faceByEdge4 = dcel.GetFaceWithEdge(lowerLeft, upperLeft); // Face 1

            var errorFormat = string.Format("Face by {{0}} -> {{1}} is not the same as face by {0} -> {1}.", upperLeft, upperRight);

            // Validate
            Assert.Multiple(() =>
            {
                Assert.That(faceByEdge2, Is.EqualTo(faceByEdge1), errorFormat, upperRight, lowerRight);
                Assert.That(faceByEdge3, Is.EqualTo(faceByEdge1), errorFormat, lowerRight, lowerLeft);
                Assert.That(faceByEdge4, Is.EqualTo(faceByEdge1), errorFormat, lowerLeft, upperLeft);
                Assert.That(faceByEdge1, Is.Not.Null, "Face by {0} -> {1} is null.", upperLeft, upperRight);
                Assert.That(faceByEdge1Twin, Is.Null, "Face by {0} -> {1} is not null", upperRight, upperLeft);
                Assert.That(faceByEdge2Twin, Is.Null, "Face by {0} -> {1} is not null", lowerRight, upperRight);
            });

            // Intellisense / compiler does not detect that faceByEdge1 must be
            // non null after Assert.Multiple. This if removes that warning. But is
            // otherwise not needed (assuming Assert.Multiple fails if any test
            // within fails).
            if (faceByEdge1 is not null)
            {
                var expectedPoint2Ds = new Point2D[] { upperLeft, upperRight, lowerRight, lowerLeft };
                ValidateFace(faceByEdge1, expectedPoint2Ds);
            }
        }

        [Test]
        public void TestGetFaces()
        {
            var faces = dcel.GetFaces();
            Assert.Multiple(() =>
            {
                Assert.That(faces, Has.Count.EqualTo(1), "Initial DCEL had more than 1 face.");
                ValidateFace(faces[0], new Point2D[] { upperLeft, upperRight, lowerRight, lowerLeft });
            });
        }

		[Test]
		public void TestAddVertex()
        {
            // Perform operation
            var newVertex = new Point2D(upperRight.X / 2, upperRight.Y / 2);
            dcel.AddVertex(newVertex, upperRight);

            // Validate
            var faces = dcel.GetFaces();

            // Want to fail and abort if count is not 1.
            Assert.That(faces, Has.Count.EqualTo(1), "DCEL does not have exactly 1 face.");

            // Validate that all Point2Ds are present and in the correct order.
            var face = faces[0];
            var expectedPoint2Ds = new Point2D[] { upperLeft, upperRight, newVertex, lowerRight, lowerLeft };

            ValidateFace(face, expectedPoint2Ds);
        }

        [Test]
		public void TestSplitEdge()
		{
            // Perform operation
            var newVertex = new Point2D(upperRight.X / 2, upperRight.Y);
            dcel.SplitEdge(newVertex, upperLeft, upperRight);

            // Validate
            var faces = dcel.GetFaces();

            // There should only be one face.
            Assert.That(faces, Has.Count.EqualTo(1), "DCEL does not have exactly 1 face.");

            // Validate that all Point2Ds are present and in the correct order.
            var face = faces[0];
            var expectedPoint2Ds = new Point2D[] { upperLeft, newVertex, upperRight, lowerRight, lowerLeft };

            ValidateFace(face, expectedPoint2Ds);

            // Perform another split face. Because we split both sides of a
            // half edge, we should test that splitting the off-face edge works
            // as expected (ie, switching source and target).
            var newVertex2 = new Point2D(lowerRight.X / 2, lowerRight.Y);
            dcel.SplitEdge(newVertex2, lowerLeft, lowerRight);

            // Validate that there is still only one face.
            faces = dcel.GetFaces();
            Assert.That(faces, Has.Count.EqualTo(1), "DCEL does not have exactly 1 face");

            // Validate that the face has all the Point2Ds in the correct order.
            face = faces[0];
            expectedPoint2Ds = new Point2D[] { upperLeft, newVertex, upperRight,
                lowerRight, newVertex2, lowerLeft };

            ValidateFace(face, expectedPoint2Ds);
        }

		[Test]
		public void TestSplitFace()
        {
            // Perform

            // Split across the main diagonal.
            dcel.SplitFace(upperLeft, lowerRight);

            // Validate

            // Should have two faces
            var faces = dcel.GetFaces();
            Assert.That(faces, Has.Count.EqualTo(2));

            // Get each face individually.
            var bottomLeftFace = dcel.GetFaceWithEdge(lowerRight, lowerLeft);
            var upperRightFace = dcel.GetFaceWithEdge(upperLeft, upperRight);

            // Check that the faces retrieved are not null.
            Assert.That(bottomLeftFace, Is.Not.Null, "Bottom left face is null");
            Assert.That(upperRightFace, Is.Not.Null, "Upper right face is null");


            // Check that the faces are correct.
            Assert.Multiple(() =>
            {
                var bottomLeftExpected = new Point2D[] { lowerLeft, upperLeft, lowerRight };
                var upperRightExpected = new Point2D[] { upperLeft, upperRight, lowerRight };

                ValidateFace(bottomLeftFace, bottomLeftExpected);
                ValidateFace(upperRightFace, upperRightExpected);
            });
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

        /// <summary>
        /// Validate that a face has the expected Point2Ds and in the correct order.
        /// </summary>
        /// <param name="face">The face to validate</param>
        /// <param name="expectedPoint2Ds">The expected Point2Ds in clockwise order.</param>
        /// <exception cref="AssertionException"/>
        private static void ValidateFace(Face face, Point2D[] expectedPoint2Ds)
        {
            // Get the vertices of the face.
            var edges = face.GetFaceEdges();

            // Assert that there is one edge per expected Point2D.
            Assert.That(edges, Has.Count.EqualTo(expectedPoint2Ds.Length),
                "Number of edges found on face did not match expected number of Point2Ds.");

            var facePoint2Ds = edges.Select(edge => edge.TargetVertex.Point).ToList();

            // Assert that all elements are present.
            Assert.That(facePoint2Ds, Is.EquivalentTo(expectedPoint2Ds),
                "Face did not contain all the expected Point2Ds.");

            // Now test that they are in order. We don't know what order the
            // Point2Ds on the face are, but they must be a continuous clockwise
            // permutation.
            int count = expectedPoint2Ds.Length;
            var testIndex = 0;
            var expectedIndex = 0;
            var lockstep = false;
            int lockstepAt = 0;
            while (testIndex < count)
            {
                var expected = expectedPoint2Ds[expectedIndex];
                var actual = facePoint2Ds[testIndex];

                if (!lockstep)
                {
                    // Once we find a match, they must remain equal for the entire step.
                    if (expected.Equals(actual))
                    {
                        lockstep = true;
                        lockstepAt = expectedIndex;
                        continue;
                    }
                }
                else
                {
                    Assert.That(actual, Is.EqualTo(expected),
                        "Test Element {0} was {1} and should be equal to Expected Element {2} which is {3}. The lists went lockstep at index {4}",
                        testIndex, actual, expectedIndex, expected, lockstepAt);

                    testIndex += 1;
                }

                expectedIndex += 1;
                expectedIndex %= count;
            }
        }
    }
}


using VoronoiModel.PlanarSubdivision;
using VoronoiModel.Geometry;

namespace VoronoiModel.Tests.PlanarSubdivision
{
	[TestFixture]
	public class DcelTest
	{
        /// <summary>
        /// (0, 100)
        /// </summary>
        private readonly Point2D _upperLeft = new(0, 100);
        /// <summary>
        /// (100, 0)
        /// </summary>
        private readonly Point2D _lowerRight = new(100, 0);
        /// <summary>
        /// (100, 100)
        /// </summary>
        private readonly Point2D _upperRight = new(100, 100);
        /// <summary>
        /// (0, 0)
        /// </summary>
        private readonly Point2D _lowerLeft = new(0, 0);

        private Dcel _dcel;

        [SetUp]
		public void Setup()
		{
			_dcel = Dcel.Create(_upperLeft, _lowerRight);
		}

        [Test]
        public void TestCreate()
        {
            void CollectPointsFollowingEdge(HalfEdge halfEdge, ICollection<Point2D> point2Ds)
            {
                var current = halfEdge;
                while (true)
                {
                    point2Ds.Add(current.TargetVertex.Point);
                    current = current.Next;

                    if (current?.Equals(halfEdge) ?? true)
                        break;
                }
            }

            // Get references
            var upperLeftToUpperRight = _dcel.GetEdge(_upperLeft, _upperRight);
            var twin = upperLeftToUpperRight?.Twin;

            if (upperLeftToUpperRight is null || twin is null)
                throw new CorruptedDcelException("DCEL was not constructed correctly.");
            
            // Expected results
            var expectedInteriorPoints = new[] { _upperLeft, _upperRight, _lowerRight, _lowerLeft };
            var expectedExteriorPoints = new[] { _upperLeft, _lowerLeft, _lowerRight, _upperRight };

            var actualInteriorPoints = new List<Point2D>();
            var actualExteriorPoints = new List<Point2D>();
            CollectPointsFollowingEdge(upperLeftToUpperRight, actualInteriorPoints);
            CollectPointsFollowingEdge(upperLeftToUpperRight.Twin!, actualExteriorPoints);

            ValidatePointOrder(expectedInteriorPoints, actualInteriorPoints, "Interior points");
            ValidatePointOrder(expectedExteriorPoints, actualExteriorPoints, "Exterior points");
        }

        [Test]
        public void TestGetFaceWithEdge()
        {
            // Perform operation
            var faceByEdge1 = _dcel.GetFaceWithEdge(_upperLeft, _upperRight); // Face 1
            var faceByEdge1Twin = _dcel.GetFaceWithEdge(_upperRight, _upperLeft); // null
            var faceByEdge2 = _dcel.GetFaceWithEdge(_upperRight, _lowerRight); // Face 1
            var faceByEdge2Twin = _dcel.GetFaceWithEdge(_lowerRight, _upperRight); // null
            var faceByEdge3 = _dcel.GetFaceWithEdge(_lowerRight, _lowerLeft); // Face 1
            var faceByEdge4 = _dcel.GetFaceWithEdge(_lowerLeft, _upperLeft); // Face 1

            var errorFormat = $"Face by {{0}} -> {{1}} is not the same as face by {_upperLeft} -> {_upperRight}.";

            // Validate
            Assert.Multiple(() =>
            {
                Assert.That(faceByEdge2, Is.EqualTo(faceByEdge1), errorFormat, _upperRight, _lowerRight);
                Assert.That(faceByEdge3, Is.EqualTo(faceByEdge1), errorFormat, _lowerRight, _lowerLeft);
                Assert.That(faceByEdge4, Is.EqualTo(faceByEdge1), errorFormat, _lowerLeft, _upperLeft);
                Assert.That(faceByEdge1, Is.Not.Null, "Face by {0} -> {1} is null.", _upperLeft, _upperRight);
                Assert.That(faceByEdge1Twin, Is.Null, "Face by {0} -> {1} is not null", _upperRight, _upperLeft);
                Assert.That(faceByEdge2Twin, Is.Null, "Face by {0} -> {1} is not null", _lowerRight, _upperRight);
            });

            // Intellisense / compiler does not detect that faceByEdge1 must be
            // non null after Assert.Multiple. This if removes that warning. But is
            // otherwise not needed (assuming Assert.Multiple fails if any test
            // within fails).
            if (faceByEdge1 is null) return;
            var expectedPoint2Ds = new[] { _upperLeft, _upperRight, _lowerRight, _lowerLeft };
            ValidateFace(faceByEdge1, expectedPoint2Ds);
        }

        [Test]
        public void TestGetFaces()
        {
            var faces = _dcel.GetFaces();
            Assert.Multiple(() =>
            {
                Assert.That(faces, Has.Count.EqualTo(1), "Initial DCEL had more than 1 face.");
                ValidateFace(faces[0], new[] { _upperLeft, _upperRight, _lowerRight, _lowerLeft });
            });
        }

		[Test]
		public void TestAddVertex()
        {
            // Perform operation
            var newVertex = new Point2D(_upperRight.X / 2, _upperRight.Y / 2);
            _dcel.AddVertex(newVertex, _upperRight);

            // Validate
            var faces = _dcel.GetFaces();

            // Want to fail and abort if count is not 1.
            Assert.That(faces, Has.Count.EqualTo(1), "DCEL does not have exactly 1 face.");

            // Validate that all Point2Ds are present and in the correct order.
            var face = faces[0];
            var expectedPoint2Ds = new[] { _upperLeft, _upperRight, newVertex, _upperRight, _lowerRight, _lowerLeft };

            ValidateFace(face, expectedPoint2Ds);
        }

        [Test]
		public void TestSplitEdge()
		{
            // Perform operation
            var newVertex = new Point2D(_upperRight.X / 2, _upperRight.Y);
            _dcel.SplitEdge(newVertex, _upperLeft, _upperRight);

            // Validate
            var faces = _dcel.GetFaces();

            // There should only be one face.
            Assert.That(faces, Has.Count.EqualTo(1), "DCEL does not have exactly 1 face.");

            // Validate that all Point2Ds are present and in the correct order.
            var face = faces[0];
            var expectedPoint2Ds = new[] { _upperLeft, newVertex, _upperRight, _lowerRight, _lowerLeft };

            ValidateFace(face, expectedPoint2Ds);

            // Perform another split face. Because we split both sides of a
            // half edge, we should test that splitting the off-face edge works
            // as expected (ie, switching source and target).
            var newVertex2 = new Point2D(_lowerRight.X / 2, _lowerRight.Y);
            _dcel.SplitEdge(newVertex2, _lowerLeft, _lowerRight);

            // Validate that there is still only one face.
            faces = _dcel.GetFaces();
            Assert.That(faces, Has.Count.EqualTo(1), "DCEL does not have exactly 1 face");

            // Validate that the face has all the Point2Ds in the correct order.
            face = faces[0];
            expectedPoint2Ds = new[] { _upperLeft, newVertex, _upperRight,
                _lowerRight, newVertex2, _lowerLeft };

            ValidateFace(face, expectedPoint2Ds);
        }

		[Test]
		public void TestSplitFace()
        {
            // Perform

            // Split across the main diagonal.
            var splitFaces = _dcel.SplitFace(_upperLeft, _lowerRight);

            // Validate

            // Should have two faces
            var faces = _dcel.GetFaces();
            Assert.That(faces, Has.Count.EqualTo(2));

            // Get each face individually.
            var bottomLeftFace = _dcel.GetFaceWithEdge(_lowerRight, _lowerLeft);
            var upperRightFace = _dcel.GetFaceWithEdge(_upperLeft, _upperRight);
            Assert.Multiple(() =>
            {
                // Check that the faces returned are the correct ones
                Assert.That(new[] {splitFaces.Item1, splitFaces.Item2}, 
                    Is.EquivalentTo(new [] {bottomLeftFace, upperRightFace}),
                    "Returned faces are not the correct ones.");
                
                // Check that the faces retrieved are not null.
                Assert.That(bottomLeftFace, Is.Not.Null, "Bottom left face is null");
                Assert.That(upperRightFace, Is.Not.Null, "Upper right face is null");
            });


            // Check that the faces are correct.
            Assert.Multiple(() =>
            {
                var bottomLeftExpected = new[] { _lowerLeft, _upperLeft, _lowerRight };
                var upperRightExpected = new[] { _upperLeft, _upperRight, _lowerRight };

                ValidateFace(bottomLeftFace!, bottomLeftExpected);
                ValidateFace(upperRightFace!, upperRightExpected);
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
        private static void ValidateFace(Face face, IReadOnlyList<Point2D> expectedPoint2Ds)
        {
            // Get the vertices of the face.
            var edges = face.GetFaceEdges();

            // Assert that there is one edge per expected Point2D.
            Assert.That(edges, Has.Count.EqualTo(expectedPoint2Ds.Count),
                "Number of edges found on face did not match expected number of Points.");

            var facePoint2Ds = edges.Select(edge => edge.TargetVertex.Point).ToList();

            ValidatePointOrder(expectedPoint2Ds, facePoint2Ds, "Face");
        }

        private static void ValidatePointOrder(IReadOnlyList<Point2D> expectedPoints, List<Point2D> actualPoints, string testObject)
        {
            // Assert that all elements are present.
            Assert.That(actualPoints, Is.EquivalentTo(expectedPoints),
                $"Actual points of {testObject} did not match expected");
            
            // Now test that they are in order. We don't know what order the
            // Point2Ds on the face are, but they must be a continuous clockwise
            // permutation.
            var count = expectedPoints.Count;
            var testIndex = 0;
            var expectedIndex = 0;
            var lockstep = false;
            var lockstepAt = 0;
            while (testIndex < count)
            {
                var expected = expectedPoints[expectedIndex];
                var actual = actualPoints[testIndex];

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


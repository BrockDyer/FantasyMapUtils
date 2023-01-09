using System;
using System.Diagnostics;

namespace VoronoiModel.Tests
{
	[SetUpFixture]
	public class SetupTrace
	{
		[OneTimeSetUp]
		public void StartTest()
		{
			Trace.Listeners.Add(new ConsoleTraceListener());
		}

		[OneTimeTearDown]
		public void EndTest()
		{
			Trace.Flush();
		}
	}
}


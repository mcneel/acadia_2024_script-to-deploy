using System;
using System.IO;

using Rhino;
using Rhino.Testing.Fixtures;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace WorkflowToolsNUnitTests
{
    [TestFixture]
    public sealed class TestWorkflowTools : RhinoTestFixture
    {
        [Test]
        public void TestSampleModelExists()
        {
            string output = Path.GetDirectoryName(GetType().Assembly.Location);
            string zipPath = Path.Combine(output, @"..\ref\24.11.05_MasterplanBuildings_Start.3dm.zip");
            string modelPath = Path.Combine(output, @"..\ref\24.11.05_MasterplanBuildings_Start.3dm");
            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, modelPath);

            Assert.FileExists(modelPath);
            Assert.DoesNotThrow(() => RhinoDoc.Open(modelPath, out bool _));
        }

    }
}

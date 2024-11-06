using System;
using System.IO;

using Rhino;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace WorkflowToolsNUnitTests
{
    [TestFixture]
    public sealed class TestWorkflowTools : Rhino.Testing.Fixtures.RhinoTestFixture
    {
        [Test]
        public void TestSampleModelIsValid()
        {
            string output = Path.GetDirectoryName(GetType().Assembly.Location);

            string refPath = Path.Combine(output, @"..\..\..\..\ref");
            string zipPath = Path.Combine(refPath, @"24.11.05_MasterplanBuildings_Start.3dm.zip");

            // deltete the files folder if it exists
            string filesPath = Path.Combine(output, @"..\..\..\..\ref\files");
            if (Directory.Exists(filesPath))
            {
                Directory.Delete(filesPath, true);
            }

            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, filesPath);

            string modelPath = Path.Combine(filesPath, @"24.11.05_MasterplanBuildings_Start.3dm");

            Assert.IsTrue(File.Exists(modelPath), "Does the sample model exist?");

            Assert.DoesNotThrow(() =>
            {
                using (RhinoDoc d = RhinoDoc.OpenHeadless(modelPath))
                {
                    // Do document things
                    var hatches = doc.Objects.FindByObjectType(Rhino.DocObjects.ObjectType.Hatch);
                    var hatch = hatches[0];
                    var userStringCount = hatch.Attributes.GetUserStrings().Count;
                    Assert.AreEqual(0, userStringCount, "The sample model should not have any user strings set.");
                }
            }, "Does Rhino open the sample model?");

        }
    }
}

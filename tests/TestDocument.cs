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
        public void TestSampleModelExists()
        {
            string output = Path.GetDirectoryName(GetType().Assembly.Location);

            string refPath = Path.Combine(output, @"..\..\..\..\ref");
            string zipPath = Path.Combine(refPath, @"24.11.05_MasterplanBuildings_Start.3dm.zip");

            string filesPath = Path.Combine(output, @"..\..\..\..\ref\files");
            if (Directory.Exists(filesPath))
            {
                Directory.Delete(filesPath, true);
            }

            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, filesPath);

            string modelPath = Path.Combine(filesPath, @"24.11.05_MasterplanBuildings_Start.3dm");

            Assert.IsTrue(File.Exists(modelPath), "Does the sample model exist?");
            Assert.DoesNotThrow(() => RhinoDoc.Open(modelPath, out bool _), "Does Rhino open the sample model?");

            var doc = RhinoDoc.ActiveDoc;
            var hatches = doc.Objects.FindByObjectType(Rhino.DocObjects.ObjectType.Hatch);
            var hatch = hatches[0];
            var userStringCount = hatch.Attributes.GetUserStrings().Count;

            Assert.AreEqual(0, userStringCount, "Does the sample model have user strings before Calculate Metrics?");
            //Assert.DoesNotThrow(() => RhinoApp.RunScript("WT_CalculateMetrics", false), "Does WT_CalculateMetrics run without error?");
            Assert.DoesNotThrow(() => RhinoApp.InvokeAndWait(new Action(() => RhinoApp.RunScript("WT_CalculateMetrics", false))), "Does WT_CalculateMetrics run without error?");
            Assert.IsTrue(hatch.Attributes.GetUserStrings().Count > 0, "Does the sample model have user strings after Calculate Metrics?");

        }
    }
}

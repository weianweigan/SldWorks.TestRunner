// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using NUnit.Framework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.IO;
using Xarial.XCad;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SolidWorks;

namespace SldWorksTest
{
    [TestFixture]
    public class SwAppTest
    {
        [SetUp]
        public void SetUp(ISldWorks swApp)
        {
            Assert.NotNull(swApp);
        }

        [TearDown]
        public void TearDown(ISldWorks swApp)
        {
            Assert.NotNull(swApp);
        }

        [Test]
        public void NewPartTest(ISldWorks swApp)
        {
            var partDoc = NewPartDoc(swApp);
        }

        [Test]
        public void FeatTest(ISldWorks swApp)
        {
            var partDoc = NewPartDoc(swApp);
        }

        private IPartDoc NewPartDoc(ISldWorks swApp)
        {
            Assert.NotNull(swApp);

            var partTemPath = swApp.GetUserPreferenceStringValue((int)
swUserPreferenceStringValue_e.swDefaultTemplatePart)?.Replace("零件", "gb_part");

            Assert.IsTrue(File.Exists(partTemPath));

            return swApp.NewDocument(partTemPath, 0, 0, 0) as IPartDoc;
        }
    }
}

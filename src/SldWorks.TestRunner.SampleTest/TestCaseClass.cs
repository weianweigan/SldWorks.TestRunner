// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using NUnit.Framework;

namespace CADApplication.TestRunner.SampleTest
{
    public class TestCaseClass
    {

        [Test()]
        [TestCase(1,2,3,Category ="加法测试")]
        [TestCase(2, 4, 6,Category = "加法测试")]
        [TestCase(2, -4, -2, Category = "减法测试")]
        [TestCase(2, -4, 0, Category = "减法测试")]
        public void TestAdd(int a,int b,int result)
        {
            Assert.AreEqual(a + b, result);
        }
    }
}

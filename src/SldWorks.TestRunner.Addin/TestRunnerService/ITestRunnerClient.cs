namespace SldWorks.TestRunner.Addin
{
    public interface ITestRunnerClient
    {
        void SendResult(int type, string msg);

        void UpdateStatus(int state);

    }
}

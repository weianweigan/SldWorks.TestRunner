using CADApplication.TestRunner.View;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SldWorks.TestRunner.Addin
{
    [HubName("TestRunner")]
    public class TestRuunerHub:Hub<ITestRunnerClient>
    {
        private readonly NUnitRunnerViewModel<ISldWorks> _runner;

        public TestRuunerHub(NUnitRunnerViewModel<ISldWorks> runner)
        {
            this._runner = runner;
        }

        /// <summary>
        /// 对程序集执行测试
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        public void TestAssembly(string assemblyPath)
        {
            _runner.OpenAssembly(assemblyPath);

            var root = _runner.Tree.ObjectTree.FirstOrDefault();

            if (root != null)
            {
                _runner.Tree.SelectedNode = root;
            }

            _runner.DebugCommand.Execute(null);

            Clients.Caller.SendResult(0,Newtonsoft.Json.JsonConvert.SerializeObject(_runner.Tree));
        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using CADApplication.TestRunner.Runner.NUnit;
using CADApplication.TestRunner.View.TestTreeView;

namespace CADApplication.TestRunner.Runner.Direct
{
    /// <summary>
    /// This Runner runs the corresponding Test using Reflection.
    /// </summary>
    public class ReflectionRunner
    {
        public ReflectionRunner( string aAssemblyPath )
        {
            AssemblyPath = aAssemblyPath;
        }

        /// <summary>程序集路径 </summary>
        internal string AssemblyPath { get; }

        /// <summary>运行测试</summary>
        /// <typeparam name="TApp">需要依赖注入的实例类型</typeparam>
        /// <param name="test">测试的数据</param>
        /// <param name="swApp">需要依赖注入的实例</param>
        internal void RunTest<TApp>( NodeViewModel test, TApp swApp )
        {
            //方法名
            string methodName = test.MethodName;
            
            //计时器
            var stopWatch = new Stopwatch();


            if ( test.Parent is NodeViewModel parent ) {
                string className = parent.ClassName;

                if( !string.IsNullOrEmpty( className ) && !string.IsNullOrEmpty( methodName ) ) {
                    var possibleParams = new object[] { swApp};

                    object obj = null;
                    MethodInfo setUp = null;
                    MethodInfo tearDown = null;
                    MethodInfo testMethod = null;

                    try {
                        Assembly assembly = Assembly.LoadFile( AssemblyPath );
                        Type type = assembly.GetType( className );
                        obj = Activator.CreateInstance( type );

                        setUp = GetMethodByAttribute( type, typeof( SetUpAttribute ) );
                        testMethod = type.GetMethod( methodName );
                        tearDown = GetMethodByAttribute( type, typeof( TearDownAttribute ) );

                        //获取标记
                        var customAttributes = testMethod.CustomAttributes;
                        //参数
                        var extendedParams = possibleParams.ToList();

                        //筛选出需要的TestCase类型
                        if (test.Type == TestType.Case)
                        {
                            var caseIndex = test.Parent.Children.IndexOf(test);
                            int i = 0;
                            customAttributes = customAttributes.Where(attr =>
                                (attr.AttributeType.Name == nameof(TestCaseAttribute)) ?
                                    (i++ == caseIndex): true);
                        }

                        //添加参数
                        foreach ( CustomAttributeData customAttribute in customAttributes ) {
                            extendedParams.AddRange( customAttribute.ConstructorArguments.Select( a => a.Value ) );
                        }

                        //Setup执行
                        Invoke( obj, setUp, possibleParams );

                        stopWatch.Start();
                        //测试方法执行
                        Invoke( obj, testMethod, extendedParams.ToArray() );

                        //标记测试状态
                        test.State = TestState.Passed;
                    }
                    catch( Exception e ) {
                        ReportException( test, e );
                    }
                    finally {

                        //统计时间
                        stopWatch.Stop();
                        test.Time = stopWatch.Elapsed.Milliseconds;
                        
                        try
                        {
                            Invoke( obj, tearDown, possibleParams );
                        }
                        catch( Exception e ) {
                            ReportException( test, e );
                        }
                    }

                    Log.Info( $" >> {test.FullName} - {test.State} - {test.Message}" );
                }
            }
        }

        /// <summary>执行</summary>
        /// <param name="obj">对象</param>
        /// <param name="method">方法</param>
        /// <param name="possibleParams">可能的参数</param>
        private void Invoke( object obj, MethodInfo method, object[] possibleParams )
        {
            if( method != null ) {
                var methodParams = OrderParameters( method, possibleParams );
                method.Invoke( obj, methodParams );
            }
        }

        private void ReportException( NodeViewModel node, Exception e )
        {
            node.State = TestState.Failed;

            Exception toLogEx = e.InnerException ?? e;

            Log.Error( toLogEx );
            node.Message = toLogEx.Message;
            node.StackTrace = toLogEx.StackTrace;
        }

        private object[] OrderParameters(MethodInfo methodInfo, object[] possibleParams)
        {
            var result = new List<object>();
            var parameters = methodInfo.GetParameters();
            var possibleParamsList = possibleParams.ToList();

            foreach (ParameterInfo parameter in parameters) {
                object o = possibleParamsList.FirstOrDefault(i => (i != null
                                                                   && i.GetType() == parameter.ParameterType) ||
                                                                   (i.GetType().FullName == "System.__ComObject"
                                                                   && parameter.ParameterType.FullName == "SolidWorks.Interop.sldworks.ISldWorks"));
                possibleParamsList.Remove( o );
                result.Add( o );
            }

            return result.ToArray();
        }

        private MethodInfo GetMethodByAttribute( Type type, Type attributeType )
        {
            var listOfMethods = new List<MethodInfo>();

            foreach( MethodInfo method in type.GetMethods() ) {
                if( MarkedByAttribute( method, attributeType ) ) {
                    listOfMethods.Add( method );
                }
            }

            if( listOfMethods.Count > 1 ) throw new InvalidOperationException( $"More than method marked with '{attributeType.Name}' attribute found!" );

            return listOfMethods.SingleOrDefault();
        }

        private bool MarkedByAttribute( MethodInfo methodInfo, Type attributeType )
        {
            return methodInfo.GetCustomAttributes( true ).Select( a => a.ToString() ).Contains( attributeType.FullName );
        }
    }
}

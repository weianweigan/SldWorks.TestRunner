﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CADApplication.TestRunner.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SldWorks.TestRunner.Properties.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 MESSAGE 的本地化字符串。
        /// </summary>
        public static string Message {
            get {
                return ResourceManager.GetString("Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 NAME 的本地化字符串。
        /// </summary>
        public static string Name {
            get {
                return ResourceManager.GetString("Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Open Log Floder 的本地化字符串。
        /// </summary>
        public static string OpenFloder {
            get {
                return ResourceManager.GetString("OpenFloder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Open Log 的本地化字符串。
        /// </summary>
        public static string OpenLog {
            get {
                return ResourceManager.GetString("OpenLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Test Runner 的本地化字符串。
        /// </summary>
        public static string TestRunner {
            get {
                return ResourceManager.GetString("TestRunner", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 TIME 的本地化字符串。
        /// </summary>
        public static string Time {
            get {
                return ResourceManager.GetString("Time", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 TYPE 的本地化字符串。
        /// </summary>
        public static string Type {
            get {
                return ResourceManager.GetString("Type", resourceCulture);
            }
        }
    }
}
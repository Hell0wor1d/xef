//==============================================================================  
//Copyright (C) 2012-2015 9UN.ORG. All rights reserved. 
//GUID：07deb6ff-dd6e-4305-9405-e0dbd63f254b
//CLR Version: 4.0.30319.18010
//Code Author：Kevin Wang
//Contact：Email(Admin@9un.org),QQ(265382 or 74344)
//Filename：XmlEntityCacheObject
//Namespace：XmlEntityFramework
//Functions：通用 XmlToEntity ORM 框架
//为庆祝2013新年业余之作，新一年新起点。
//本框架将会在GPL协议下开源，源码可以随意更改，但请务必保留代码文件顶端的版权以及注释
//目前已测试三种不同的XML嵌套方式，如果在使用过程中发现有BUG，请联系作者。  
//
//主要功能：
//1，实现通用XML处理引擎，XmlParseEngine；
//2，可自定义的处理引擎，可定制接口IXmlParseEngine；
//3，XML映射到实体对象，ORM；
//4，XML实体类型动态绑定, DOB；
//5，灵活的XPATH设置模式；
//6，实体可设置缓存, 自动控制实体生命周期， Cache；
//7，十几种重载的读取方式，内存流，远程文件，本地文件，override；
//8，支持各类复杂的XML嵌套, Nested；
//9，支持多线程安全操作, Safe Thread；
//10，支持REST风格的HTTP读取；
//11，兼容.NET 2.0（包含）以上版本  
//Created by Kevin Wang at 2013/1/2 2:17:26 http://blog.9un.org
//============================================================================== 

using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace XmlEntityFramework
{
    /// <summary>
    /// Class XmlEntityCacheObject
    /// </summary>
    public class XmlEntityCacheObject : IDisposable
    {
        private Timer _timer;
        /// <summary>
        /// Gets the cache object.
        /// </summary>
        /// <value>The cache object.</value>
        public object Cache { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEntityCacheObject" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="lifeCycle">The life cycle.</param>
        /// <exception cref="System.ArgumentNullException">targetObject</exception>
        public XmlEntityCacheObject(object target, int lifeCycle)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (lifeCycle > 0x00)
            {
                _timer = new Timer();
                _timer.Elapsed += TimerEvent;
                // 设置引发时间的时间间隔
                _timer.Interval = lifeCycle * 1000;
                _timer.Enabled = true;
            }
            Cache = target;
        }

        /// <summary>
        /// Timers the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs" /> instance containing the event data.</param>
        private void TimerEvent(object sender, ElapsedEventArgs e)
        {
            if (Cache == null) return;
#if DEBUG
            Console.WriteLine("清理缓存 {0} , 线程：{1}", Cache.GetType(), Thread.CurrentThread.GetHashCode());
#endif
            Cache = null;
        }

        // Dispose() calls Dispose(true)
        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't 
        // own unmanaged resources itself, but leave the other methods
        // exactly as they are. 
        /// <summary>
        /// Finalizes an instance of the <see cref="XmlEntityCacheObject" /> class.
        /// </summary>
        ~XmlEntityCacheObject()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            // free managed resources
            if (_timer == null) return;
            _timer.Dispose();
            _timer = null;
        }
    }
}

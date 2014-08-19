//==============================================================================  
//Copyright (C) 2012-2015 9UN.ORG. All rights reserved. 
//GUID：f99b2251-5427-47bd-93c0-b34d03282121
//CLR Version: 4.0.30319.18010
//Code Author：Kevin Wang
//Contact：Email(Admin@9un.org),QQ(265382 or 74344)
//Filename：XmlEntityCacheWorker
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
//Created by Kevin Wang at 2013/1/2 2:00:29 http://blog.9un.org
//============================================================================== 

using System;
using System.Collections;
using System.Threading;

namespace XmlEntityFramework
{
    /// <summary>
    /// Class XmlEntityCacheWorker
    /// </summary>
    public class XmlEntityCacheWorker
    {
        /// <summary>
        /// The _instance
        /// </summary>
        private volatile static XmlEntityCacheWorker _instance;
        private volatile static object _obj = new object();
        private readonly Hashtable _cacheTable;
        /// <summary>
        /// Prevents a default instance of the <see cref="XmlEntityCacheWorker" /> class from being created.
        /// </summary>
        private XmlEntityCacheWorker()
        {
            _cacheTable = Hashtable.Synchronized(new Hashtable());
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static XmlEntityCacheWorker Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_obj)
                    {
                        if (_instance == null)
                        {
                            _instance = new XmlEntityCacheWorker();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Sets the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="lifeCycle">The life cycle.</param>
        public void Set(Type id, object entity, int lifeCycle)
        {
            lock (_obj)
            {
                if (!_cacheTable.ContainsKey(id))
                {
#if DEBUG
                    Console.WriteLine("Set Cache {0}, Thread Id: {1}", id, Thread.CurrentThread.Name);
#endif
                    var cObject = new XmlEntityCacheObject(entity, lifeCycle);
                    _cacheTable.Add(id, cObject);
                    return;
                }
                var cacheObject = (XmlEntityCacheObject)_cacheTable[id];
                if (cacheObject == null)
                {
                    var newObject = new XmlEntityCacheObject(entity, lifeCycle);
                    _cacheTable[id] = newObject;
                    return;
                }
                if (cacheObject.Cache != null) return;
#if DEBUG
                Console.WriteLine("Update Cache {0}, Thread Id: {1}", id, Thread.CurrentThread.Name);
#endif
                cacheObject.Cache = entity;
            }
        }

        /// <summary>
        /// Gets the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>System.Object.</returns>
        public object Get(Type id)
        {
            if (!_cacheTable.ContainsKey(id))
            {
                return null;
            }
            var cacheObject = _cacheTable[id] as XmlEntityCacheObject;
            if (cacheObject != null && cacheObject.Cache != null)
            {
#if DEBUG
                Console.WriteLine("Read Cache {0}, Thread Id: {1}", id, Thread.CurrentThread.Name);
#endif
                return  cacheObject.Cache;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _cacheTable.Clear();
        }

        /// <summary>
        /// Determines whether the specified id contains key.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns><c>true</c> if the specified id contains key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(Type id)
        {
            return _cacheTable.ContainsKey(id);
        }

        /// <summary>
        /// Removes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        public void Remove(Type id)
        {
            _cacheTable.Remove(id);
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
          get { return _cacheTable.Count; }  
        }
    }
}

//==============================================================================  
//Copyright (C) 2012-2015 9UN.ORG. All rights reserved. 
//GUID：e3ceafb3-287d-4082-a41e-d3b40387877e
//CLR Version: 4.0.30319.18010
//Code Author：Kevin Wang
//Contact：Email(Admin@9un.org),QQ(265382 or 74344)
//Filename：EntityProvider
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
//Created by Kevin Wang at 2012/12/31 21:35:50 http://blog.9un.org
//============================================================================== 

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace XmlEntityFramework
{
    /// <summary>
    /// Class XmlEntityProvider
    /// </summary>
    public class XmlEntityProvider : IXmlEntityProvider
    {
        /// <summary>
        /// The HTTP regex pattern
        /// </summary>
        private readonly Regex _httpRegex;

        /// <summary>
        /// The local regex pattern
        /// </summary>
        private readonly Regex _localRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEntityProvider" /> class.
        /// </summary>
        public XmlEntityProvider()
        {
            _httpRegex = new Regex(@"http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            _localRegex = new Regex(@"[a-zA-Z]\:[\\a-zA-Z0-9_\\]+[\.]?[a-zA-Z0-9_]+");
        }

        #region 读取方式
        /// <summary>
        /// Reads the From local.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.IO.IOException">文件不存在，请检查文件是否存在。目标文件： + localPath</exception>
        protected string ReadFromLocal(string localPath, Encoding encoding)
        {
            if (!File.Exists(localPath))
            {
                throw new IOException("The file does not exist, please check whether a file exists. Target file:" + localPath);
            }
            string xmlContent;
            using (var fs = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var sr = new StreamReader(fs, encoding);
                xmlContent = sr.ReadToEnd();
            }
            return xmlContent;
        }

        /// <summary>
        /// Reads the From HTTP.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="method">The method.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="paras">The paras.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.Exception">不是有效的网络地址，请检查文件是否存在。目标地址： + url</exception>
        protected string ReadFromHttp(string url, Encoding encoding, HttpMethodType method = HttpMethodType.GET,
                                      WebHeaderCollection headers = null, NameValueCollection paras = null)
        {
            if (!_httpRegex.IsMatch(url))
            {
                throw new Exception("Is not a valid network path, the destination address:" + url);
            }
            const string acceptType = "Accept";
            const string contentTypeValue = "text/html,application/xhtml+xml,application/xml";
            const string userAgent = "User-Agent";
            const string userAgentValue =
                "Mozilla/5.0 (Windows; U; Windows NT 5.2) AppleWebKit/525.13 (KHTML,like Gecko) Chrome/0.2.149.27 Safari/525.13";
            if (headers == null)
            {
                headers = new WebHeaderCollection();
            }
            if (paras == null)
            {
                paras = new NameValueCollection();
            }
            headers.Remove(acceptType);
            headers.Add(acceptType, contentTypeValue);
            headers.Remove(userAgent);
            headers.Add(userAgent, userAgentValue);
            string xmlContent = null;
            using (var wc = new WebClient { Headers = headers, Encoding = encoding })
            {
                switch (method)
                {
                    case HttpMethodType.GET:
                        wc.QueryString = paras;
                        xmlContent = wc.DownloadString(url);
                        break;
                    case HttpMethodType.POST:
                    case HttpMethodType.PUT:
                    case HttpMethodType.DELETE:

                        wc.Headers = headers;
                        var byRemoteInfo = wc.UploadValues(url, method.ToString(), paras);
                        xmlContent = encoding.GetString(byRemoteInfo);
                        break;
                }
            }
            return xmlContent;
        }

        #endregion

        #region 缓存处理
        /// <summary>
        /// Uses the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Object.</returns>
        protected Object UseCache(Type key)
        {
            return XmlEntityCacheWorker.Instance.Get(key);
        }

        /// <summary>
        /// Determines whether the specified key has cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified key has cache; otherwise, <c>false</c>.</returns>
        protected bool HasCache(Type key)
        {
            var xmlEntityCacheAttribute = XmlEntityUtility.GetXmlEntityCacheAttribute(key);
            return null != xmlEntityCacheAttribute;
        }

        /// <summary>
        /// Sets the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tarObject">The tar object.</param>
        protected void SetCache<T>(object tarObject) where T : class
        {
            var type = typeof(T);
            var xmlEntityCacheAttribute = XmlEntityUtility.GetXmlEntityCacheAttribute(type);
            if (null == xmlEntityCacheAttribute)
            {
                return;
            }
            //var id = typeof(T);
            var lifeCycle = xmlEntityCacheAttribute.LifeCycle;
            XmlEntityCacheWorker.Instance.Set(type, tarObject, lifeCycle);
        }

        #endregion

        /// <summary>
        /// Read the specified URL or local path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlOrLocalPath">The URL or local path.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<T> Read<T>(string urlOrLocalPath, bool forceRefreshCache = false) where T : class
        {
            return Read<T>(urlOrLocalPath, Encoding.Default, forceRefreshCache);
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<T> Read<T>(MemoryStream stream, bool forceRefreshCache = false) where T : class
        {
            return Read<T>(stream, Encoding.Default, forceRefreshCache);
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<T> Read<T>(MemoryStream stream, Encoding encoding, bool forceRefreshCache = false) where T : class
        {
            var type = typeof(T);
            if (!forceRefreshCache && HasCache(type))
            {
                var cache = UseCache(type);
                if (cache != null)
                {
                    return (List<T>)cache;
                }
            }
            var xmlContent = encoding.GetString(stream.GetBuffer());
            return ParseData<T>(xmlContent);
        }

        /// <summary>
        /// Read the specified URL or local path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlOrLocalPath">The URL or local path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        /// <exception cref="System.Exception">不是有效的网络路径或本地路径，目标地址： + urlOrLocalPath</exception>
        public List<T> Read<T>(string urlOrLocalPath, Encoding encoding, bool forceRefreshCache = false) where T : class
        {
            var type = typeof(T);
            if (!forceRefreshCache && HasCache(type))
            {
                var cache = UseCache(type);
                if (cache != null)
                {
                    return (List<T>)cache;
                }
            }
            if (_localRegex.IsMatch(urlOrLocalPath))
            {
                var xmlContent = ReadFromLocal(urlOrLocalPath, encoding);
                return ParseData<T>(xmlContent);
            }
            if (_httpRegex.IsMatch(urlOrLocalPath))
            {
                return Read<T>(urlOrLocalPath, encoding, HttpMethodType.GET, null, null);
            }
            throw new Exception("Is not a valid network path or local path, the destination address:" + urlOrLocalPath);
        }

        /// <summary>
        /// Read the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        public List<T> Read<T>(string url, HttpMethodType method, bool forceRefreshCache = false) where T : class
        {
            return Read<T>(url, Encoding.Default, method, null, null, forceRefreshCache);
        }

        /// <summary>
        /// Reads the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="paras">The paras.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<T> Read<T>(string url, NameValueCollection paras, bool forceRefreshCache = false) where T : class
        {
            return Read<T>(url, Encoding.Default, HttpMethodType.GET, null, paras, forceRefreshCache);
        }

        /// <summary>
        /// Reads the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        public List<T> Read<T>(string url, WebHeaderCollection headers, bool forceRefreshCache = false) where T : class
        {
            return Read<T>(url, Encoding.Default, HttpMethodType.GET, headers, null, forceRefreshCache);
        }

        /// <summary>
        /// Reads the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="paras">The paras.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        public List<T> Read<T>(string url, WebHeaderCollection headers, NameValueCollection paras, bool forceRefreshCache = false) where T : class
        {
            return Read<T>(url, Encoding.Default, HttpMethodType.GET, headers, paras, forceRefreshCache);
        }

        /// <summary>
        /// Read the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="paras">The paras.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        public List<T> Read<T>(string url, HttpMethodType method, NameValueCollection paras, bool forceRefreshCache = false) where T : class
        {
            return Read<T>(url, Encoding.Default, method, null, paras, forceRefreshCache);
        }

        /// <summary>
        /// Reads the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        public List<T> Read<T>(string url, HttpMethodType method, WebHeaderCollection headers, bool forceRefreshCache = false) where T : class
        {
            return Read<T>(url, Encoding.Default, method, headers, null, forceRefreshCache);
        }

        /// <summary>
        /// Gets the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="paras">The paras.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<T> Read<T>(string url, HttpMethodType method, WebHeaderCollection headers, NameValueCollection paras, bool forceRefreshCache = false) where T : class
        {
            return Read<T>(url, Encoding.Default, method, headers, paras, forceRefreshCache);
        }

        /// <summary>
        /// Read the specified URL or local path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL or local path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="method">The method.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        public List<T> Read<T>(string url, Encoding encoding, HttpMethodType method, bool forceRefreshCache = false) where T : class
        {
            return Read<T>(url, encoding, method, null, null, forceRefreshCache);
        }

        /// <summary>
        /// Read the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="method">The method.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        public List<T> Read<T>(string url, Encoding encoding, HttpMethodType method, WebHeaderCollection headers, bool forceRefreshCache = false) where T : class
        {
            return Read<T>(url, encoding, method, headers, null, forceRefreshCache);
        }

        /// <summary>
        /// Read the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="method">The method.</param>
        /// <param name="paras">The paras.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        public List<T> Read<T>(string url, Encoding encoding, HttpMethodType method, NameValueCollection paras, bool forceRefreshCache = false) where T : class
        {
            return Read<T>(url, encoding, method, null, paras, forceRefreshCache);
        }

        /// <summary>
        /// Reads the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="method">The method.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="paras">The paras.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        public List<T> Read<T>(string url, Encoding encoding, HttpMethodType method, WebHeaderCollection headers,
                            NameValueCollection paras, bool forceRefreshCache = false) where T : class
        {
            var type = typeof(T);
            if (!forceRefreshCache && HasCache(type))
            {
                var cache = UseCache(type);
                if (cache != null)
                {
                    return (List<T>)cache;
                }
            }
            var xml = ReadFromHttp(url, encoding, method, headers, paras);
            return ParseData<T>(xml);
        }

        /// <summary>
        /// Parses the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlContent">Content of the XML.</param>
        /// <returns>List{``0}.</returns>
        protected List<T> ParseData<T>(string xmlContent) where T : class
        {
            var res = new List<T>();
            var type = typeof(T);
#if DEBUG
            Console.WriteLine("Real-time processing {0}, thread: {1}", type, Thread.CurrentThread.Name);
#endif
            var attribute = XmlEntityUtility.GetXmlEntityHandlerAttribute(type);
            var result = attribute.EntityParser.Invoke(xmlContent, type);
            var flag = XmlEntityUtility.GetXmlEntityAttribute(type).XmlEntityFlag;
            var isSingle = false;
            switch (flag)
            {
                case XmlEntityFlags.Base:
                case XmlEntityFlags.Nested:
                case XmlEntityFlags.Single | XmlEntityFlags.Base:
                case XmlEntityFlags.Single | XmlEntityFlags.Nested:
                case XmlEntityFlags.Nested | XmlEntityFlags.Base | XmlEntityFlags.Single:
                    isSingle = true;
                    break;
            }
            //判断是否实体集
            if (!isSingle)
            {
                var items = result as object[];
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        res.Add(item as T);
                    }
                }
                //尝试设置缓存
                SetCache<T>(res);
                return res;
            }
            res.Add(result as T);
            //尝试设置缓存
            SetCache<T>(res);
            return res;
        }
    }
}

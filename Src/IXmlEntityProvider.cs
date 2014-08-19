//==============================================================================  
//Copyright (C) 2012-2015 9UN.ORG. All rights reserved. 
//GUID：fa58b797-ddca-41d6-9170-cdbb6f69ba50
//CLR Version: 4.0.30319.18010
//Code Author：Kevin Wang
//Contact：Email(Admin@9un.org),QQ(265382 or 74344)
//Filename：IEntityProvider
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
//Created by Kevin Wang at 2012/12/31 21:31:15 http://blog.9un.org
//============================================================================== 
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Net;
namespace XmlEntityFramework
{
    /// <summary>
    /// Interface IXmlEntityProvider
    /// </summary>
    public interface IXmlEntityProvider
    {
        /// <summary>
        /// 从网络路径或者本地路径读取XML实体.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlOrLocalPath">The URL or local path.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        List<T> Read<T>(string urlOrLocalPath, bool forceRefreshCache = false) where T : class;

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        List<T> Read<T>(MemoryStream stream, bool forceRefreshCache = false) where T : class;

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        List<T> Read<T>(MemoryStream stream, Encoding encoding, bool forceRefreshCache = false) where T : class;

        /// <summary>
        /// Read the specified URL or local path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlOrLocalPath">The URL or local path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        List<T> Read<T>(string urlOrLocalPath, Encoding encoding, bool forceRefreshCache = false) where T : class;

        /// <summary>
        /// Read the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        List<T> Read<T>(string url, HttpMethodType method, bool forceRefreshCache = false) where T : class;

        /// <summary>
        /// Reads the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="paras">The paras.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        List<T> Read<T>(string url, NameValueCollection paras, bool forceRefreshCache = false) where T : class;

        /// <summary>
        /// Reads the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        List<T> Read<T>(string url, WebHeaderCollection headers, bool forceRefreshCache = false) where T : class;

        /// <summary>
        /// Reads the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="paras">The paras.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        List<T> Read<T>(string url, WebHeaderCollection headers, NameValueCollection paras, bool forceRefreshCache = false) where T : class;

        /// <summary>
        /// Read the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="paras">The paras.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        List<T> Read<T>(string url, HttpMethodType method, NameValueCollection paras, bool forceRefreshCache = false) where T : class;

        /// <summary>
        /// Reads the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        List<T> Read<T>(string url, HttpMethodType method, WebHeaderCollection headers, bool forceRefreshCache = false) where T : class;

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
        List<T> Read<T>(string url, HttpMethodType method, WebHeaderCollection headers, NameValueCollection paras, bool forceRefreshCache = false) where T : class;

        /// <summary>
        /// Read the specified URL or local path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL or local path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="method">The method.</param>
        /// <param name="forceRefreshCache">if set to <c>true</c> [force refresh cache].</param>
        /// <returns>List{``0}.</returns>
        List<T> Read<T>(string url, Encoding encoding, HttpMethodType method, bool forceRefreshCache = false) where T : class;

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
        List<T> Read<T>(string url, Encoding encoding, HttpMethodType method, WebHeaderCollection headers, bool forceRefreshCache = false) where T : class;

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
        List<T> Read<T>(string url, Encoding encoding, HttpMethodType method, NameValueCollection paras, bool forceRefreshCache = false) where T : class;

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
        List<T> Read<T>(string url, Encoding encoding, HttpMethodType method, WebHeaderCollection headers, NameValueCollection paras, bool forceRefreshCache = false) where T : class;
    }
}

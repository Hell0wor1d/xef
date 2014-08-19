//==============================================================================  
//Copyright (C) 2012-2015 9UN.ORG. All rights reserved. 
//GUID：5ac265b4-675b-47dd-a3a3-f6274d58d26e
//CLR Version: 4.0.30319.18010
//Code Author：Kevin Wang
//Contact：Email(Admin@9un.org),QQ(265382 or 74344)
//Filename：XmlEntityAttribute
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
//Created by Kevin Wang at 2013/1/1 0:16:58 http://blog.9un.org
//============================================================================== 

using System;

namespace XmlEntityFramework
{
    /// <summary>
    /// Class XmlEntityAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class XmlEntityAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEntityAttribute" /> class.
        /// </summary>
        /// <param name="mappingNodeName">Name of the mapping node.</param>
        /// <param name="baseNodeXPath">The base node X path.</param>
        /// <param name="xmlEntityFlag">The XML entity flag.</param>
        /// <exception cref="System.Exception">节点名称不能为空。</exception>
        public XmlEntityAttribute(string mappingNodeName, string baseNodeXPath, XmlEntityFlags xmlEntityFlag)
        {
            if (string.IsNullOrEmpty(mappingNodeName.Trim()))
            {
                throw new Exception("映射的节点名称不能为空。");
            }
            //设置节点的映射名称
            Name = mappingNodeName;
            //设置节点的标记
            XmlEntityFlag = xmlEntityFlag;
            switch (xmlEntityFlag)
            {
                case XmlEntityFlags.Base:
                case XmlEntityFlags.Base | XmlEntityFlags.Multiple:
                case XmlEntityFlags.Base | XmlEntityFlags.Single:
                case XmlEntityFlags.Nested | XmlEntityFlags.Base:
                case XmlEntityFlags.Nested | XmlEntityFlags.Base | XmlEntityFlags.Multiple:
                case XmlEntityFlags.Nested | XmlEntityFlags.Base | XmlEntityFlags.Single:
                case XmlEntityFlags.Nested:
                case XmlEntityFlags.Nested | XmlEntityFlags.Single:
                case XmlEntityFlags.Nested | XmlEntityFlags.Multiple:
                    XPath = CheckXPath(baseNodeXPath);
                    break;
                default:
                    throw new Exception("不合法的实体类型标记或者标记组合，标记：" + xmlEntityFlag);
            }
        }

        /// <summary>
        /// Checks the X path.
        /// </summary>
        /// <param name="baseNodeXPath">The base node X path.</param>
        /// <returns>System.String.</returns>
        protected string CheckXPath(string baseNodeXPath)
        {
            string res;
            if (string.IsNullOrEmpty(baseNodeXPath.Trim()))
            {
                //如果未设置根节点的XPATH则创建默认的XPATH
                res = "/" + Name;
                return res;
            }
            if (baseNodeXPath.Trim().Equals("/"))
            {
                res = baseNodeXPath + Name;
                return res;
            }
            if (!baseNodeXPath.StartsWith("/"))
            {
                if (baseNodeXPath.EndsWith("/"))
                {
                    res = "/" + baseNodeXPath + Name;
                    return res;
                }
                res = "/" + baseNodeXPath + "/" + Name;
                return res;
            }
            if (baseNodeXPath.EndsWith("/"))
            {
                res = baseNodeXPath + Name;
                return res;
            }
            res = baseNodeXPath + "/" + Name;
            return res;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEntityAttribute" /> class.
        /// </summary>
        /// <param name="mappingNodeName">Name of the mapping node.</param>
        /// <param name="xmlEntityType">Type of the XML entity.</param>
        /// <exception cref="System.Exception">节点名称不能为空。</exception>
        public XmlEntityAttribute(string mappingNodeName, XmlEntityFlags xmlEntityType)
            : this(mappingNodeName, string.Empty, xmlEntityType)
        {

        }

        /// <summary>
        /// Gets or sets the X path.
        /// </summary>
        /// <value>The X path.</value>
        public string XPath { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        public XmlEntityFlags XmlEntityFlag { get; private set; }
    }
}

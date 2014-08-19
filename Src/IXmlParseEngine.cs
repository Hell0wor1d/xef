//==============================================================================  
//Copyright (C) 2012-2015 9UN.ORG. All rights reserved. 
//GUID：4b9100e4-0ab7-4956-a563-e090ec3842c1
//CLR Version: 4.0.30319.18010
//Code Author：Kevin Wang
//Contact：Email(Admin@9un.org),QQ(265382 or 74344)
//Filename：IXmlParseEngine
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
//Created by Kevin Wang at 2012/12/31 23:17:24 http://blog.9un.org
//============================================================================== 
using System;
using System.Xml;

namespace XmlEntityFramework
{
    /// <summary>
    /// XML处理引擎接口
    /// </summary>
    public interface IXmlParseEngine
    {
        /// <summary>
        /// Parses the specified XML content.
        /// </summary>
        /// <param name="xmlContent">Content of the XML.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>List{``0}.</returns>
        [XmlParseMethod(XmlParseMethodType.Default)]
        object Parse(string xmlContent, Type entityType);

        /// <summary>
        /// Parses the specified XML node.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>System.Object.</returns>
        [XmlParseMethod(XmlParseMethodType.Normal)]
        object Parse(XmlNode xmlNode, Type entityType);

        /// <summary>
        /// Parses the specified XML doc.
        /// </summary>
        /// <param name="xmlDoc">The XML doc.</param>
        /// <param name="xmlEntityXPath">The XML entity X path.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>Object[].</returns>
        [XmlParseMethod(XmlParseMethodType.Normal)]
        Object[] Parse(XmlNode xmlDoc, string xmlEntityXPath, Type entityType);
    }
}

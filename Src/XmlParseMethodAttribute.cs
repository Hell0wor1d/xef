﻿//==============================================================================  
//Copyright (C) 2012-2015 9UN.ORG. All rights reserved. 
//GUID：c8e1fa14-3a4c-4932-a02f-9ca69f7f92b8
//CLR Version: 4.0.30319.18010
//Code Author：Kevin Wang
//Contact：Email(Admin@9un.org),QQ(265382 or 74344)
//Filename：XmlParse
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
//Created by Kevin Wang at 2013/1/1 15:48:19 http://blog.9un.org
//============================================================================== 
using System;

namespace XmlEntityFramework
{
    /// <summary>
    /// Class XmlParseMethodAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class XmlParseMethodAttribute :Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlParseMethodAttribute" /> class.
        /// </summary>
        /// <param name="xmlParseMethodType">Type of the XML parse method.</param>
        public XmlParseMethodAttribute(XmlParseMethodType xmlParseMethodType)
        {
            MethodType = xmlParseMethodType;
        }

        /// <summary>
        /// Gets the type of the method.
        /// </summary>
        /// <value>The type of the method.</value>
        public XmlParseMethodType MethodType { get; private set; }
    }
}

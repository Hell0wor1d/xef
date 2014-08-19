//****************************************************************************************************
//Author:          Jun Wang
//Guid:		       2fa4bfee-921a-4e58-8e48-d869ae80d0f6
//DateTime:        1/5/2013 11:15:54 AM
//Email Address:   edu-wangjun@gedu.org
//FileName:        XmlEntityNodeRegexAttribute
//CLR Version:     4.0.30319.18010
//Machine Name:    META-DESKTOP
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
//Created by Kevin Wang at 1/5/2013 11:15:54 AM http://blog.9un.org
//****************************************************************************************************
using System;
using System.Text.RegularExpressions;

namespace XmlEntityFramework
{
    /// <summary>
    /// XmlEntityNodeRegexAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class XmlEntityNodeRegexAttribute: Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEntityNodeRegexAttribute" /> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="regexOptions">The regex options.</param>
        public XmlEntityNodeRegexAttribute(string pattern, RegexOptions regexOptions)
        {
            Regex = new Regex(pattern, regexOptions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEntityNodeRegexAttribute" /> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public XmlEntityNodeRegexAttribute(string pattern)
        {
            Regex = new Regex(pattern);
        }

        /// <summary>
        /// Gets the regex.
        /// </summary>
        /// <value>
        /// The regex.
        /// </value>
        public Regex Regex { get; private set; }
    }
}

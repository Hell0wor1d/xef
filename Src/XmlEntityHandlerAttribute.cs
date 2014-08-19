//==============================================================================  
//Copyright (C) 2012-2015 9UN.ORG. All rights reserved. 
//GUID：65182464-e4a2-4f65-b9c9-0eb92f97a975
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
//Created by Kevin Wang at 2012/12/31 21:38:22 http://blog.9un.org
//============================================================================== 
using System;

namespace XmlEntityFramework
{
    /// <summary>
    /// Class XmlEntityHandlerAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class XmlEntityHandlerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEntityHandlerAttribute" /> class.
        /// </summary>
        /// <param name="parseEngineDelegateType">Type of the parse engine delegate.</param>
        /// <param name="parseEngineDelegateName">Name of the parse engine delegate.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <exception cref="System.Exception">Xml处理引擎必须实现接口：IXmlParseEngine。</exception>
        public XmlEntityHandlerAttribute(Type parseEngineDelegateType, string parseEngineDelegateName, Type entityType)
        {
            EntityType = entityType;
            var obj = Activator.CreateInstance(parseEngineDelegateType) as IXmlParseEngine;
            if (obj == null)
            {
                throw new Exception("Xml processing engine must implement the interface: IXmlParseEngine.");
            }
            EntityParser = (XmlEntityHandler)Delegate.CreateDelegate(typeof(XmlEntityHandler), obj, parseEngineDelegateName);
            //EntityParser = (XmlEntityHandler)Delegate.CreateDelegate(typeof(XmlEntityHandler), delegateType, delegateName);
            //EntityParser = (EntityHandler)Delegate.CreateDelegate(delegateType, delegateType.GetMethod(delegateName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEntityHandlerAttribute" /> class.
        /// </summary>
        /// <param name="parseEngineDelegateType">Type of the parse engine delegate.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <exception cref="System.Exception">接口不能包含O个方法。</exception>
        public XmlEntityHandlerAttribute(Type parseEngineDelegateType, Type entityType)
        {
            EntityType = entityType;
            var obj = Activator.CreateInstance(parseEngineDelegateType) as IXmlParseEngine;
            if (obj == null)
            {
                throw new Exception("Xml processing engine must implement the interface: IXmlParseEngine. Object: " + parseEngineDelegateType);
            }
            var mtds = typeof(IXmlParseEngine).GetMethods();
            if (mtds.Length <= 0x00)
            {
                throw new Exception("Interface can not contain 0 methods. Type: " + parseEngineDelegateType);
            }
            var bindMethodName = string.Empty;
            var bindCount = 0;
            foreach (var methodInfo in mtds)
            {
                if (bindCount > 1)
                {
                    //保证有且仅有一个方法被标记为默认处理方法，否则抛出异常
                    throw new Exception("The interface must have one and only one method marked XmlParseMethodType.Default. Type: " + parseEngineDelegateType);
                }
                var methodAttribute = (XmlParseMethodAttribute)GetCustomAttribute(methodInfo, (typeof(XmlParseMethodAttribute)));
                if (methodAttribute.MethodType != XmlParseMethodType.Default) continue;
                //绑定遍历到得第一个包含XmlParseMethodType.Default属性的方法
                bindMethodName = methodInfo.Name;
                //计数器
                bindCount++;
            }
            if (string.IsNullOrEmpty(bindMethodName))
            {
                throw new Exception("Unable to find any binding method, make sure that the interface has been defined in the method, and the method has been label XmlParseMethodAttribute properties, there are methods and a property defined as XmlParseMethodType.Default");
            }
            EntityParser = (XmlEntityHandler)Delegate.CreateDelegate(typeof(XmlEntityHandler), obj, bindMethodName);
        }

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        public XmlEntityHandler EntityParser { get; private set; }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public Type EntityType { get; private set; }
    }
}

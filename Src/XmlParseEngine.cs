//==============================================================================  
//Copyright (C) 2012-2015 9UN.ORG. All rights reserved. 
//GUID：b783d8b9-de8b-44f8-b9ec-e29017ebcacb
//CLR Version: 4.0.30319.18010
//Code Author：Kevin Wang
//Contact：Email(Admin@9un.org),QQ(265382 or 74344)
//Filename：XmlParseEngine
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
//Created by Kevin Wang at 2012/12/31 23:13:12 http://blog.9un.org
//============================================================================== 

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace XmlEntityFramework
{
    /// <summary>
    /// Class XmlParseEngine
    /// </summary>
    public class XmlParseEngine : IXmlParseEngine
    {
        /// <summary>
        /// Parses the specified XML content.
        /// </summary>
        /// <param name="xmlContent">Content of the XML.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>List{``0}.</returns>
        /// <exception cref="System.Xml.XmlException">无效的Xml格式， + ex.Message</exception>
        public virtual object Parse(string xmlContent, Type entityType)
        {
            var xmlDoc = new XmlDocument();
            try
            {
                //移除命名空间
                xmlContent = Regex.Replace(xmlContent, @"(xmlns:?[^=]*=[""][^""]*[""])", string.Empty,
                                           RegexOptions.IgnoreCase | RegexOptions.Multiline);
                xmlDoc.LoadXml(xmlContent);
            }
            catch (Exception ex)
            {
                throw new XmlException("无效的Xml格式，" + ex.Message);
            }
            var xmlEntityAttribute = XmlEntityUtility.GetXmlEntityAttribute(entityType);
            switch (xmlEntityAttribute.XmlEntityFlag)
            {
                case XmlEntityFlags.Base | XmlEntityFlags.Multiple:
                case XmlEntityFlags.Base | XmlEntityFlags.Nested | XmlEntityFlags.Multiple:
                case XmlEntityFlags.Nested | XmlEntityFlags.Multiple:
                    //提取节点列表
                    return Parse(xmlDoc, xmlEntityAttribute.XPath, entityType);
                case XmlEntityFlags.Base:
                case XmlEntityFlags.Base | XmlEntityFlags.Single:
                case XmlEntityFlags.Nested:
                case XmlEntityFlags.Nested | XmlEntityFlags.Single:
                case XmlEntityFlags.Base | XmlEntityFlags.Nested | XmlEntityFlags.Single:
                    //提取单个节点
                    var xmlNode = xmlDoc.SelectSingleNode(xmlEntityAttribute.XPath);
                    return xmlNode == null ? null : Parse(xmlNode, entityType);
            }
            return null;
        }

        /// <summary>
        /// Extracts the item.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>System.Object.</returns>
        public virtual object Parse(XmlNode xmlNode, Type entityType)
        {
            if (xmlNode == null)
            {
                return null;
            }
            //创建实体
            var entity = Activator.CreateInstance(entityType);
            var pros = entityType.GetProperties();
            foreach (var propertyInfo in pros)
            {
                //获取属性的特性 
                var proAttribute =
                    (XmlEntityNodeAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(XmlEntityNodeAttribute));
                if (proAttribute == null)
                {
                    //忽略未标注特性的属性 
                    continue;
                }
                string value = null;
                //需要搜索根节点的某个属性值
                if (proAttribute.NodeAttributeType == XmlEntityNodeFlags.RootNodeAttribute)
                {
                    if (xmlNode.Attributes == null) continue;
                    var arrRootConllection = xmlNode.Attributes[proAttribute.Name];
                    if (arrRootConllection != null)
                    {
                        value = arrRootConllection.Value;
                    }
                    XmlEntityUtility.SetEntityValue(entity, value, propertyInfo);
                    continue;
                }
                //需要搜索子节点的某个属性值
                if (proAttribute.NodeAttributeType == XmlEntityNodeFlags.SubNodeAttribute)
                {
                    //通过XPath搜索节点
                    var selectSingleNode = xmlNode.SelectSingleNode(proAttribute.XPath);
                    if (selectSingleNode == null) continue;
                    if (selectSingleNode.Attributes == null) continue;
                    var arrSubConllection = selectSingleNode.Attributes[proAttribute.Name];
                    if (arrSubConllection != null)
                    {
                        value = arrSubConllection.Value;
                    }
                    XmlEntityUtility.SetEntityValue(entity, value, propertyInfo);
                    continue;
                }
                //if (proAttribute.NodeAttributeType != XmlEntityNodeFlags.NormalNode)
                //{
                //    throw new Exception("未知的XmlEntity的节点，无法处理。数据：" + proAttribute.NodeAttributeType);
                //}
                if (proAttribute.NodeAttributeType != XmlEntityNodeFlags.NormalNode) continue;
                //正常搜索节点
                //判断节点的特性
                var proType = propertyInfo.PropertyType;
                //判断是否是嵌套节点
                if (!XmlEntityUtility.IsNestedNode(proType))
                {
                    //通过XPath搜索节点
                    var selectSingleNode = xmlNode.SelectSingleNode(proAttribute.XPath);
                    if (selectSingleNode == null) continue;
                    value = selectSingleNode.InnerText;
                    XmlEntityUtility.SetEntityValue(entity, value, propertyInfo);
                    continue;
                }
                //尝试获取嵌套节点的元素的实体类型
                var kwnAttribute = (XmlEntityKnownTypeAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(XmlEntityKnownTypeAttribute));
                if (kwnAttribute == null)
                {
                    //忽略未标注XmlEntityKnownTypeAttribute特性的属性
                    continue;
                }
                //检查是否有嵌套列表 
                if (XmlEntityUtility.IsNestedSingleNode(proType))
                {
                    //嵌套的单节点处理 
                    var node = xmlNode.SelectSingleNode(proAttribute.XPath);
                    //循环到嵌套的实体子节点
                    var valueObject = Parse(node, kwnAttribute.KnownType);
                    XmlEntityUtility.SetEntityValue(entity, valueObject, propertyInfo, kwnAttribute.KnownType);
                    continue;
                }
                //嵌套的多节点处理 
                var nodes = xmlNode.SelectNodes(proAttribute.XPath);
                if (nodes == null)
                {
                    continue;
                }
                var valueObjectList = new List<Object>();
                foreach (XmlNode node in nodes)
                {
                    //当前映射节点名称与待处理的节点名称相同时则开始处理该节点属性
                    if (node.Name == proAttribute.Name)
                    {
                        var subEntity = Parse(node, kwnAttribute.KnownType);
                        //添加实体到列表
                        if (subEntity != null)
                        {
                            valueObjectList.Add(subEntity);
                        }
                        continue;
                    }
                    //有子节点处理
                    var valueObject = Parse(node, proAttribute.Name, kwnAttribute.KnownType);
                    foreach (var item in valueObject)
                    {
                        valueObjectList.Add(item);
                    }
                }
                var valueObjects = valueObjectList.Count <= 0 ? null : valueObjectList.ToArray();
                XmlEntityUtility.SetEntityValue(entity, valueObjects, propertyInfo, kwnAttribute.KnownType);
            }
            return entity;
        }

        /// <summary>
        /// Extracts the list.
        /// </summary>
        /// <param name="xmlDoc">The XML doc.</param>
        /// <param name="xmlEntityXPath">The XML entity X path.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>List{Object}.</returns>
        /// <exception cref="System.Exception"></exception>
        public virtual Object[] Parse(XmlNode xmlDoc, string xmlEntityXPath, Type entityType)
        {
            var result = new List<Object>();
            var xmlNodeList = xmlDoc.SelectNodes(xmlEntityXPath);
            if (xmlNodeList != null)
            {
                //遍历子节点
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    var subEntity = Parse(xmlNode, entityType);
                    //添加实体到列表
                    if (subEntity != null)
                    {
                        result.Add(subEntity);
                    }
                }
            }
            return result.ToArray();
        }
    }
}

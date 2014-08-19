//==============================================================================  
//Copyright (C) 2012-2015 9UN.ORG. All rights reserved. 
//GUID：703214d7-5ce4-4ed9-a161-177e41c293d7
//CLR Version: 4.0.30319.18010
//Code Author：Kevin Wang
//Contact：Email(Admin@9un.org),QQ(265382 or 74344)
//Filename：XmlEntityHelper
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
//Created by Kevin Wang at 2013/1/1 3:23:21 http://blog.9un.org
//============================================================================== 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace XmlEntityFramework
{
    /// <summary>
    /// Class XmlEntityHelper
    /// </summary>
    public static class XmlEntityUtility
    {
        /// <summary>
        /// Gets the entity member info.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>MemberInfo.</returns>
        /// <exception cref="System.Exception">类型未继承BaseXmlEntity或该类型（父类）需未标注属性：XmlEntityHandlerAttribute</exception>
        public static XmlEntityHandlerAttribute GetXmlEntityHandlerAttribute(Type entityType)
        {
            MemberInfo info = entityType;
            var xmlEntityHandlerAttribute = (XmlEntityHandlerAttribute)Attribute.GetCustomAttribute(info, typeof(XmlEntityHandlerAttribute));
            if (xmlEntityHandlerAttribute == null)
            {
                throw new Exception("The type is not inherited to BaseXmlEntity or Unlabeled property of the type (parent class)：XmlEntityHandlerAttribute");
            }
            return xmlEntityHandlerAttribute;
        }

        /// <summary>
        /// Gets the XML entity attribute.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>XmlEntityHandlerAttribute.</returns>
        /// <exception cref="System.Exception">该类型（父类）需未标注属性：XmlEntityAttribute</exception>
        public static XmlEntityAttribute GetXmlEntityAttribute(Type entityType)
        {
            MemberInfo info = entityType;
            var xmlEntityAttribute = (XmlEntityAttribute)Attribute.GetCustomAttribute(info, typeof(XmlEntityAttribute));
            if (xmlEntityAttribute == null)
            {
                throw new Exception("Unlabeled property of the type (parent class). XmlEntityAttribute");
            }
            return xmlEntityAttribute;
        }

        /// <summary>
        /// Gets the XML entity attribute.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>XmlEntityHandlerAttribute.</returns>
        /// <exception cref="System.Exception">该类型（父类）需未标注属性：XmlEntityAttribute</exception>
        public static XmlEntityKnownTypeAttribute GetXmlEntityKnownTypeAttribute(Type entityType)
        {
            MemberInfo info = entityType;
            var xmlEntityKnownTypeAttribute = (XmlEntityKnownTypeAttribute)Attribute.GetCustomAttribute(info, typeof(XmlEntityKnownTypeAttribute));
            if (xmlEntityKnownTypeAttribute == null)
            {
                throw new Exception("Unlabeled property of the type (parent class). XmlEntityKnownTypeAttribute");
            }
            return xmlEntityKnownTypeAttribute;
        }

        /// <summary>
        /// Gets the XML entity cache attribute.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>XmlEntityCacheAttribute.</returns>
        public static XmlEntityCacheAttribute GetXmlEntityCacheAttribute(Type entityType)
        {
            MemberInfo info = entityType;
            var xmlEntityCacheAttribute = (XmlEntityCacheAttribute)Attribute.GetCustomAttribute(info, typeof(XmlEntityCacheAttribute));
            return xmlEntityCacheAttribute;
        }


        /// <summary>
        /// Determines whether [is nested node] [the specified type].
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns><c>true</c> if [is nested node] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsNestedNode(Type targetType)
        {
            var res = !(!targetType.IsArray && (targetType == typeof(string) || (!targetType.IsClass && !targetType.IsInterface)));
            return res;
        }

        /// <summary>
        /// Determines whether [is nested single node].
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns><c>true</c> if [is nested single node]; otherwise, <c>false</c>.</returns>
        public static bool IsNestedSingleNode(Type targetType)
        {
            var res = !targetType.IsArray && !targetType.IsGenericType && !targetType.IsGenericTypeDefinition;
            return res;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="knownTypeofNestedEntity">The known type of nested entity.</param>
        /// <exception cref="System.Exception">需要设置嵌套属性的类型，类型： + propertyInfo.PropertyType</exception>
        public static void SetEntityValue(object entity, object value, PropertyInfo propertyInfo, Type knownTypeofNestedEntity = null)
        {
            if (entity == null || value == null)
            {
                return;
            }
            //var entityType = entity.GetType();
            var propertyType = propertyInfo.PropertyType;
            //var propertyName = propertyInfo.Name;
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;
            //如果是值类型
            if (propertyType.IsValueType)
            {
                if (string.IsNullOrEmpty(value.ToString().Trim()))
                {
                    //空值不处理
                    return;
                }
                value = Convert.ChangeType(value, propertyType);
                propertyInfo.SetValue(entity, value, flags, Type.DefaultBinder, null, CultureInfo.CurrentCulture);
                //entityType.InvokeMember(propertyName, flags, Type.DefaultBinder, entity, new[] { value });
                return;
            }
            //属性为数组或者泛型
            if (propertyType.IsArray || propertyType.IsGenericType)
            {
                var objs = GetArrayFormValue(value);
                if (objs == null || objs.Length <= 0)
                {
                    //无法将传入的参数转为数组时或者数组长度为空时，直接返回
                    return;
                }
                if (knownTypeofNestedEntity == null)
                {
                    //如果未设置嵌套类型的已知类型，则尝试获取
                    knownTypeofNestedEntity = objs.GetValue(0).GetType();
                }
                //为数组时
                if (propertyType.IsArray)
                {
                    //创建数组对象
                    var array = Array.CreateInstance(knownTypeofNestedEntity, objs.Length);
                    //复制对象
                    objs.CopyTo(array, 0);
                    propertyInfo.SetValue(entity, array, flags, Type.DefaultBinder, null, CultureInfo.CurrentCulture);
                    //entityType.InvokeMember(propertyName, flags, Type.DefaultBinder, entity, new object[] { array });
                    return;
                }
                object nList;
                if (propertyType.IsInterface || propertyType.IsAbstract)
                {
                    //如果嵌套类型为接口或者抽象类时，创建List<>的泛型实例
                    var listType = typeof(List<>).MakeGenericType(knownTypeofNestedEntity);
                    nList = Activator.CreateInstance(listType);
                }
                else
                {
                    //创建属性实体类型的的实例
                    nList = Activator.CreateInstance(propertyType);
                }
                Type kType;
                var nListType = nList.GetType();
                if (TryGetICollectionElementType(nListType, out kType))
                {
                    //找到Add方法
                    var addMethod = nListType.GetMethod("Add", new[] { kType });
                    //无法找到Add方法时，返回。
                    if (addMethod == null) return;
                    foreach (var o in objs)
                    {
                        //调用Add方法，插入一条数据
                        addMethod.Invoke(nList, new[] { o });
                    }
                    propertyInfo.SetValue(entity, nList, flags, Type.DefaultBinder, null, CultureInfo.CurrentCulture);
                    //entityType.InvokeMember(propertyName, flags, Type.DefaultBinder, entity, new[] { nList });
                    return;
                }
                throw new Exception("Can not get the element type form current collection. Collection Type: " + nListType + ", Property Type：" + propertyType);
            }

            propertyInfo.SetValue(entity, value, flags, Type.DefaultBinder, null, CultureInfo.CurrentCulture);
            //entityType.InvokeMember(propertyName, flags, Type.DefaultBinder, entity, new[] { value });
        }

        /// <summary>
        /// Tries to get array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Array.</returns>
        /// <exception cref="System.Exception">无法找到ToArray方法，类型： + vType</exception>
        public static Array GetArrayFormValue(object value)
        {
            Array array = null;
            var vType = value.GetType();
            if (vType.IsArray)
            {
                array = value as Array;
                return array;
            }
            if (vType.IsGenericType)
            {
                //找到ToArray方法
                var toArray = vType.GetMethod("ToArray");
                if (toArray != null)
                {
                    array = toArray.Invoke(value, null) as Array;
                }
                else
                {
                    throw new Exception("Can not find the method: ToArray() from the target type, Type is: " + vType);
                }
            }
            return array;
        }

        /// <summary>
        /// Tries the type of the get I collection element.
        /// </summary>
        /// <param name="collectionType">Type of the collection.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public static bool TryGetICollectionElementType(Type collectionType, out Type elementType)
        {
            elementType = null;
            // We have to check if the type actually is the interface, or if it implements the interface:
            try
            {
                var collectionInterface =
                    (collectionType.IsGenericType && typeof(ICollection<>).IsAssignableFrom(collectionType.GetGenericTypeDefinition()))
                        ? collectionType
                        : collectionType.GetInterface(typeof(ICollection<>).FullName);

                // We need to make sure the type is fully specified otherwise we won't be able to add element to it.
                if (collectionInterface != null
                    && !collectionInterface.ContainsGenericParameters)
                {
                    elementType = collectionInterface.GetGenericArguments()[0];
                    return true;
                }
            }
            catch (AmbiguousMatchException)
            {
                // Thrown if collection type implements ICollection<> more than once
            }
            return false;
        }

        /// <summary>
        /// Detects the type and match value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public static object DetectAndMatchValue(string value, PropertyInfo propertyInfo)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            //获取数据格式属性
            var proAttribute = (XmlEntityNodeDataFormatAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(XmlEntityNodeDataFormatAttribute));
            if (proAttribute == null)
            {
                //忽略未标注特性的属性 
                return value;
            }
            var dataFormat = proAttribute.DataFormat;
            if (string.IsNullOrEmpty(dataFormat))
            {
                //格式为空忽略
                return value;
            }
            var propertyType = propertyInfo.PropertyType;
            if (typeof(DateTime) == propertyType)
            {
                //尝试提取日期/时间
                object returnObject = DateTime.ParseExact(value, dataFormat, CultureInfo.CurrentCulture);
                return returnObject;
            }
            if (typeof(string) == propertyType)
            {
                //格式化
                value = string.Format(CultureInfo.CurrentCulture, dataFormat, value);
                return value;
            }
            return value;
        }

        ///// <summary>
        ///// Gets the properties.
        ///// </summary>
        ///// <param name="type">The type.</param>
        ///// <param name="baseXPath">The base X path.</param>
        ///// <returns>Dictionary{System.StringSystem.String}.</returns>
        //public static Dictionary<object, object> GetMapping(Type type, string baseXPath ="") 
        //{
        //    var dict = new Dictionary<object, object>();
        //    var xmlEntityAttribute = GetXmlEntityAttribute(type);
        //    switch (xmlEntityAttribute.XmlEntityFlag)
        //    {
        //        case XmlEntityFlags.Base:
        //        case XmlEntityFlags.Base | XmlEntityFlags.Multiple:
        //        case XmlEntityFlags.Base | XmlEntityFlags.Single:
        //             baseXPath = xmlEntityAttribute.XPath;
        //        dict.Add(type.Name, baseXPath);
        //            break;
        //    }
        //    var pros = type.GetProperties();
        //    foreach (var propertyInfo in pros)
        //    {
        //        //获取属性的特性
        //        var proAttribute =
        //            (XmlEntityNodeAttribute) Attribute.GetCustomAttribute(propertyInfo, typeof (XmlEntityNodeAttribute));
        //        if (proAttribute == null)
        //        {
        //            //忽略未标注特性的属性
        //            continue;
        //        }
        //        if (IsNestedNode(propertyInfo.PropertyType))
        //        {
        //            var kwnAttribute =
        //                (XmlEntityKnownTypeAttribute)
        //                Attribute.GetCustomAttribute(propertyInfo, typeof (XmlEntityKnownTypeAttribute));
        //            if (kwnAttribute == null)
        //            {
        //                //忽略未标注XmlEntityKnownTypeAttribute特性的属性
        //                continue;
        //            }
        //            var dic = GetMapping(kwnAttribute.KnownType, baseXPath);
        //            foreach (var o in dic)
        //            {
        //                dict.Add("[" + propertyInfo.Name + "]" + o.Key, o.Value);
        //            }
        //            continue;
        //        }
        //        dict.Add(propertyInfo.Name, baseXPath + "/" + proAttribute.XPath);
        //    }
        //    return dict;
        //}
    }
}

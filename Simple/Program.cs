//==============================================================================  
//Copyright (C) 2012-2015 9UN.ORG. All rights reserved. 
//GUID：e3ceafb3-287d-4082-a41e-d3b40387877e
//CLR Version: 4.0.30319.18010
//Code Author：Kevin Wang
//Contact：Email(Admin@9un.org),QQ(265382 or 74344)
//Filename：Simple
//Namespace：Simple
//Functions：Program  
//Created by Kevin Wang at 2012/12/31 21:35:50 http://blog.9un.org
//============================================================================== 

using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using XmlEntityFramework;

namespace Simple
{
    //General Xml To Entity ORM framework V1.0
    //Author: Kevin Wang
    //E-mail: admin@9un.org
    //Main functions:
    //  generic XML processing engine. XmlParseEngine;
    //  custom processing engines. Customizable interface IXmlParseEngine;
    //  mapping XML to the entity object. ORM;
    //  dynamic binding XML entity type. DOB;
    //  flexible set of XPATH mode.
    //  the entity can set the cache automatically controlled entity lifecycle. Entity Cache;
    //  dozen overloaded read mode, memory stream, remote files, local files. Read override;
    //  support a variety of complex XML nested. Support Xml Nested;
    //  to support the safe operation of the multi-threading. Thread-Safe;
    //  support for REST-style HTTP read.
    //  compatible. NET 2.0 (including 2.0) or later.
    class Program
    {
        # region custom engine from the interface
        /// Implement custom XML processing engine, you need to implement the interface IXmlParseEngine or inheritance XmlParseEngine.
        /// of course, you can use the internal implementation of the method.
        /// For example [XmlEntityHandler (typeof (XmlParseEngineSimple), typeof (BaseXmlEntity))]
        /// Otherwise default internal processing engine
        public class XmlParseEngineSimple : XmlParseEngine
        {
            /// <summary>
            /// Parses the specified XML content.
            /// </summary>
            /// <param name="xmlContent">Content of the XML.</param>
            /// <param name="entityType">Type of the entity.</param>
            /// <returns>List{``0}.</returns>
            public override object Parse(string xmlContent, Type entityType)
            {
                //Custom processing logic
                return base.Parse(xmlContent, entityType);
            }

            /// <summary>
            /// Extracts the list.
            /// </summary>
            /// <param name="xmlDoc">The XML doc.</param>
            /// <param name="xmlEntityXPath">The XML entity X path.</param>
            /// <param name="entityType">Type of the entity.</param>
            /// <returns>List{Object}.</returns>
            public override object[] Parse(XmlNode xmlDoc, string xmlEntityXPath, Type entityType)
            {
                //Custom processing logic
                return base.Parse(xmlDoc, xmlEntityXPath, entityType);
            }

            /// <summary>
            /// Extracts the item.
            /// </summary>
            /// <param name="xmlNode">The XML node.</param>
            /// <param name="entityType">Type of the entity.</param>
            /// <returns>System.Object.</returns>
            public override object Parse(XmlNode xmlNode, Type entityType)
            {
                //Custom processing logic
                return base.Parse(xmlNode, entityType);
            }
        }
        #endregion

        #region Defined your entity object(no nested), demo.xml
        //Can use custom interface engine
        //[XmlEntityHandler(typeof(XmlParseEngineSimple), typeof(Book))]
        [XmlEntity("book", "/bookstore", XmlEntityFlags.Base | XmlEntityFlags.Multiple)]
        //Cache characteristics, life cycle (seconds)
        [XmlEntityCache(10)]
        public class Book : BaseXmlEntity
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            [XmlEntityNode("category", "book", XmlEntityNodeFlags.RootNodeAttribute)]
            public string Category { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            [XmlEntityNode("title")]
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the lang.
            /// </summary>
            /// <value>The lang.</value>
            [XmlEntityNode("lang", "title", XmlEntityNodeFlags.SubNodeAttribute)]
            public string Lang { get; set; }

            /// <summary>
            /// Gets or sets the author.
            /// </summary>
            /// <value>The author.</value>
            [XmlEntityNode("author")]
            public string Author { get; set; }

            /// <summary>
            /// Gets or sets the book.
            /// </summary>
            /// <value>The book.</value>
            [XmlEntityNode("book")]
            [XmlEntityKnownType(typeof(Book))]
            public Book SubBook { get; set; }

            /// <summary>
            /// Gets or sets the childs.
            /// </summary>
            /// <value>The childs.</value>
            [XmlEntityNode("book", "childs")]
            //Nested xml node. you need to use XmlEntityKnownType.
            [XmlEntityKnownType(typeof(Book))]
            public ICollection<Book> Books { get; set; }

            /// <summary>
            /// Gets or sets the name of the child.
            /// </summary>
            /// <value>The name of the child.</value>
            [XmlEntityNode("name", "childs", XmlEntityNodeFlags.SubNodeAttribute)]
            public string ChildName { get; set; }

            /// <summary>
            /// Gets or sets the year.
            /// </summary>
            /// <value>The year.</value>
            [XmlEntityNode("year")]
            public string Year { get; set; }

            /// <summary>
            /// Gets or sets the price.
            /// </summary>
            /// <value>The price.</value>
            [XmlEntityNode("price")]
            public double Price { get; set; }
        }
        #endregion

        #region Defined your entity object(with xml nested),  demo1.xml
        //Statement principal object
        [XmlEntity("root", "/", XmlEntityFlags.Base | XmlEntityFlags.Single)]
        //Cache characteristics, life cycle (seconds)
        [XmlEntityCache(10)]
        public class Network : BaseXmlEntity
        {
            /// <summary>
            /// Gets or sets the servers.
            /// </summary>
            /// <value>The servers.</value>
            [XmlEntityNode("node")]
            [XmlEntityKnownType(typeof(Location))]
            public Location[] Locations { get; set; }
        }

        //Statement nested object.
        [XmlEntity("node", XmlEntityFlags.Nested | XmlEntityFlags.Multiple)]
        public class Location : BaseXmlEntity
        {
            /// <summary>
            /// Gets or sets the lang.
            /// </summary>
            /// <value>The lang.</value>
            [XmlEntityNode("name", "node", XmlEntityNodeFlags.RootNodeAttribute)]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the servers.
            /// </summary>
            /// <value>The servers.</value>
            [XmlEntityNode("server")]
            [XmlEntityKnownType(typeof(Server))]
            public Server[] Servers { get; set; }
        }

        //Statement nested object.
        [XmlEntity("server", XmlEntityFlags.Nested | XmlEntityFlags.Multiple)]
        public class Server : BaseXmlEntity
        {
            /// <summary>
            /// Gets or sets the lang.
            /// </summary>
            /// <value>The lang.</value>
            [XmlEntityNode("name", "server", XmlEntityNodeFlags.RootNodeAttribute)]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the IP.
            /// </summary>
            /// <value>The IP.</value>
            [XmlEntityNode("ipaddr", "server", XmlEntityNodeFlags.RootNodeAttribute)]
            public string IP { get; set; }

            /// <summary>
            /// Gets or sets the status.
            /// </summary>
            /// <value>The status.</value>
            [XmlEntityNode("route", "server", XmlEntityNodeFlags.RootNodeAttribute)]
            public DateTime Route { get; set; }

            /// <summary>
            /// Gets or sets the status.
            /// </summary>
            /// <value>The status.</value>
            public string Status { get; set; }
        }

        #endregion
        static void Main(string[] args)
        {
            IXmlEntityProvider provider = new XmlEntityProvider();
            for (var i = 0; i < 1; i++)
            {
                //Multi-threaded test
                new Thread(new ThreadStart(
                    () =>
                    {
                        while (true)
                        {
                            //try
                            //{
                            //The node attribute mapping test.
                            //var slist = provider.Read<Network>(AppDomain.CurrentDomain.BaseDirectory + "\\demo1.xml", Encoding.UTF8);
                            // Console.WriteLine("Root data:{0}", slist.Count);
                            //The various nested node mapping test.
                            var clist = provider.Read<Book>(AppDomain.CurrentDomain.BaseDirectory + "\\demo.xml");
                            Console.WriteLine("Book data:{0}", clist.Count);
                            Thread.Sleep(1000);
                            //}
                            //catch (Exception ex)
                            //{
                            //    Console.WriteLine(ex.Message);
                            //    break;
                            //}
                        }
                    }
                    )) { IsBackground = true, Name = "Thread " + i }.Start();
            }
            Console.ReadLine();
        }
    }
}

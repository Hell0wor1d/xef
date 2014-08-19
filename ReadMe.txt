General Xml To Entity ORM framework V1.0
Author: Kevin Wang
E-mail: admin@9un.org
Main functions:

generic XML processing engine. XmlParseEngine;
custom processing engines. Customizable interface IXmlParseEngine;
mapping XML to the entity object. ORM;
dynamic binding XML entity type. DOB;
flexible set of XPATH mode.
the entity can set the cache automatically controlled entity lifecycle. Entity Cache;
dozen overloaded read mode, memory stream, remote files, local files. Read override;
support a variety of complex XML nested. Support Xml Nested;
to support the safe operation of the multi-threading. Thread-Safe;
support for REST-style HTTP read.
compatible. NET 2.0 (including 2.0) or later.

Xml Demo
<?xml version="1.0" encoding="utf-8" ?>
<bookstore>
  <book category="COOKING">
    <title lang="en">Everyday Italian</title>
    <author>Giada De Laurentiis</author>
    <year>2005</year>
    <price>30.00</price>
    <childs name="list test">
      <book category="CHILDREN">
        <title lang="en">Harry Potter</title>
        <author>J K. Rowling</author>
        <year>2005</year>
        <price>29.99</price>
      </book>
      <book category="CHILDREN">
        <title lang="en">Harry Potter</title>
        <author>J K. Rowling</author>
        <year>2005</year>
        <price>29.99</price>
      </book>
    </childs>
  </book>

  <book category="CHILDREN">
    <title lang="en">Harry Potter</title>
    <author>J K. Rowling</author>
    <year>2005</year>
    <price>29.99</price>
    <book category="CHILDREN">
      <title lang="en">Harry Potter</title>
      <author>J K. Rowling</author>
      <year>2005</year>
      <price>29.99</price>
    </book>
  </book>

  <book category="WEB">
    <title lang="en">XQuery Kick Start</title>
    <author>James McGovern</author>
    <author>Per Bothner</author>
    <author>Kurt Cagle</author>
    <author>James Linn</author>
    <author>Vaidyanathan Nagarajan</author>
    <year>2003</year>
    <price>49.99</price>
  </book>

  <book category="WEB">
    <title lang="en">Learning XML</title>
    <author>Erik T. Ray</author>
    <year>2003</year>
    <price>39.95</price>
  </book>
</bookstore>
 
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <node name="download">
    <server name="download3" ipaddr="down3.xxxx.com" route="2012-12-31" load="good"/>
    <server name="download2" ipaddr="down2.xxxx.com" route="2012-12-31" load="good"/>
    <server name="download1" ipaddr="down1.xxxx.com" route="2012-12-31" load="good"/>
  </node>
  <node name="USA">
    <server name="game-03" ipaddr="us13.xxxx.com" route="2012-12-31" load="good"/>
    <server name="game-02" ipaddr="us12.xxxx.com" route="2012-12-31" load="good"/>
    <server name="game-01" ipaddr="us11.xxxx.com" route="2012-12-31" load="good"/>
    <server name="game-03-01" ipaddr="us16.xxxx.com" route="2012-12-31" load="good"/>
    <server name="game-02-01" ipaddr="us15.xxxx.com" route="2012-12-31" load="good"/>
    <server name="game-01" ipaddr="us14.xxxx.com" route="2012-12-31" load="good"/>
    <server name="video-03" ipaddr="us9.xxxx.com" route="2012-12-31" load="good"/>
    <server name="video-023" ipaddr="us8.xxxx.com" route="2012-12-31" load="good"/>
    <server name="video-011" ipaddr="us7.xxxx.com" route="2012-12-31" load="good"/>
    <server name="video-033" ipaddr="us6.xxxx.com" route="2012-12-31" load="good"/>
    <server name="video-02" ipaddr="us5.xxxx.com" route="2012-12-31" load="good"/>
    <server name="video-011" ipaddr="us4.xxxx.com" route="2012-12-31" load="good"/>
    <server name="video-03" ipaddr="us3.xxxx.com" route="2012-12-31" load="good"/>
    <server name="video-021" ipaddr="us2.xxxx.com" route="2012-12-31" load="good"/>
    <server name="video-01" ipaddr="us1.xxxx.com" route="2012-12-31" load="good"/>
  </node>
  <node name="china">
    <server name="china-03" ipaddr="jp8.xxxx.com" route="2012-12-31" load="good"/>
    <server name="china-022" ipaddr="jp7.xxxx.com" route="2012-12-31" load="good"/>
    <server name="china-01" ipaddr="jp6.xxxx.com" route="2012-12-31" load="good"/>
    <server name="china-021" ipaddr="jp5.xxxx.com" route="2012-12-31" load="good"/>
    <server name="china-012" ipaddr="jp4.xxxx.com" route="2012-12-31" load="good"/>
    <server name="china-031" ipaddr="jp3.xxxx.com" route="2012-12-31" load="good"/>
    <server name="china-02" ipaddr="jp2.xxxx.com" route="2012-12-31" load="good"/>
    <server name="china-011" ipaddr="jp1.xxxx.com" route="2012-12-31" load="good"/>
  </node>
</root>
Custom engine from the interface
        # region custom engine from the interface
	/// <summary>
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
Defined your entity object(no nested)
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
Defined your entity object(with xml nested)
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
How to use
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
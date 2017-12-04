using System;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using Domains;
using System.IO;

namespace Server
{
	class Program
	{
		private static List<Item> database;

		private static void readDB()
		{
			//preparing
			XmlDataDocument xmlDoc = new XmlDataDocument();
			XmlNodeList xmlNode;
			database = new List<Item>();
			//read 
			FileStream fs = new FileStream(@".\database.xml", FileMode.Open, FileAccess.Read);
			xmlDoc.Load(fs);
			xmlNode = xmlDoc.GetElementsByTagName("Item");
			foreach (XmlNode node in xmlNode)
			{
				Item item = new Item();
				item.Type = node.ChildNodes.Item(0).InnerText.Trim();
				item.Stock = XmlConvert.ToInt32(node.ChildNodes.Item(1).InnerText.Trim());
				item.Price = XmlConvert.ToInt32(node.ChildNodes.Item(2).InnerText.Trim());
			}
		}

		static void Main(string[] args)
		{
			readDB();
		}
	}
}

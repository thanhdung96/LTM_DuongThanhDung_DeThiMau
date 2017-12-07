using System.Net.Sockets;
using System.Collections.Generic;
using Domains;
using System.Net;
using System.Xml;
using System;
using System.Linq;
using System.IO;
using System.Text;

namespace Server
{
	class ShopServer
	{
		private List<Item> database;
		private List<Item> order;

		private const int BUFFER_SIZE = 4096;

		private TcpListener server;
		private TcpClient client;
		private NetworkStream netStream;

		private byte[] dataSend, dataReceive;
		private int dataSize;
		byte signal;

		public ShopServer()
		{
			//populate database into list
			readDB();

			server = new TcpListener(new IPEndPoint(IPAddress.Any, 1724));
			dataSend = new byte[BUFFER_SIZE];
			dataReceive = new byte[BUFFER_SIZE];
		}

		private void readDB()
		{
			//for database
			//preparing
			XmlDataDocument xmlDoc = new XmlDataDocument();
			XmlNodeList xmlNode;
			database = new List<Item>();
			//read 
			FileStream fs = new FileStream(@".\database.xml", FileMode.Open, FileAccess.Read);
			xmlDoc.Load(fs);
			xmlNode = xmlDoc.GetElementsByTagName("Item");
			foreach (XmlNode node in xmlNode)	//parse into list
			{
				Item item = new Item();
				item.Id = XmlConvert.ToInt32(node.ChildNodes.Item(0).InnerText.Trim());
				item.Type = node.ChildNodes.Item(1).InnerText.Trim();
				item.Stock = XmlConvert.ToInt32(node.ChildNodes.Item(2).InnerText.Trim());
				item.Price = XmlConvert.ToInt32(node.ChildNodes.Item(3).InnerText.Trim());
			}

			//for customer order
			order = new List<Item>();
		}

		public void start()
		{
			server.Start();
			client = server.AcceptTcpClient();
			netStream = client.GetStream();

			signal = Convert.ToByte(netStream.ReadByte());
			if (signal == Signal.LIST_REQUEST)		//if client send request for list
			{
				foreach (Item item in database)		//send each item in db to client
				{
					netStream.WriteByte(Signal.LIST_SEND_MORE);		//client continue receiving

					string data = item.ToString();
					dataSend = Encoding.ASCII.GetBytes(data);
					netStream.Write(dataSend, 0, data.Length);
				}
			}
			netStream.WriteByte(Signal.LIST_SEND_FIN);		//signal client finished sending list

			signal = Convert.ToByte(netStream.ReadByte());
			if (signal == Signal.ORDER_RTS)
			{
				signal = Convert.ToByte(netStream.ReadByte());
				while (signal != Signal.ORDER_SEND_FIN)
				{
					if (signal == Signal.ORDER_SEND_MORE)
					{
						dataSize = netStream.Read(dataReceive, 0, BUFFER_SIZE);
						string data = Encoding.ASCII.GetString(dataReceive, 0, dataSize);
						order.Add(Item.GetObject(data));
					}
					signal = Convert.ToByte(netStream.ReadByte());
				}
			}

			if (signal == Signal.PAYMENT_REQUEST)
			{
				int totalPayment = 0;
				foreach (Item item in order)
				{
					totalPayment += database.Where(i => i.Id == item.Id).First().Price * item.Stock;
				}

				dataSend = BitConverter.GetBytes(totalPayment);
				netStream.Write(dataSend, 0, sizeof(int));
			}
		}
	}
}

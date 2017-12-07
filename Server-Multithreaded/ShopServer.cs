using Domains;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace Server_Multithreaded
{
	class ShopServer
	{
		private const int BUFFER_SIZE = 1024;
		private const int MAX_CLIENT = 4;
		private List<Item> database;
		private TcpListener server;

		public ShopServer()
		{
			ReadDB();
			server = new TcpListener(new IPEndPoint(IPAddress.Any, 1724));
			Console.WriteLine("Server initialized components");
		}

		private void ReadDB()
		{
			//for database
			//preparing
			XmlDataDocument xmlDoc = new XmlDataDocument();
			XmlNodeList xmlNode;
			database = new List<Item>();
			try
			{
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
				Console.WriteLine("Server finised reading database");
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Database not found");
			}
		}
		internal void Start()
		{
			server.Start(MAX_CLIENT);		//waiting for 4 clients maximum
			for (int i = 0; i < MAX_CLIENT; i++)
			{
				server.BeginAcceptTcpClient(new AsyncCallback(AcceptClient), server);
			}
		}

		private void AcceptClient(IAsyncResult ar)
		{
			//only receive signals from clients in this method
			TcpListener listener = (TcpListener)ar;
			TcpClient client = listener.EndAcceptTcpClient(ar);
			List<Item> order = new List<Item>();
			NetworkStream receive = client.GetStream();
			byte signal = Signal.NULL;
			while (signal != Signal.DISCONNECT)
			{
				signal = Convert.ToByte(receive.ReadByte());
				if (signal == Signal.LIST_REQUEST)
					SendList(ref client);
				else if (signal == Signal.ORDER_RTS)
					ReceiveOrder(ref client, ref order);
				else if (signal == Signal.PAYMENT_REQUEST)
					SendTotalPayment(ref client, ref order);
				else if (signal == Signal.DISCONNECT)
				{
					receive.Close(); 
					client.Close();
					listener.Stop();
				}
			}
		}

		private void SendTotalPayment(ref TcpClient client, ref List<Item> order)
		{
			NetworkStream netStream = client.GetStream();
			int totalPayment = 0;
			byte[] dataSend = new byte[BUFFER_SIZE];
			foreach (Item item in order)
			{
				totalPayment += database.Where(i => i.Id == item.Id).First().Price * item.Stock;
			}

			dataSend = BitConverter.GetBytes(totalPayment);
			netStream.Write(dataSend, 0, sizeof(int));
			netStream.Close();
		}

		private void ReceiveOrder(ref TcpClient client, ref List<Item> order)
		{
			NetworkStream netStream = client.GetStream();
			int dataSize;
			byte[] dataReceive = new byte[BUFFER_SIZE];
			byte signal = Convert.ToByte(netStream.ReadByte());

			while (signal != Signal.ORDER_SEND_FIN && signal == Signal.ORDER_SEND_MORE)
			{
					dataSize = netStream.Read(dataReceive, 0, BUFFER_SIZE);
					string data = Encoding.ASCII.GetString(dataReceive, 0, dataSize);
					order.Add(Item.GetObject(data));
				signal = Convert.ToByte(netStream.ReadByte());
			}
			netStream.WriteByte(Signal.OK);
			netStream.Close();
		}

		private void SendList(ref TcpClient client)
		{
			NetworkStream send = client.GetStream();
			byte[] sendBuffer = new byte[BUFFER_SIZE];

			foreach (Item item in database)		//send each item in db to client
			{
				send.WriteByte(Signal.LIST_SEND_MORE);		//client continue receiving

				string data = item.ToString();
				sendBuffer = Encoding.ASCII.GetBytes(data);
				send.Write(sendBuffer, 0, data.Length);
			}
			send.WriteByte(Signal.LIST_SEND_FIN);		//signal client finished sending list
			send.Close();
		}
	}
}

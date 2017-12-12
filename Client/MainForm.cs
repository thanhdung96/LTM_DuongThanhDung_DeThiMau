using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Domains;

namespace Client
{
	public partial class MainForm : Form
	{
		private const int BUFFER_SIZE = 1024;
		private List<Item> order, listItems;
		TcpClient client;
		NetworkStream netstream;
		byte[] dataSend, dataReceive;

		public MainForm()
		{
			InitializeComponent();
		}

		private void btnRefresh_Click(object sender, EventArgs e)		//send LIST_REQUEST
		{

		}

		private void btnAdd_Click(object sender, EventArgs e)
		{

		}

		private void btnRemove_Click(object sender, EventArgs e)
		{

		}

		private void btnPay_Click(object sender, EventArgs e)		//send ORDER_RTS then PAYMENT_REQUEST
		{

		}
	}
}

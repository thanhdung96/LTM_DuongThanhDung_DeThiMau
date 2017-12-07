namespace Domains
{
	public class Signal
	{
		public static readonly byte NULL = 0;
		public static readonly byte OK = 1;				//acknowledged
		public static readonly byte DISCONNECT = 2;
		public static readonly byte LIST_SEND_MORE = 10;	//signal client to prepare to receive more items in db
		public static readonly byte LIST_REQUEST = 11;	//client request list of items available
		public static readonly byte LIST_SEND_FIN = 12;	//finished sending list of items available

		public static readonly byte ORDER_RTS = 20;		//client request to send order to server
		public static readonly byte ORDER_SEND_MORE = 21;	//client send order to server, item by item
		public static readonly byte ORDER_SEND_FIN = 22;	//client finished sending order to server

		public static readonly byte PAYMENT_REQUEST = 30;	//client request to know how musch much to pay
	}
}

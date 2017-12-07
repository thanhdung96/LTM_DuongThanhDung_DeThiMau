using Server_Multithreaded;
namespace Server
{
	class Program
	{
		private static ShopServer shop;
		static void Main(string[] args)
		{
			shop = new ShopServer();
			shop.Start();
		}
	}
}

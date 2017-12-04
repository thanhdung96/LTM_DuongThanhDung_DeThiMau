namespace Domains
{
	public class Item
	{
		private int id;
		private string type;
		private int stock;
		private int price;

		public Item()
		{
			this.type = "";
			this.stock = 0;
			this.price = 0;
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Type
		{
			get { return type; }
			set { type = value; }
		}

		public int Stock
		{
			get { return stock; }
			set { stock = value; }
		}

		public int Price
		{
			get { return price; }
			set { price = value; }
		}
	}
}

namespace Domains
{
	using System;

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

		public override string ToString()
		{
			return this.id + ";" + this.type + ";" + this.stock + ";" + this.price;		// 1;Giay;10;100
		}

		public static Item GetObject(string str)
		{
			Item item = new Item();
			item.Id = Convert.ToInt32(str.Split(';')[0]);
			item.Type = str.Split(';')[1];
			item.Stock = Convert.ToInt32(str.Split(';')[2]);
			return item;
		}
	}
}

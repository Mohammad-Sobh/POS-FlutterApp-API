using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using static POS_Data_API.Classes.UserClass;

namespace POS_Data_API.Classes
{
    public class UserClass
    {
        static int id;
        public int UserID { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public string ShopName { get; set; }
        public UserStock Stock { get; set; }

        static UserClass()
        {
            FirebaseResponse response = new FB().firebase.Get("USERS");
            if (response.Body != "\"\"")
                id = response.ResultAs<Dictionary<string, dynamic>>().Count + 10;
            else
                throw new Exception("NO DATA!");
        }


        public UserClass(string phone, string password)
        {
            Phone = phone;
            Password = password;
            ShopName = "";
            Active = false;
            UserID = ++id;
            //test
            Stock = new UserStock();
            //Stock.CreateUserStock();
        }
        public UserClass() { }

        public class UserStock
        {
            public double Cash { get; set; } = 0;
            public List<Bill> Bills { get; set; } = new List<Bill>();
            public List<Sale> Sales { get; set; } = new List<Sale>();
            public List<string> Categories { get; set; } = new List<string>();
            public List<Item> Items { get; set; } = new List<Item>();
            public UserStock() { }
            public void EditItem(int id, string? name, double? price, string? pic, string? category, string? description, string? barcode)
            {
                //category = category.ToLower();
                if (Items.Exists( i => i.Id == id))
                {
                    var index = Items.FindIndex(i => i.Id == id);
                    Items[index] = Items[index].CopyWith(category, name, price, pic, description, barcode);
                }
            }
            public void AddCategory(string name)
            {
                name = name.ToLower();
                if (Categories.Exists(x => x == name))
                    throw new Exception("Cetegory with the same name already added.");
                Categories.Add(name);
            }
            public List<Item> GetCategory(string name) 
            {
                name = name.ToLower();
                List<Item> items = new List<Item>();
                if(name == "all")
                {
                    items.AddRange(Items.FindAll(x => x.Category == "none"));
                    items.AddRange(Items.FindAll(x => x.Category != "none"));
                    return items;
                }
                if (!Categories.Exists(x => x == name))
                    throw new Exception("Cetegory with the same name was not found.");
                items.AddRange(Items.FindAll(x => x.Category.ToLower() == name));
                return items;
            }
            public void AddItem(string name, double price, string pic, string category, string description, string barcode)
            {
                category = category.ToLower();
                if (!Categories.Exists(x => x == category))
                    throw new Exception("Cetegory with the same name was not found.");
                Items.Add(new Item(genarate_id(name, category, price.ToString()),
                    category, name, price, pic, description, barcode));

                //Generate Item ID
                int genarate_id(string n, string c, string p)
                {
                    var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes
                        (n + c + p + DateTime.Now.Ticks.ToString()));

                    var i = Convert.ToHexString(hash).ToLower();
                    i = i.Substring(0, 3);

                    int id = int.Parse(i, System.Globalization.NumberStyles.HexNumber);
                    id = id % 900 + 100;

                    while (Items.Exists(x => x.Id == id))
                        id++;
                    return id;
                }
            }
            public void AddBill(double total, string description)
            {                
                Bill bill = new Bill(Sales.Count + 100, DateTime.UtcNow.AddHours(3).ToString(),total,description);
                Cash -= bill.Total;
                Bills.Add(bill);
            }
            public void AddSale(List<int> id, double discount)
            {
                List<Item> items = new List<Item>();
                foreach (var i in id)
                {
                    items.Add(Items.Find(x => x.Id == i)!);
                }
                Sale sale = new Sale(Sales.Count + 100, DateTime.UtcNow.AddHours(3).ToString(), items, discount);
                Cash += sale.Total;
                Sales.Add(sale);
            }
            public void RemoveCategory(string name)
            {
                name = name.ToLower();
                Categories.Remove(name);
                foreach (Item item in Items)
                {
                    if (item.Category == name) 
                        item.Category = "none";
                }
            }
            public void RemoveItem(int id)
            {
                Items.RemoveAt(Items.FindIndex(x => x.Id == id));
            }
        }


        public class Item
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Barcode { get; set; }
            public double Price { get; set; }
            public string Pic { get; set; }
            public string Category { get; set; }
            public Item(int id, string category, string name, double price, string pic, string description, string barcode)
            {
                Name = name;
                Id = id;
                Price = price;
                Pic = pic;
                Category = category;
                Description = description;
                Barcode = barcode;
            }
            public Item CopyWith(string? category, string? name, double? price, string? pic, string? description, string? barcode)
            {
                return new Item(
                    this.Id,
                    category?.ToLower() ?? this.Category,
                    name ?? this.Name,
                    price ?? this.Price,
                    pic ?? this.Pic,
                    description ?? this.Description,
                    barcode ?? this.Barcode
                    );
            }
            public Item() { }
        }

        public class Sale
        {
            public int Id { get; set; }
            public string Date { get; set; }
            public double Total { get; set; }
            public List<Item> Items { get; set; }
            public double Discount { get; set; }
            public Sale(int id, string date, List<Item> items, double discount)
            {
                Id = id;
                Date = date;
                Items = items;
                foreach (Item i in items)
                    Total = Total + i.Price;
                Total = Total - discount;
                Discount = discount;
            }
            public Sale() { }
        }
        
        public class Bill
        {
            public int Id { get; set; }
            public string Date { get; set; }
            public double Total { get; set; }
            public string Description { get; set; }
            public Bill(int id, string date, double total, string description)
            {
                Total = total;
                Description = description;
                Id = id;
                Date = date;
            }
            public Bill() { }
        }
    }
}

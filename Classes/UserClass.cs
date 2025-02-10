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
            public float Cash { get; set; } = 0;
            public List<Bill> Bills { get; set; } = new List<Bill>();
            public List<string> Categories { get; set; } = new List<string>();
            public List<Item> Items { get; set; } = new List<Item>();
            public UserStock() { }
            //public void CreateUserStock()
            //{
            //    Cash = 0;
            //    Bills = new List<Bill>();
            //    Categories = new List<string>();
            //    Items = new List<Item>();

            //    //test
            //    AddCategory("Phones");
            //    AddCategory("Tablets");
            //    AddItem("Iphone 11", 299, "null", "Phones");
            //    AddItem("Iphone 12", 345, "null", "Phones");
            //    AddItem("Iphone 11 Used", 189, "null", "Phones");
            //    AddItem("Iphone 15PM", 799, "null", "Phones");
            //    AddItem("Iphone 15", 559, "null", "Phones");
            //    AddItem("Ipad 11th Gen", 445, "null", "Tablets");
            //    AddItem("Vikosha Z60", 159, "null", "Tablets");
            //    AddItem("G-Tab G80 10\'", 69, "null", "Tablets");

            //    //Bills.Add();
            //    //Bills.Add();
            //    //Bills.Add();

            //}
            public void AddCategory(string name)
            {
                if (Categories.Exists(x => x == name))
                    throw new Exception("Cetegory with the same name already added.");
                Categories.Add(name);
            }
            public List<Item> GetCategory(string name)
            {
                if (!Categories.Exists(x => x == name))
                    throw new Exception("Cetegory with the same name was not found.");
                List<Item> items = new List<Item>();
                items.AddRange(Items.FindAll(x => x.Category == name));
                items.AddRange(Items.FindAll(x => x.Category == "none"));
                return items;
            }
            public void AddItem(string name, float price, string pic, string category)
            {
                if (!Categories.Exists(x => x == category))
                    throw new Exception("Cetegory with the same name was not found.");
                Items.Add(new Item(genarate_id(name, category, price.ToString()),
                    category, name, price, pic));

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
            public void AddBill(List<int> id, float discount)
            {
                List<Item> items = new List<Item>();
                foreach (var i in id)
                {
                    items.Add(Items.Find(x => x.Id == i));
                }
                Bill bill = new Bill(Bills.Count + 100, DateTime.UtcNow.AddHours(3).ToString(), items, discount);
                Cash += bill.Total;
                Bills.Add(bill);
            }
            public void RemoveCategory(string name)
            {
                Categories.Remove(name);
                foreach(Item item in Items)
                {
                    Items.Find(x => x.Category == name).Category = "none";
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
            public float Price { get; set; }
            public string Pic { get; set; }
            public string Category { get; set; }
            public Item(int id, string category, string name, float price, string pic)
            {
                Name = name;
                Id = id;
                Price = price;
                Pic = pic;
                Category = category;
            }
            public Item() { }
        }

        public class Bill
        {
            public int Id { get; set; }
            public string Date { get; set; }
            public float Total { get; set; }
            public List<Item> Items { get; set; }
            public float Discount { get; set; }
            public Bill(int id, string date, List<Item> items, float discount)
            {
                Id = id;
                Date = date;
                Items = items;
                foreach (Item i in items)
                    Total = Total + i.Price;
                Total = Total - discount;
                Discount = discount;
            }
            public Bill() { }
        }
    }
}

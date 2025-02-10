using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using FireSharp;
using FireSharp.Response;
using Azure;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity.Data;
using RestSharp.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;
using FireSharp.Extensions;
using POS_Data_API.Classes;


namespace POS_Data_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStockController : ControllerBase
    {
        //body = (phone , id , name)
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] Dictionary<string, string> body)
        {
            try
            {
                string phone = body["phone"];
                int id = int.Parse(body["userId"]);
                string name = body["name"];
                var check = await new FB().firebase.GetTaskAsync("USERS/" + phone);//query
                if (check.Body == "null")
                {
                    return StatusCode(404, "user does not exist");
                }
                else
                {
                    UserClass user = check.ResultAs<UserClass>();
                    if (user.UserID == id)
                    {
                        user.Stock.AddCategory(name);
                        FirebaseResponse response = await new FB().firebase.UpdateTaskAsync("USERS/" + phone + "/",user);
                        return Ok(response);
                    }
                }
                return StatusCode(404, $"User info error");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //body = (phone , id , name , pic , category , price)
        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem([FromBody] Dictionary<string, string> body)
        {
            try
            {
                string phone = body["phone"];
                int id = int.Parse(body["userId"]);
                string name = body["name"];
                string pic = body["pic"];
                string category = body["category"];
                float price = JsonConvert.DeserializeObject<float>(body["price"]);

                var check = await new FB().firebase.GetTaskAsync("USERS/" + phone);//query
                if (check.Body == "null")
                {
                    return StatusCode(404, "user does not exist");
                }
                else
                {
                    UserClass user = check.ResultAs<UserClass>();
                    if (user.UserID == id)
                    {
                        user.Stock.AddItem(name, price, pic, category);
                        FirebaseResponse response = await new FB().firebase.UpdateTaskAsync("USERS/" + phone + "/",user);
                        return Ok(response);
                    }
                }
                return StatusCode(404, $"User info error");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //body = (phone , id , itemsId , discount)
        [HttpPost("AddBill")]
        public async Task<IActionResult> AddBill([FromBody] Dictionary<string, string> body)
        {
            try
            {
                string phone = body["phone"];
                int id = int.Parse(body["userId"]);
                List<int> itemsId = JsonConvert.DeserializeObject<List<int>>(body["itemsId"]);
                float discount = JsonConvert.DeserializeObject<float>(body["discount"]);

                var check = await new FB().firebase.GetTaskAsync("USERS/" + phone);//query
                if (check.Body == "null")
                {
                    return StatusCode(404, "user does not exist");
                }
                else
                {
                    UserClass user = check.ResultAs<UserClass>();
                    if (user.UserID == id)
                    {
                        user.Stock.AddBill(itemsId, discount);
                        FirebaseResponse response = await new FB().firebase.UpdateTaskAsync("USERS/" + phone + "/",user);
                        return Ok(response);
                    }
                }
                return StatusCode(404, $"User info error");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //body = (phone , id , name)
        [HttpPost("RemoveCategory")]
        public async Task<IActionResult> RemoveCategory([FromBody] Dictionary<string, string> body)
        {
            try
            {
                string phone = body["phone"];
                int id = int.Parse(body["userId"]);
                string name = body["name"];
                var check = await new FB().firebase.GetTaskAsync("USERS/" + phone);//query
                if (check.Body == "null")
                {
                    return StatusCode(404, "user does not exist");
                }
                else
                {
                    UserClass user = check.ResultAs<UserClass>();
                    if (user.UserID == id)
                    {
                        user.Stock.RemoveCategory(name);
                        FirebaseResponse response = await new FB().firebase.UpdateTaskAsync("USERS/" + phone + "/", user);
                        return Ok(response);
                    }
                }
                return StatusCode(404, $"User info error");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //body = (phone , id , itemId)
        [HttpPost("RemoveItem")]
        public async Task<IActionResult> RemoveItem([FromBody] Dictionary<string, string> body)
        {
            try
            {
                string phone = body["phone"];
                int id = int.Parse(body["userId"]);
                int itemId = int.Parse(body["itemId"]);
                var check = await new FB().firebase.GetTaskAsync("USERS/" + phone);//query
                if (check.Body == "null")
                {
                    return StatusCode(404, "user does not exist");
                }
                else
                {
                    UserClass user = check.ResultAs<UserClass>();
                    if (user.UserID == id)
                    {
                        user.Stock.RemoveItem(itemId);
                        FirebaseResponse response = await new FB().firebase.UpdateTaskAsync("USERS/" + phone + "/", user);
                        return Ok(response);
                    }
                }
                return StatusCode(404, $"User info error");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        //fetch user data //body = (phone , id)
        [HttpPost("GetBills")]
        public async Task<IActionResult> GetBills([FromBody] Dictionary<string, string> body)
        {
            try
            {
                string phone = body["phone"];
                int id = int.Parse(body["userId"]);
                var check = await new FB().firebase.GetTaskAsync("USERS/" + phone);//query
                if (check.Body == "null")
                {
                    return StatusCode(404, "user does not exist");
                }
                else
                {
                    if (check.ResultAs<UserClass>().UserID == id)
                    {
                        return Ok(check.ResultAs<UserClass>().Stock.Bills);
                    }
                }
                return StatusCode(404, $"User info error");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("GetCategories")]
        //body = (phone , id , name)
        public async Task<IActionResult> GetCategories([FromBody] Dictionary<string, string> body)
        {
            try
            {
                string phone = body["phone"];
                int id = int.Parse(body["userId"]);
                var check = await new FB().firebase.GetTaskAsync("USERS/" + phone);//query
                if (check.Body == "null")
                {
                    return StatusCode(404, "user does not exist");
                }
                else
                {
                    if (check.ResultAs<UserClass>().UserID == id)
                    {
                        return Ok(check.ResultAs<UserClass>().Stock.Categories);
                    }
                }
                return StatusCode(404, $"User info error");
            }
            catch (Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("GetItems")]
        public async Task<IActionResult> GetItems([FromBody] Dictionary<string, string> body)
        {
            try
            {
                string phone = body["phone"];
                int id = int.Parse(body["userId"]);
                string category = body["category"];
                var check = await new FB().firebase.GetTaskAsync("USERS/" + phone);//query
                if (check.Body == "null")
                {
                    return StatusCode(404, "user does not exist");
                }
                else
                {
                    if (check.ResultAs<UserClass>().UserID == id)
                    {
                        return Ok(check.ResultAs<UserClass>().Stock.GetCategory(category));
                    }
                }
                return StatusCode(404, $"User info error");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("GetCash")]
        public async Task<IActionResult> GetCash([FromBody] Dictionary<string, string> body)
        {
            try
            {
                string phone = body["phone"];
                int id = int.Parse(body["userId"]);
                var check = await new FB().firebase.GetTaskAsync("USERS/" + phone);//query
                if (check.Body == "null")
                {
                    return StatusCode(404, "user does not exist");
                }
                else
                {
                    if (check.ResultAs<UserClass>().UserID == id)
                    {
                        return Ok(check.ResultAs<UserClass>().Stock.Cash);
                    }
                }
                return StatusCode(404, $"User info error");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}


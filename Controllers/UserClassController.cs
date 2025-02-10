
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
using POS_Data_API.Classes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace POS_Data_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserClassController : ControllerBase
    {
        //body = (phone , id , shopName , cash)
        [HttpPost("FirstLogin")]
        public async Task<IActionResult> FirstLogin([FromBody] Dictionary<string, string> body)
        {
            try
            {
                string shopName = body["shopName"];
                float cash = JsonConvert.DeserializeObject<float>(body["cash"]);
                int id = JsonConvert.DeserializeObject<int>(body["userId"]);
                string phone = body["phone"];

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
                        user.ShopName = shopName;
                        user.Stock.Cash = cash;
                        await new FB().firebase.UpdateTaskAsync(@"USERS/" + phone + "/", user); //query //warning!!
                        return Ok();
                    }
                }
                return StatusCode(404, $"User info error");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpPost("SignIn")]
        public async Task<IActionResult> Login([FromBody] Dictionary<string, string> body)
        {
            try
            {
                string Phone = body["phone"];
                string Password = body["password"];

                if (Phone == null || Password == null)
                    return BadRequest("Invalid User Data");

                //Check Phone regex
                Regex regex = new Regex(@"07\d{8}");
                Match match = regex.Match(Phone);

                if (!match.Success)
                    return BadRequest("Invalid PhoneNumber");

                //Check Password regex
                Regex regex2 = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$");
                Match match2 = regex2.Match(Password);

                if (!match2.Success)
                    return BadRequest("Invalid Password");

                var Check = await new FB().firebase.GetTaskAsync("USERS/" + Phone);
                if (Check.Body == "null")
                    return BadRequest("User does not exist");    
                else             
                    if (Check.ResultAs<UserClass>().Password != Password)
                        return BadRequest("Wrong Password");

                UserClass user = Check.ResultAs<UserClass>();
                Dictionary<string,dynamic> result = new Dictionary<string,dynamic>();
                result.Add("userId",user.UserID);
                result.Add("phone",user.Phone);
                result.Add("active",user.Active);
                result.Add("shopName", user.ShopName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> Register([FromBody] Dictionary<string, string> body)
        {
            
            try
            {
                string phone = body["phone"];
                string password = body["password"];

                if (phone == null || password == null)
                    return BadRequest("Invalid Data");
                //Check Email regex
                Regex regex = new Regex(@"^\d{10}$");
                Match match = regex.Match(phone);
                if(!match.Success)
                    return BadRequest("Invalid Email");
                //Check Password regex
                Regex regex2 = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$");
                Match match2 = regex2.Match(password);
                if (!match2.Success)
                    return BadRequest("Invalid Password");
                    var Check = await new FB().firebase.GetTaskAsync(@"USERS/" + phone);
                    if (Check.Body != "null")
                    {
                        return BadRequest("Phone Number already exist");
                    }
                await new FB().firebase.SetTaskAsync(@"USERS/"+ phone +"/", new UserClass(phone,password));

                Check = await new FB().firebase.GetTaskAsync(@"USERS/" + phone);
                UserClass user = Check.ResultAs<UserClass>();

                Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                result.Add("userId", user.UserID);
                result.Add("phone", user.Phone);
                result.Add("active", user.Active);
                result.Add("shopName", user.ShopName);
                return Ok(result);
             
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("RefreshUser")]
        public async Task<IActionResult> RefreshUser([FromBody] Dictionary<string, string> body)
        {
            try
            {
                int id = JsonConvert.DeserializeObject<int>(body["userId"]);
                string phone = body["phone"];

                var check = await new FB().firebase.GetTaskAsync("USERS/" + phone);
                if (check.Body == "null")
                {
                    return StatusCode(404, "user does not exist");
                }
                else
                {
                    UserClass user = check.ResultAs<UserClass>();
                    if (user.UserID == id)
                    {
                        Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                        result.Add("userId", user.UserID);
                        result.Add("phone", user.Phone);
                        result.Add("active", user.Active);
                        result.Add("shopName", user.ShopName);
                        return Ok(result);
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using Test.CSRF.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Test.CSRF.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
         
        private static string sessionCSRF = "csrf";
        private static string sessionAuthToken = "authToken";

        public ApiController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

         
        public static List<User> Users { get; } = new List<User>()
        {
            new User(1, "Tester 1", "tester", "", 1000),
            new User(2, "Tester 2", "tester", "", 1000),
            new User(3, "Tester 3", "tester", "", 1000),
        };


        [HttpGet("test")]
        public IActionResult test()
        {

            return Ok(_httpContextAccessor?.HttpContext?.Session.GetString(sessionAuthToken));

        }


        [HttpPost("login")]
        public IActionResult login([FromBody] AuthRequest authRequest)
        {

            User? user = Users.Where(u => u.Name == authRequest.Name).FirstOrDefault();

            if (user == null || user.Password != authRequest.Password) return Unauthorized();
            user.AuthToken = RandomString(12);
             
            _httpContextAccessor?.HttpContext?.Session.SetString(sessionAuthToken, user.AuthToken);

            return Ok(user);
        }
         
        [HttpGet("users")]
        [Authorize]
        public IActionResult getUsers()
        {
            if(_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Session.TryGetValue(sessionAuthToken, out byte[]? value) && value != null)
            {
                 return Ok(Users);
            }

            return Unauthorized();

           
        }
 
        [HttpPost("translation")]
        [Authorize]
        public IActionResult Translation(string authToken, int addresseesID, int sum)
        {
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Session.TryGetValue(authToken, out byte[]? value) && value != null)
            {
                int id = BitConverter.ToInt32(value, 0);

                User? user = Users.Where(u => u.Id == id).FirstOrDefault();
                User? Addressees = Users.Where(u => u.Id == addresseesID).FirstOrDefault();

                if (user == null || Addressees == null) return new StatusCodeResult(500);

                user.Balance -= sum;
                Addressees.Balance += sum;

                return Ok("Транзакция успешно завершена!");
            }

            return Unauthorized();
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}

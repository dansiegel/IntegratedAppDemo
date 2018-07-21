using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoMobileApi.DAL;
using DemoMobileApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoMobileApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private TodoContext _todoContext { get; }

        public TodoItemsController(TodoContext context)
        {
            _todoContext = context;
        }

        [HttpPost("")]
        public async Task<IActionResult> SaveItemAsync(TodoItem item)
        {
            item.UserName = $"{HttpContext.User.FindFirst("givenName").Value} {HttpContext.User.FindFirst("surname").Value}";

            var userId = HttpContext.User.FindFirst("objectId");
            if(string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
            }

            string message = null;
            if(_todoContext.TodoItems.Any(i => i.Id == item.Id))
            {
                var existingItem = _todoContext.TodoItems.First(i => i.Id == item.Id);
                if(existingItem.Done)
                {
                    message = $"{item.UserName} marked {item.Name} as completed";
                }
                else
                {
                    message = $"{item.UserName} updated {item.Name}";
                }
                _todoContext.TodoItems.Update(item);
            }
            else
            {
                message = $"{item.UserName} added {item.Name}";
                _todoContext.TodoItems.Add(item);
            }


            _todoContext.SaveChanges();


            return Ok();
        }

        [HttpGet("list")]
        public IEnumerable<TodoItem> GetAll()
        {
            return _todoContext.TodoItems.ToList();
        }
    }
}
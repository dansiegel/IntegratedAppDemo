using System.Collections.Generic;
using System.Threading.Tasks;
using IntegratedTodoClient.Models;

namespace IntegratedTodoClient.Todo.Services
{
    public interface IApiClient
    {
        Task<UserProfile> GetUserProfileAsync();

        Task SaveTodoItemAsync(TodoItem item);

        Task<IEnumerable<TodoItem>> GetTodoItemsAsync();
    }
}

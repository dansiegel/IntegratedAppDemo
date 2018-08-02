using System.Collections.Generic;
using System.Threading.Tasks;
using IntegratedTodoClient.Models;

namespace IntegratedTodoClient.Services
{
    public interface IApiClient
    {
        Task<bool> LoginAsync();

        Task<UserProfile> GetUserProfileAsync();

        Task SaveTodoItemAsync(TodoItem item);

        Task<IEnumerable<TodoItem>> GetTodoItemsAsync();
    }
}

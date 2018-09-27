using System.Threading.Tasks;

namespace IntegratedTodoClient.Identity.Services
{
    public interface IAuthClient
    {
        Task<bool> LoginAsync();

        Task Logout();
    }
}

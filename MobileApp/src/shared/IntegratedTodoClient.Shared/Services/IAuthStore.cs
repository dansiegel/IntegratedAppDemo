using System.Threading.Tasks;

namespace IntegratedTodoClient.Services
{
    public interface IAuthStore
    {
        Task<string> GetTokenAsync();
    }
}

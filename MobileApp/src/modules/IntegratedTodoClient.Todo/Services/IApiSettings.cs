namespace IntegratedTodoClient.Todo.Services
{
    public interface IApiSettings
    {
        string ApiEndpoint { get; }
        string InstallId { get; }
    }
}

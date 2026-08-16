namespace WebApplication1.Service.Interface
{
    public interface IAuditService
    {
        Task LogActivityAsync(string? userName, string action, string status, string ipAddress, string? details = null);
    }
}

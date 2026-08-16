using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Service.Interface;

namespace WebApplication1.Service.Impl
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(string? userId, string action, string status, string ipAddress, string? details = null)
        {
            var auditLog = new AuditLog
            {
                UserName = userId,
                Action = action,
                Status = status,
                IpAddress = ipAddress,
                Details = details
            };

            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }
    }
}

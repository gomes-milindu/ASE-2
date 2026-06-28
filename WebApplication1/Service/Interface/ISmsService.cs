namespace WebApplication1.Service.Interface
{
    public interface ISmsService
    {
        Task SendSmsAsync(string toMobile,string smsMessage);
    }
}

namespace BusTour.Common.Services
{
    public interface IUserContext
    {
        int UserId { get; }
        string Role { get; }
    }
}

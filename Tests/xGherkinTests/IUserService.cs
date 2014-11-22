
namespace xGherkinTests
{
    public interface IUserService
    {
        void Add(User user);

        User GetByCredentials(string username, string password);

        bool ResetPassword(User user, string oldpassword, string newPassword);
    }
}

using Xunit;
using xGherkin;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Moq;
using System.Linq;
using Xunit.Should;

namespace xGherkinTests
{
    [Issue("345"),
     Feature("Password management",
               @"An authenticated user must be able to change password, by providing new and old password. 
                 If user isn't authenticated yet, then providing email (that user had registered with), 
                 s/he shoudld be able to reset password. Resetting password will cause an email to be sent to 
                 the user with a new system generated password.
    ")]
    public class PasswordManagement
    {
        public void Background()
        {
            "Given that following users exist".Do(new GherkinTable("username", "password")
            {
                {"bob@123", "pass1"},
                {"sam@123", "pass2"}
            }, SetupUserService);
        }

        [PBI("32160"), Bug("42150"), Sprint("1"),
        Senario("Successfull password change")]
        public void Senario1()
        {
            string oldpassword = "";
            string newpassword = "";
            bool success = false;

            "Given that I have logged in with 'bob@123'".Do(_ =>
            {
                _authenticatedUser = new User { Username = "bob@123", Password = "pass1" };
            });

            "And I have set oldpassword to 'pass1' and new password to 'pass2'".Do(_ =>
            {
                oldpassword = "pass1";
                newpassword = "pass2";
            });

            "When I call reset password".Do(_ =>
            {
                success = _userService.ResetPassword(_authenticatedUser, oldpassword, newpassword);
            });

            "Then it should be successful".Do(_ => 
            {
                success.ShouldBeTrue();
            });

            "And calling GetByCredentials with username: 'bob@123' and with password: 'pass1' should return null".Do(_ =>
            {
                _userService.GetByCredentials("bob@123", "pass1").ShouldBeNull();
            });

            @"And calling GetByCredentials with username: 'bob@123' and password: 'pass2' 
        should return a user with username 'bob@123'".Do(_ =>
            {
                var user = _userService.GetByCredentials("bob@123", "pass2");

                user.ShouldNotBeNull();
                user.Username.ShouldBe("bob@123");
            });

        }

        [PBI("32160"), Bug("42150"), Sprint("1"),
        Senario("Unsuccessfull password change, due to wrong old password")]
        public void Senario2()
        {
            string oldpassword = "";
            string newpassword = "";
            bool success = false;

            "Given that I have logged in with 'bob@123'".Do(_ =>
            {
                _authenticatedUser = new User { Username = "bob@123", Password = "pass1" };
            });

            "And I have set oldpassword to 'abc' and new password to 'pass2'".Do(_ =>
            {
                oldpassword = "abc";
                newpassword = "pass2";
            });

            "When I call reset password".Do(_ =>
            {
                success = _userService.ResetPassword(_authenticatedUser, oldpassword, newpassword);
            });

            "Then it should not succeed".Do(_ =>
            {
                success.ShouldBeFalse();
            });

            @"And calling GetByCredentials with username: 'bob@123' and password: 'abc' should return null".Do(_ =>
            {
                _userService.GetByCredentials("bob@123", "abc").ShouldBeNull();
            });

            @"And calling GetByCredentials with username: 'bob@123' and password: 'pass1' should user".Do(_ =>
            {
                _userService.GetByCredentials("bob@123", "pass1").ShouldNotBeNull();
            });

        }

        IUserService _userService;
        
        User _authenticatedUser;

        private void SetupUserService(GherkinTable table)
        {
            var userFromTable = table.Rows.Select(x => new User { Username = x[0] as string, Password = x[1] as string }).ToList();

            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            var serviceMock = fixture.Freeze<Mock<IUserService>>();

            serviceMock.Setup(rep => rep.GetByCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((username, password) =>
            {
                return userFromTable.FirstOrDefault(x => x.Username == username && x.Password == password);
            });

            serviceMock.Setup(rep => rep.ResetPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).Returns<User, string, string>((user, oldpassword, newpassword) =>
            {
                var svcUser = userFromTable.FirstOrDefault(x => x.Username == user.Username && x.Password == oldpassword);
                if (svcUser != null)
                {
                    svcUser.Password = newpassword;
                    return true;
                }

                return false;
            });

            _userService = serviceMock.Object;
        }
    }

}

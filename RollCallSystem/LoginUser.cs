namespace RollCallSystem
{
    public class LoginUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
        
        public LoginUser()
        {

        }

        public LoginUser(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}

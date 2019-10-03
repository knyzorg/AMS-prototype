namespace vkAMS_prototype.Models
{
    public class LoginResult {
        public bool? Success { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class LoginPageModel {
        public LoginResult LoginResult { get; set; }
        public string ReturnUrl { get; set; }
    }
}
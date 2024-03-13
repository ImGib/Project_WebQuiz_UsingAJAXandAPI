namespace API.Common
{
    public static class Variables
    {
        public enum UserStatus
        {
            Disable,
            Enable
        }
        public enum UserRole
        {
            Admin,
            NormalUser
        }
        public static int numberQuestion = 4;
        public static string Admin = "Admin";
        public static string NormalUser = "NormalUser";
        public static string ResponseOk = "OK";
        public static string ResponseError = "Error";
        public static string ResgisterOk = "Resgister Successfully";
        public static string ResgisterFail = "Resgister Fail";
        public static string UserExisted = "User Name Already Existed";
        public static string UserNotExisted = "User Name Not Existed";
        public static string EmailExisted = "Email Already Existed";
        public static string PhoneNumberExisted = "PhoneNumber Already Existed";
        public static string LoginOk = "Login Successfully";
        public static string LoginFail = "Your User Name Or Password might wrong";
        public static string ConfirmPassFail = "Your Confirm Password has wrong";
        public static string RequestEnroll = "Your Have To Enroll Subject";
    }
}

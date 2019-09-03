namespace VBaseProject.Service.Authentication
{
    public class AuthenticationConfiguration
    {
        public static class JWT
        {
            public const string SymmetricSecurityKey = "5cCI6IkpXVCJ9.eyJlbWFpbCI6InZhbmRlcmxhbi5nc0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1p";
            public const string Issuer = "http://www.myinvapp.com";
            public const string Audience = "http://www.myinvapp.com";

            public const int TokenExpirationMinutes = 30;
        }

        public static class Roles
        {
            public const string Admin = "admin";
            public const string Customer = "customer";
        }
    }
}

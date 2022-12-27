namespace VBaseProject.Domain.Authentication
{
    public static class AuthenticationConfiguration
    {
        public static class Jwt
        {
            public const string SymmetricSecurityKey = "5cCI6IkpXVCJ9.eyJlbWFpbCI6InZhbmRlcmxhbi5nc0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1p";
            public const string Issuer = "http://www.myapplication.com";
            public const string Audience = "http://www.myapplication.com";

            public const int TokenExpirationMinutes = 10;
        }

        public static class Roles
        {
            public const string Admin = "admin";
            public const string Customer = "customer";
        }
    }
}

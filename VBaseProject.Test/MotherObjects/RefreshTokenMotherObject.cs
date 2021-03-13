using System;
using VBaseProject.Api.Models;
using VBaseProject.Entities.Domain;

namespace VBaseProject.Test.MotherObjects
{
    public class RefreshTokenMotherObject
    {
        public static RefreshToken ValidRefreshToken()
        {
            return new RefreshToken
            {
                Email = UserMotherObject.ValidAdminUser().Email,
                Refreshtoken = "d3326815-8839-47e2-9a9d-9ff6ec945c60",
                PublicId = "a3323815-8839-47e2-9a9d-9ff6ec945c61",
                CreatedAt = new DateTime(2021, 01, 01)
            };
        }
        public static RefreshTokenModel ValidRefreshTokenModel()
        {
            return new RefreshTokenModel
            {
                RefreshToken = "d3326815-8839-47e2-9a9d-9ff6ec945c60"
            };
        }
    }
}

using Framework.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common
{
    public static class Common
    {
        public static string MD5(this string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        public static bool vnIsNull(this object o)
        {
            return o == null || o == DBNull.Value;
        }

        private static JWTContainerModel GetJWTContainerModel(string userId)
        {
            return new JWTContainerModel()
            {
                Claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId),
                }
            };
        }
        public static string GenerateJWT(this string userId)
        {
            IAuthContainerModel model = GetJWTContainerModel(userId);
            IAuthService authService = new JWTService(model.SecretKey);

            return authService.GenerateToken(model);
        }
        public static string ValidJWT(this string token)
        {
            try
            {
                IAuthContainerModel model = new JWTContainerModel();
                IAuthService authService = new JWTService(model.SecretKey);
                if (!authService.IsTokenValid(token))
                    throw new UnauthorizedAccessException();
                else
                {
                    List<Claim> claims = authService.GetTokenClaims(token).ToList();
                    return claims.FirstOrDefault(e => e.Type.Equals(ClaimTypes.Name)).Value;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}

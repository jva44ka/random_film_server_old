using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace randomfilm_backend.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "randomfilm_backend"; // издатель токена
        public const string AUDIENCE = "randomfilm_frontend"; // потребитель токена
        private const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
        public const int LIFETIME = 180; // время жизни токена - в минутах
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}

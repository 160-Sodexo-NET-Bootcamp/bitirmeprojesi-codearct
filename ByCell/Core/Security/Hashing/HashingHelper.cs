using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

namespace Core.Security.Hashing
{
    public class HashingHelper
    {
        //Kullanıcının kayıt sırasında verdiği şifreye bağlı olarak password hash ve password salt üretecek
        //ürettiği hash ve saltı dışarı verecek
        public static void CreatePasswordHash(string registeredPassword,out byte[] registeredPasswordHash,out byte[] registeredPasswordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                registeredPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registeredPassword));
                registeredPasswordSalt = hmac.Key;
            }
        }
        //kullanıcı giriş sırasında girdiği şifreye ve veritabanından gelen salta bağlı olarak yeni bir password hash üretecek
        //üretilen yeni password hash ile veritabanındaki password hashi karşılaştıracak
        //birebir eşleşme sağlanırsa true; sağlanmazsa false dönecek
        public static bool CheckPasswordHash(string loginPassword, byte[] registeredPasswordHash, byte[] registeredPasswordSalt)
        {
            using (var hmac = new HMACSHA256(registeredPasswordSalt))
            {
                var loginPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginPassword));

                for (int i = 0; i < loginPasswordHash.Length; i++)
                {
                    if (loginPasswordHash[i]!= registeredPasswordHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;

        }
    }
}

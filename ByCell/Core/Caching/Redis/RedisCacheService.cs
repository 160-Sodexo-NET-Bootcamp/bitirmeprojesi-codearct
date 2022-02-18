using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Caching.Redis
{
    public class RedisCacheService:ICacheService
    {
        private readonly RedisEndpoint _redisEndpoint;

        public RedisCacheService()
        {
            //Redisin bağlantı bilgileri yapıcı methodla gelir
            _redisEndpoint = new RedisEndpoint("localhost", 6379);
        }

        //Verilen tip ve anahtara göre Redisten veri dönüşü yapar
        public T Get<T>(string key)
        {
            var result = default(T);
            RedisInvoker(x => { result = x.Get<T>(key); });
            return result;
        }

        //Verilen anahtara göre Redisten object tipindeki veriyi döner
        public object Get(string key)
        {
            var result = default(object);
            RedisInvoker(x => { result = x.Get<object>(key); });
            return result;
        }

        //Tanımlanan anahtar ile verilen object tipindeki veriler Redise kayededilir
        //Veriler verilen süre boyunca Rediste saklanır daha sonra otomatik silinir
        public void Add(string key, object data, int duration)
        {
            RedisInvoker(x => x.Add(key, data, TimeSpan.FromMinutes(duration)));
        }

        //Tanımlanan anahtar ile verilen object tipindeki veriler Redise kaydedilir
        public void Add(string key, object data)
        {
            RedisInvoker(x => x.Add(key, data));
        }

        //Redis e tanımlanan anahtar ile daha önce veri girişi yapılmış mı kontrol eder
        public bool IsAdd(string key)
        {
            var isAdded = false;
            RedisInvoker(x => isAdded = x.ContainsKey(key));
            return isAdded;
        }
        //Tanımlanan anahtara göre veriyi siler
        public void Remove(string key)
        {
            RedisInvoker(x => x.Remove(key));
        }

        //Tanımlanan desene göre veritabanındaki verileri siler
        public void RemoveByPattern(string pattern)
        {
            RedisInvoker(x => x.RemoveByPattern(pattern));
        }

        //Redis veritabanlarındaki bütün verileri siler
        public void Clear()
        {
            RedisInvoker(x => x.FlushAll());
        }

        //Redis instance oluşturulur ilgili bağlantı kullanılarak
        //İçine aldığı action methodu işler
        private void RedisInvoker(Action<RedisClient> redisAction)
        {
            using (var client = new RedisClient(_redisEndpoint))
            {
                redisAction.Invoke(client);
            }
        }
    }
}

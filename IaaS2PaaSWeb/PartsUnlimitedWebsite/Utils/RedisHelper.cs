using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PartsUnlimited.Utils
{
    static class RedisHelper
    {
        /// <summary>
        /// static object to use for redis interaction, new-up only when required
        /// </summary>
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string redisHost = ConfigurationManager.AppSettings["RedisCacheConnectionString"];
            return ConnectionMultiplexer.Connect(redisHost);
        });

        /// <summary>
        /// Redis connection object
        /// </summary>
        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        /// <summary>
        /// Retrieve a strongly-typed T based on key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            var r = Connection.GetDatabase().StringGet(key);
            return Deserialize<T>(r);
        }

        /// <summary>
        /// Retrieve a strongly-typed List of T based on key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(string key)
        {
            return (List<T>)Get(key);
        }

        /// <summary>
        /// Store a strongly-typed List of T with key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="list"></param>
        public static void SetList<T>(string key, List<T> list)
        {
            Set(key, list, new TimeSpan(0, 5, 0));
        }

        /// <summary>
        /// Store a strongly-typed List of T with key, specify TTL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="list"></param>
        /// <param name="expiration"></param>
        public static void SetList<T>(string key, List<T> list, TimeSpan expiration)
        {
            Set(key, list, expiration);
        }

        /// <summary>
        /// Retrieve an untyped object based on key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            return Deserialize<object>(Connection.GetDatabase().StringGet(key));
        }

        /// <summary>
        /// Store an untyped object with key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set(string key, object value)
        {
            Set(key, Serialize(value), new TimeSpan(0,5,0));
        }

        /// <summary>
        /// Store and untyped object with key, specified TTL
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        public static void Set(string key, object value, TimeSpan expiration)
        {
            Connection.GetDatabase().StringSet(key, Serialize(value), expiration);
        }

        /// <summary>
        /// Convert object to binary for storage
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        static byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        /// <summary>
        /// Re-hydrate object from binary serialized format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        static T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                T result = default(T);
                try
                {
                    result = (T)binaryFormatter.Deserialize(memoryStream);
                }
                catch(Exception ex)
                {
                    //eat it
                }
                return result;

            }
        }
    }
}
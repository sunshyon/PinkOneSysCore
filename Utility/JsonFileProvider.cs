using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// Json配置文件适配器
    /// </summary>
    public class JsonFileProvider
    {
        #region 变量区
        /// <summary>
        /// 加载锁
        /// </summary>
        private readonly object mLoadLock = new object();
        /// <summary>
        /// 配置缓存
        /// </summary>
        private readonly ConcurrentDictionary<string, object> mSettingsCache = new ConcurrentDictionary<string, object>();
        /// <summary>
        /// 文件路径
        /// </summary>
        private string mFilePath;
        /// <summary>
        /// 文件对应文件夹
        /// </summary>
        private string mFIleFolder = @"Config/";
        #endregion

        #region 内嵌延时实体
        private class Nested
        {
            static Nested() { }
            internal static readonly JsonFileProvider Provider = new JsonFileProvider();
        }
        #endregion

        #region 对象实例
        public static JsonFileProvider Instance
        {
            get
            {
                return Nested.Provider;
            }
        }
        #endregion

        #region 构造函数
        public JsonFileProvider()
        {
            mFilePath = Directory.GetCurrentDirectory() +"//"+ mFIleFolder;
        }
        #endregion

        #region 方法区域
        public T GetSettings<T>()where T:new()
        {
            object cached;
            if(mSettingsCache.TryGetValue(GetTypeName<T>(),out cached))
                return (T)cached;

            lock (mLoadLock)
            {
                if (mSettingsCache.TryGetValue(GetTypeName<T>(), out cached))
                    return (T)cached;
                var settings = GetFromFile<T>();
                if (settings == null)
                    return default(T);
                AddUpdateWatcher(settings);//更新缓存
                mSettingsCache.TryAdd(GetTypeName<T>(), settings);//添加缓存
                return settings;
            }
        }

        #endregion

        #region 方法区域
        /// <summary>
        /// 获取类型名称，注意：类型名就是文件名
        /// </summary>
        private string GetTypeName<T>()
        {
            string fullName = typeof(T).FullName;
            fullName = fullName.Substring(fullName.LastIndexOf('[') + 1);
            if (fullName.IndexOf(',') > 0)
                fullName = fullName.Substring(0, fullName.IndexOf(','));
            fullName = fullName.Substring(fullName.LastIndexOf('.') + 1);
            return fullName;
        }
        /// <summary>
        /// 读取文件内容
        /// </summary>
        private T GetFromFile<T>() where T : new()
        {
            //获取文件路径
            string path= Path.Combine(mFilePath, GetTypeName<T>() + ".json");
            try
            {
                if (!File.Exists(path))
                    return new T();
                using(var sr = File.OpenText(path))
                {
                    var serializer = new JsonSerializer
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling= NullValueHandling.Include
                    };
                    return (T)serializer.Deserialize(sr, typeof(T));
                }
            }
            catch
            {
                return default(T);
            }
        }
        /// <summary>
        /// 配置更新函数
        /// <summary>
        private void AddUpdateWatcher<T>(T settings) where T : new()
        {
            var watcher = new FileSystemWatcher(mFilePath, GetTypeName<T>() + ".json")
            {
                NotifyFilter = NotifyFilters.LastWrite
            };
            watcher.Changed += (s, args) =>
            {
                string key = GetTypeName<T>();
                T value = GetFromFile<T>();
                if (mSettingsCache.ContainsKey(key))
                    mSettingsCache[key] = value;
                else
                    mSettingsCache.TryAdd(key, value);
            };
            watcher.EnableRaisingEvents = true;
        }
        #endregion
    }
}

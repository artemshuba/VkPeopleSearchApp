using System;
using System.Diagnostics;
using System.IO;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace VkPeopleSearchApp.Domain
{
    /// <summary>
    /// Base class for application settings
    /// </summary>
    public abstract class AppSettings
    {
        private readonly IPropertySet _settings = ApplicationData.Current.LocalSettings.Values;

        public void Set<TValue>(string settingName, TValue value)
        {
            try
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, value);
                    _settings[settingName] = writer.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public bool TryGet<TValue>(string settingName, out TValue value)
        {
            object val;
            if (_settings.TryGetValue(settingName, out val))
            {

                try
                {
                    var strValue = (string)val;
                    using (var reader = new StringReader(strValue))
                    {
                        using (var jsonReader = new Newtonsoft.Json.JsonTextReader(reader))
                        {
                            var serializer = new Newtonsoft.Json.JsonSerializer();
                            value = serializer.Deserialize<TValue>(jsonReader);
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    value = default(TValue);
                    Debug.WriteLine(ex);
                }

                return false;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public T Get<T>(string settingName, T defaultValue)
        {
            T obj;
            if (TryGet<T>(settingName, out obj))
                return obj;
            else
                return defaultValue;
        }

        public virtual void Load()
        {

        }

        public virtual void Save()
        {

        }

        public virtual void Clear()
        {

        }
    }
}

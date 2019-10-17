using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace Server.Services
{
    public class TsvHandler
    {
        public bool TryConvertTsvToObject<T>(string delimetedFormat, out T searlizedObj) where T : new()
        {
            string separator = "\t";
            searlizedObj = new T();
            try
            {
                if (string.IsNullOrEmpty(delimetedFormat))
                    return true;
                PropertyInfo[] tProperties = GetObjProperties<T>();
                int index = 0;
                var splitedText = delimetedFormat.Split(separator);

                //create as JSON to parse later
                JObject obj = new JObject();
                foreach (var item in tProperties)
                {
                    if (splitedText.Count() > index)
                        obj[item.Name] = splitedText[index];
                    else
                        break;
                    index++;
                }

                try
                {
                    searlizedObj = JsonConvert.DeserializeObject<T>(obj.ToString());
                }
                catch (Exception ex)
                {

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private PropertyInfo[] GetObjProperties<T>()
        {
            PropertyInfo[] tProperties = null;

            tProperties = typeof(T)
               .GetProperties()
               .ToArray();


            return tProperties;
        }
    }
}

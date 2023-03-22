using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataRooms.UI
{
    public static class DataRoomsExtension
    {
        public static string ToApplicationFormat(this DateTime value)
        {
            if (value == null)
                return "";
            return value.ToString("dd-MM-yyyy hh:mm:ss");
        }
        public static string ToApplicationFormat(this DateTime? value)
        {
            if (value == null)
                return "";
            return value.Value.ToString("dd-MM-yyyy hh:mm:ss");
        }

        public static List<Variance> DetailedCompare<T>(this T val1, T val2)
        {
            List<Variance> variances = new List<Variance>();
            foreach (PropertyInfo property in val1.GetType().GetProperties())
            {
                Variance v = new Variance();
                v.Column = property.Name;
                v.OldValue = property.GetValue(val1, null);
                v.NewValue = property.GetValue(val2, null);
                if (!Equals(v.OldValue, v.NewValue))
                    variances.Add(v);
            }
            return variances;
        }

        public static T Clone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null)) return default;

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        /// <summary>
        /// Function to convert the given bytes to either Kilobyte, Megabyte, or Gigabyte
        /// </summary>
        /// <param name="bytes">Double -> Total bytes to be converted</param>
        /// <param name="type">String -> Type of conversion to perform</param>
        /// <returns>Int32 -> Converted bytes</returns>
        /// <remarks></remarks>
        public static string ConvertSize(this double bytes, string type)
        {
            try
            {
                string size = "";
                switch (type)
                {
                    case "MB":
                        for (int i = 0; i < 32; i++)
                        {
                            bytes = bytes / 1024;
                        }
                        size = Math.Round(bytes, 3) + " MB";
                        break;
                    case "GB":
                        for (int i = 0; i < 3; i++)
                        {
                            bytes = bytes / 1024;
                        }
                        size = Math.Round(bytes, 3) + " GB";
                        break;
                }
                return size;

                //string[] Suffix = { " B", " kB", " MB", " GB", " TB" };
                //double dblSByte = bytes;
                //int i;
                //for (int j = 0; j < 3; j++)
                //{
                //    dblSByte = 
                //}
                //for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
                //{
                //    dblSByte = bytes / 1024.0;
                //}
                //return $"{dblSByte:0.##}{ Suffix[i] }";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

       
    }
    public class Variance
    {
        public string Column { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}

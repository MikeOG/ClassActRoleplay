using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Drawing;
using CitizenFX.Core;
using System.Collections;
using System.Globalization;
using Newtonsoft.Json;

namespace Roleplay.Shared.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Changes e.g. BANANA to Banana 
        /// useful for e.g. vehicle model names which are returned in caps
        /// from default native
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string str)
        {
            var tokens = str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                tokens[i] = token.Substring(0, 1).ToUpper() + token.Substring(1).ToLower();
            }

            return String.Join(" ", tokens);
        }

        /// <summary>
        /// Turns e.g. "StunGun" into "Stun Gun"
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string AddSpacesToCamelCase(this string source)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in source)
            {
                if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(c);
            }
            return builder.ToString();
        }

        public static string RemoveSpaces(this string source)
        {
            return source.Replace(" ", "");
        }

        /// <summary>
        /// Returns same string but with first letter changed to uppercase.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FirstLetterToUpper(this string source)
        {
            if (source == null)
                return null;

            if (source.Length > 1)
                return Char.ToUpper(source[0]) + source.Substring(1);

            return source.ToUpper();
        }

        public static string ToLength(this string source, int length = 10)
        {
            if (source.Length > length)
                return source.Substring(0, length);

            while (source.Length < length)
            {
                source += " ";
            }

            return source;
        }

        public static string RemoveSection(this string source, char start, char end)
        {
            var startIndex = source.IndexOf(start);
            var endIndex = source.IndexOf(end);

            if (startIndex != -1 && endIndex != -1)
            {
                var tempSource = source;

                tempSource = tempSource.Substring(0, startIndex);

                source = tempSource + source.Substring(endIndex);
            }

            return source;
        }

        public static T Clamp<T>(T value, T min, T max)
        where T : IComparable<T>
        {
            T result = value;
            if (value.CompareTo(max) > 0)
                result = max;
            if (value.CompareTo(min) < 0)
                result = min;
            return result;
        }

        public static bool NextBool(this Random r, int truePercentage = 50)
        {
            return r.NextDouble() < truePercentage / 100.0;
        }

        public static double NextDouble(this Random random, double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
        {
            foreach (var value in list)
            {
                await func(value);
            }
        }

        public static PointF Add(this PointF c1, PointF c2)
        {
            return new PointF(c1.X + c2.X, c1.Y + c2.Y);
        }

        public static PointF Subtract(this PointF c1, PointF c2)
        {
            return new PointF(c1.X - c2.X, c1.Y - c2.Y);
        }

        public static List<T> Slice<T>(this List<T> list, int start, int end)
        {
            return list.Skip(start).Take(end - start + 1).ToList();
        }

        public static bool IsBetween<T>(this T value, T start, T end) where T : IComparable
        {
            return value.CompareTo(start) >= 0 && value.CompareTo(end) <= 0;
        }

        public static Color ToColor(this string color)
        {
            try
            {
                return Color.FromArgb(Int32.Parse(color.Replace("#", ""),
                             NumberStyles.AllowHexSpecifier));
            }
            catch (Exception e)
            {
                //Log.Error($"ToColor exception: {e}");
            }
            return Color.FromArgb(255, 255, 255, 255);
        }

        public static int[] ToArray(this Color color)
        {
            return new int[] {color.R, color.G, color.B};
        }

        public static float[] ToArray(this Vector3 vector)
        {
            try
            {
                return new float[] { vector.X, vector.Y, vector.Z };
            }
            catch (Exception e)
            {
                //Log.Error($"ToArray exception: {e}");
            }
            return null;
        }

        public static Vector3 ToVector3(this float[] xyzArray)
        {
            try
            {
                return new Vector3(xyzArray[0], xyzArray[1], xyzArray[2]);
            }
            catch (Exception e)
            {
                //Log.Error($"ToVector3 exception: {e}");
            }
            return Vector3.Zero;
        }

        /// <summary>
        /// Extension method that turns a dictionary of string and object to an ExpandoObject
        /// </summary>
        public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = ((IDictionary<string, object>)kvp.Value).ToExpando();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any strin-object dictionaries
                    // along the way into expando objects
                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = ((IDictionary<string, object>)item).ToExpando();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }

                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }

            return expando;
        }

        public static void Print(this ExpandoObject dynamicObject)
        {
            var dynamicDictionary = dynamicObject as IDictionary<string, object>;

            foreach (KeyValuePair<string, object> property in dynamicDictionary)
            {
                Debug.WriteLine("{0}: {1}", property.Key, property.Value.ToString());
            }
            Debug.WriteLine();
        }

        public static string ToObjectString(this Vector3 pos)
        {
            return $"new_Vector3({pos.ToString("0.00f")})"
                .Replace("X", "")
                .Replace("Y", "")
                .Replace("Z", "")
                .Replace(":", "")
                .Replace(" ", ", ")
                .Replace("_", " ");
        }

        public static Vector3 ToVector3(this List<object> vecArray)
        {
            return new Vector3(vecArray.Select(Convert.ToSingle).ToArray());
        }

        public static bool IsInPolygon(this Vector3 vec, float[][] polygon)
        {
            var currentPosition = new[] { vec.X, vec.Y };

            return PolygonCollision.Contains(polygon, currentPosition);
        }

        public static bool IsInPolygon(this Vector3 vec, List<float[][]> polygons)
        {
            foreach (var poly in polygons)
            {
                if (vec.IsInPolygon(poly))
                    return true;
            }

            return false;
        }

        public static List<T> Shuffle<T>(this List<T> inputList)
        {
            List<T> randomList = new List<T>();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }

        /// <summary>
        /// Removes all items from a list after the inputted index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputList">List the items want to be removed from</param>
        /// <param name="index">The index that items will be removed from after (the inputted index will not be removed)</param>
        /// <returns></returns>
        public static List<T> RemoveFrom<T>(this List<T> inputList, int index)
        {
            var itemList = new List<T>();

            while (index > inputList.Count) index--;

            for (var idx = 0; idx < index; idx++)
            {
                Log.Debug($"{idx}: {inputList[idx]} ({index})");
                itemList.Add(inputList[idx]);
            }

            return itemList;
        }

        public static bool TryParseJson<T>(this string @this, out T result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(@this, settings);
            return success;
        }
    }
}

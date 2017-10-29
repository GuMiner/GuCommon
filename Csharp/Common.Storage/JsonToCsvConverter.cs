using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Common.Storage
{
    /// <summary>
    /// Converts a JSON array of objects into CSV
    /// </summary>
    public class JsonToCsvConverter
    {
        /// <summary>
        /// Converts a JSON array under the <paramref name="jsonStartPath"/> into a CSV array, optionally with a header line.
        /// </summary>
        /// <param name="filename">The name of the file being converted.</param>
        /// <param name="jsonStartPath">The path that the JSON array is under</param>
        /// <param name="includeHeader">If true, includes the header line.</param>
        /// <returns>The CSV array</returns>
        public static IEnumerable<string> ConvertJsonToCsv(string filename, string jsonStartPath, bool includeHeader)
        {
            return JsonToCsvConverter.ConvertJsonToCsv(File.ReadAllLines(filename).ToList(), jsonStartPath, includeHeader);
        }

        /// <summary>
        /// Converts a JSON array under the <paramref name="jsonStartPath"/> into a CSV array, optionally with a header line.
        /// </summary>
        /// <param name="jsonLines">The JSON content</param>
        /// <param name="jsonStartPath">The path that the JSON array is under</param>
        /// <param name="includeHeader">If true, includes the header line.</param>
        /// <returns>The CSV array</returns>
        public static IEnumerable<string> ConvertJsonToCsv(IEnumerable<string> jsonLines, string jsonStartPath, bool includeHeader)
        {
            return JsonToCsvConverter.ConvertJsonToDeliminatedArray(jsonLines, jsonStartPath, ",", includeHeader);
        }

        /// <summary>
        /// Converts a JSON array under the <paramref name="jsonStartPath"/> into a deliminated,  array, optionally with a header line.
        /// </summary>
        /// <param name="jsonLines">The JSON content</param>
        /// <param name="jsonStartPath">The path that the JSON array is under</param>
        /// <param name="deliminator">The deliminator in the array</param>
        /// <param name="includeHeader">If true, includes the header line.</param>
        /// <returns>The CSV array</returns>
        public static IEnumerable<string> ConvertJsonToDeliminatedArray(IEnumerable<string> jsonLines, string jsonStartPath, string deliminator, bool includeHeader)
        {
            List<string> deliminatedLines = new List<string>();
            bool hasParsedHeader = !includeHeader;
            List<string> headerValues = new List<string>();

            JObject rootJsonObject = JObject.Parse(string.Join(string.Empty, jsonLines));
            JArray array = rootJsonObject.SelectToken(jsonStartPath) as JArray;
            foreach (JToken childToken in array.Children())
            {
                List<string> propertyValues = new List<string>();
                foreach (JProperty property in childToken.Children().Select(token => token as JProperty).OrderBy(prop => prop.Name))
                {
                    if (!hasParsedHeader)
                    {
                        headerValues.Add(property.Name);
                    }

                    // Properly escape the property value if it contains the deliminator.
                    string propertyValue = property.Value.ToString();
                    if (propertyValue.Contains(deliminator))
                    {
                        propertyValue = "\"" + propertyValue.Replace("\"", "\"\"") + "\"";
                    }
                    propertyValues.Add(propertyValue);
                }

                if (!hasParsedHeader)
                {
                    deliminatedLines.Add(string.Join(deliminator, headerValues));
                    hasParsedHeader = true;
                }

                deliminatedLines.Add(string.Join(deliminator, propertyValues));
            }

            return deliminatedLines;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers
{
    public static class CommonVariousHelpers
    {
        /// <summary>
        /// Returns the URL with an appended Guid as a query string
        /// Specially useful for photos we do not want to get the cached version 
        /// The browser will always RELOAD the photo and not take the chached version (as it will think a query was made)
        /// </summary>
        public static string AppendQueryStringGuidToURL(string url)
        {
            url += "?" + Guid.NewGuid().ToString();
            return url;
        }

        /// <summary>
        /// Constructs a single string from all The Strings in a List , Appends all strings and seperates them with the provided charachter
        /// </summary>
        /// <param name="list">The List to convert into a single string</param>
        /// <param name="seperatorChar">The Charachter seperating each value</param>
        /// <param name="withSpaces">Wheather to Include spaces or Not between the seperator and the values (Default = true)</param>
        /// <returns>A string sequence of all the Values of the provided List - Seperated according to the provided parameters - string.Empty if string is null or Empty </returns>
        public static string GetStringOfList(IEnumerable<string>? list, char seperatorChar =',' , bool withSpaces = true)
        {
            if (list is null)
            {
                return "";
            }

            StringBuilder builder = new();
            foreach (var item in list)
            {
                builder.Append(item);
                if (withSpaces)
                {
                    builder.Append(' ').Append(seperatorChar).Append(' ');
                }
                else
                {
                    builder.Append(seperatorChar);
                }
            }
            if (withSpaces)
            {
                //Remove the Last occurence if " speratorChar "
                return builder.ToString().TrimEnd().TrimEnd(seperatorChar).TrimEnd();
            }
            else
            {
                return builder.ToString().TrimEnd(seperatorChar);
            }
        }

        /// <summary>
        /// Appends a Suffix to the End of a Path , if the Path contains a file it appends it Before the File Extension
        /// </summary>
        /// <param name="fullPath">The Full Path</param>
        /// <param name="suffix">The Suffix to Append</param>
        /// <returns>The New Path containing the Extension in the End</returns>
        public static string AppendSuffixToFullPath(string fullPath, string suffix)
        {
            //Remove the extension from the Url (.png)
            string fullPathWithoutExtension = Path.ChangeExtension(fullPath,null);
            //Get only the extension (the last 4 letters ex '.png')
            string fileExtension = Path.GetExtension(fullPath);

            //Build the new url containing the suffix
            StringBuilder builder = new();
            builder
                .Append(fullPathWithoutExtension)
                .Append(suffix)
                .Append(fileExtension);
            return builder.ToString();
        }

        public static bool AreEqual(params double[] values)
        {
            if (values.Length == 0) throw new Exception("No Values supplied...");
            if (values.Length < 2) return true;
            
            // Use the first element as the reference for comparison.
            double firstValue = values[0];
            // Compare every element in the array to the first one.
            for (int i = 1; i < values.Length; i++)
            {
                if (Math.Abs(values[i] - firstValue) > 0)
                {
                    // Found a value that is not equal (within tolerance) to the first one.
                    return false;
                }
            }
            // All values are equal (within tolerance).
            return true;
        }

#nullable disable
        /// <summary>
        /// Executes a Function if it meets the condition else returns the default Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">The Condition under which the function will be executed</param>
        /// <param name="function">The Function to execute that returns the result</param>
        /// <param name="defaultValue">The Default value if the condition is false</param>
        /// <returns></returns>
        public static T ExecuteIf<T>(bool condition, Func<T> function, T defaultValue = default)
        {
            if (condition)
            {
                return function.Invoke();
            }
            else
            {
                return defaultValue;
            }
        }

    }
#nullable enable
}

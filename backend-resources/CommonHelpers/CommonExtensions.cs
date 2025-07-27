using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers
{
    /// <summary>
    /// Contains Various Extension Methods
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// Compares the Values in a Dictionary with the Default Equality Comparer for each of teh TKey , TValue objects
        /// </summary>
        /// <typeparam name="TKey">The Type of the Key of the Dictionary</typeparam>
        /// <typeparam name="TValue">The Type of the Value of the Dictionary</typeparam>
        /// <param name="dict">The Dictionary</param>
        /// <param name="other">The Other Dictionary with which to compare this Dictionary</param>
        /// <param name="keyComparer">The Equality Comparer for the Keys of the Dictionary - Default if Null</param>
        /// <param name="valueComparer">The Equality Comparer for the Values of the Dictionary - Default if Null </param>
        /// <returns>True if all Keys are the Same and all their Returned Values are Also the Same , False Otherwise</returns>
        public static bool IsEqualToOtherDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> other, IEqualityComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null)
        {
            //If no Custom Comparers are Defined , Use the Default Ones 
            var comparerValue = valueComparer ?? EqualityComparer<TValue>.Default;
            var comparerKey = keyComparer ?? EqualityComparer<TKey>.Default;

            //Check if the Dictionaries have the Same Number of Items
            if (dict.Count != other.Count)
            {
                return false;
            }

            //Check whether their keys are Equal
            bool areKeysEqual = dict.Keys.SequenceEqual(other.Keys, comparerKey);
            if (!areKeysEqual)
            {
                return false;
            }

            //If one value is found not equal return
            foreach (var key in dict.Keys)
            {
                if (!comparerValue.Equals(dict[key], other[key]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Segregates a List into a Dictionary of Lists based on a Key Selector
        /// <para>If the Key Selector is null a single list will be provided under the key <paramref name="defaultNullKey"/> </para>
        /// </summary>
        /// <typeparam name="T">The type of elements in the list</typeparam>
        /// <param name="list">The list to segregate</param>
        /// <param name="keySelector">A function to extract a key from an element</param>
        /// <param name="defaultNullKey">The key to use when the keySelector returns null</param>
        /// <returns>A dictionary where the keys are the result of applying the keySelector to the elements of the list, and the values are lists of elements that share the same key</returns>
        /// <exception cref="ArgumentNullException">Thrown when keySelector is null</exception>
        public static Dictionary<object, List<T>> SegregateBy<T>(this IEnumerable<T> list, Func<T, object>? keySelector , string defaultNullKey = "nullGroupingKey")
        {
            //So the Func actually returns each time the value of the selected property
            //and the groupBy method actually uses this value as a key for the dictionary
            //If the selector function is null , return a single List under the default null key

            if (keySelector == null) return new Dictionary<object, List<T>>() { { defaultNullKey, list.ToList() } };
            else return list.GroupBy(keySelector).ToDictionary(g => g.Key ?? defaultNullKey, g => g.ToList());
        }

        /// <summary>
        /// Adds an Item to the List if the Condition is Met , otherwise it Doesn't
        /// </summary>
        /// <typeparam name="T">The Lists Object Type</typeparam>
        /// <param name="list">The List</param>
        /// <param name="condition">The Condition under which the item will be added</param>
        /// <param name="itemToAdd">The item to be added</param>
        public static void AddIf<T>(this List<T> list, bool condition, T itemToAdd)
        {
            if (condition) { list.Add(itemToAdd); }
        }

        /// <summary>
        /// Adds multiple Items to the List if the Condition is Met , otherwise it Doesn't
        /// </summary>
        /// <typeparam name="T">The Lists Object Type</typeparam>
        /// <param name="list">The List</param>
        /// <param name="condition">The Condition under which the items will be added</param>
        /// <param name="itemsToAdd">The items to be added</param>
        public static void AddIf<T>(this List<T> list, bool condition, IEnumerable<T> itemsToAdd)
        {
            if (condition) { list.AddRange(itemsToAdd); }
        }

        /// <summary>
        /// Adds an item to the List if it is of the Same Type
        /// Otherwise it does not
        /// </summary>
        /// <typeparam name="T">The Lists Item Type</typeparam>
        /// <param name="list">The List</param>
        /// <param name="itemToAdd">The Item to be Added to the List</param>
        /// <returns>True if item was Added false if it did not</returns>
        public static bool AddIfSameType<T>(this List<T> list, dynamic itemToAdd)
        {
            if (itemToAdd is T)
            {
                list.Add(itemToAdd);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Trys to Add an Item to the List if its not null , Otherwise it doesnot add it
        /// </summary>
        /// <typeparam name="T">The Type of the List's Objects</typeparam>
        /// <param name="list">The List</param>
        /// <param name="itemToAdd">The Item to Add</param>
        public static void AddNotNull<T>(this List<T> list, T? itemToAdd)
        {
            if (itemToAdd != null)
            {
                list.Add(itemToAdd);
            }
        }
        /// <summary>
        /// Trys to Add Items to the List if the enumerable is not null , Otherwise it doesnot add anything
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="items"></param>
        public static void AddRangeNotNull<T>(this List<T> list, IEnumerable<T>? items)
        {
            if (items != null)
            {
                list.AddRange(items);
            }
        }

        /// <summary>
        /// Returns weather the List has Duplicate Values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="comparer">A comparer on how to check for Duplicates otherwise null for the Default Comparer</param>
        /// <returns></returns>
        public static bool HasDuplicates<T>(this List<T> list, IEqualityComparer<T>? comparer = null)
            where T : class
        {
            return list.Count != list.Distinct(comparer).Count();
        }

        /// <summary>
        /// Removes the First Item matching the type from an ICollection<typeparamref name="T"/> 
        /// </summary>
        /// <typeparam name="T">The Type of items that the Collection Holds</typeparam>
        /// <param name="collection">The Collection</param>
        /// <param name="typeOfItemToRemove">The Type of Item to Remove</param>
        /// <returns>True if an item was found and Removed , False if nothing was Removed</returns>
        public static bool RemoveItemByType<T>(this ICollection<T> collection, Type typeOfItemToRemove)
        {
            var itemToRemove = collection.FirstOrDefault(i => i?.GetType() == typeOfItemToRemove);
            if (itemToRemove is not null)
            {
                return collection.Remove(itemToRemove);
            }
            else return false;
        }

        /// <summary>
        /// Removes the Diacritics from a String (For example greek accent { ' } )
        /// </summary>
        /// <param name="text">The Text from which to remove the Diacritics</param>
        /// <returns>The new string without diacritics</returns>
        public static string RemoveDiacritics(this string text)
        {
            //Normalizes the string by bringing it to a unicode form (diacritics are represented as unicode caracters)
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                //Checks if each charachter is not a diacritic and adds it to the return string , else scraps it
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Splits the input string into substrings of the specified size.
        /// </summary>
        /// <param name="input">The string to split.</param>
        /// <param name="size">The size of each substring.</param>
        /// <returns>A list of substrings.</returns>
        /// <exception cref="ArgumentException">Thrown when size is less than or equal to zero.</exception>
        public static List<string> SplitBySize(this string input, int size)
        {
            if (size < 0) throw new ArgumentException("Size must be greater than zero.", nameof(size));
            if(size == 0) return [input];

            var result = new List<string>();
            for (int i = 0; i < input.Length; i += size)
            {
                if (i + size > input.Length)
                    result.Add(input.Substring(i));
                else
                    result.Add(input.Substring(i, size));
            }
            return result;
        }

        /// <summary>
        /// Adds the Designated Number of Sapaces to the Front of the String
        /// </summary>
        /// <param name="text">The Text to add spaces to</param>
        /// <param name="numberOfSpaces">The Number of space charachters to add</param>
        /// <returns>The modified text</returns>
        public static string AddSpacesToFront(this string text, int numberOfSpaces)
        {
            string spaces = new(' ', numberOfSpaces);
            return $"{spaces}{text}";
        }
        /// <summary>
        /// Adds the Designated Number of Sapaces to the End of the String
        /// </summary>
        /// <param name="text">The Text to add spaces to</param>
        /// <param name="numberOfSpaces">The Number of space charachters to add</param>
        /// <returns>The modified text</returns>
        public static string AddSpacesToEnd(this string text, int numberOfSpaces)
        {
            string spaces = new(' ', numberOfSpaces);
            return $"{text}{spaces}";
        }
        /// <summary>
        /// Adds spaces to the Front and End of the String (thus padding the string)
        /// </summary>
        /// <param name="text">The Text in which to add the spaces to</param>
        /// <param name="numberOfSpaces">The Number of spaces that will be added in the Front and End of the text</param>
        /// <returns>The modified text</returns>
        public static string AddPadding(this string text, int numberOfSpaces)
        {
            return text.AddSpacesToFront(numberOfSpaces).AddSpacesToEnd(numberOfSpaces);
        }

        /// <summary>
        /// Devides a List of T items into seperate Lists containing the Provided number of Items , the last chunk has the remaining items
        /// </summary>
        /// <typeparam name="T">The Type of the List</typeparam>
        /// <param name="list">The List that will be divided into chunks</param>
        /// <param name="subListSize">the Chunk Size</param>
        /// <returns>A new IEnumerable containing the Divided Lists</returns>
        public static IEnumerable<List<T>> SplitList<T>(this List<T> list, int subListSize)
        {
            for (int i = 0; i < list.Count; i += subListSize)
            {
                yield return list.GetRange(i, Math.Min(subListSize, list.Count - i));
            }
        }

        public static readonly Dictionary<char, char> MapEnglishToGreek = new()
        {
            {'A','Α' },
            {'B','Β' },
            {'C','Ψ' },
            {'D','Δ' },
            {'E','Ε' },
            {'F','Φ' },
            {'G','Γ' },
            {'H','Η' },
            {'I','Ι' },
            {'J','Ξ' },
            {'K','Κ' },
            {'L','Λ' },
            {'M','Μ' },
            {'N','Ν' },
            {'O','Ο' },
            {'P','Π' },
            {'Q',';' },
            {'R','Ρ' },
            {'S','Σ' },
            {'T','Τ' },
            {'U','Θ' },
            {'V','Ω' },
            {'W','ς' },
            {'X','Χ' },
            {'Y','Υ' },
            {'Z','Ζ' },
        };

        public static readonly Dictionary<char, char> MapGreekToEnglish = new()
        {
            {'Α','A' },
            {'Β','B' },
            {'Ψ','C' },
            {'Δ','D' },
            {'Ε','E' },
            {'Φ','F' },
            {'Γ','G' },
            {'Η','H' },
            {'Ι','I' },
            {'Ξ','J' },
            {'Κ','K' },
            {'Λ','L' },
            {'Μ','M' },
            {'Ν','N' },
            {'Ο','O' },
            {'Π','P' },
            {';','Q' },
            {'Ρ','R' },
            {'Σ','S' },
            {'Τ','T' },
            {'Θ','U' },
            {'Ω','V' },
            {'ς','W' },
            {'Χ','X' },
            {'Υ','Y' },
            {'Ζ','Z' },
        };

        /// <summary>
        /// Transforms any Greek Charachters to Latin UpperInvariant (usually)
        /// </summary>
        /// <param name="text">The text to transform</param>
        /// <returns>The Replaced Text to an UpperInvariant Latin Charachters</returns>
        public static string NormalizeGreekToLatin(this string text)
        {
            text = text.ToUpperInvariant();
            StringBuilder builder = new(text.Length);

            foreach (var c in text)
            {
                //Replace with English if there is a ref in the Dictionary else put the same Charachter if there is no match in the dictionary
                builder.Append(MapGreekToEnglish.TryGetValue(c, out char englishChar) ? englishChar : c);
            }
            return builder.ToString();
        }
        /// <summary>
        /// Transforms any Latin Charachters to Greek UpperInvariant (usually)
        /// </summary>
        /// <param name="text">The text to transform</param>
        /// <returns>The Replaced Text to an UpperInvariant Greek Charachters</returns>
        public static string NormalizeLatinToGreek(this string text)
        {
            text = text.ToUpperInvariant();
            StringBuilder builder = new(text.Length);

            foreach (var c in text)
            {
                //Replace with English if there is a ref in the Dictionary else put the same greek Charachter
                builder.Append(MapEnglishToGreek.TryGetValue(c, out char greekChar) ? greekChar : c);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Adds a charachter to the end of the string if the string is smaller than the desired Length ,
        /// <para>Truncates the string if its length is bigger than the desired one</para>
        /// <para>returns the same string if its of the same charachters length as the desired one</para>
        /// <para>returns if the string is null , returns a filled string with the placeholder charachters</para>
        /// </summary>
        /// <param name="text">The text to modify</param>
        /// <param name="desiredCharachtersLength">The Desired length of the text</param>
        /// <param name="placeholderChar">The placeholder charachter to add if the text is smaller than the desired one</param>
        /// <returns></returns>
        public static string AddCharachtersIfSmallerTruncateIfBigger(this string? text, int desiredCharachtersLength, char placeholderChar)
        {
            if (text is null) return string.Empty.PadRight(desiredCharachtersLength, placeholderChar);
            if (text.Length == desiredCharachtersLength) return text;
            else if (text.Length < desiredCharachtersLength)
            {
                return text.PadRight(desiredCharachtersLength, placeholderChar);
            }
            //text is bigger than intented truncate it to the desired Length
            else
            {
                return text[..desiredCharachtersLength];
            }
        }

        /// <summary>
        /// Defines weather a charachter is greek
        /// </summary>
        /// <param name="c">The Charachter</param>
        /// <returns></returns>
        public static bool IsCharachterGreek(this char c)
        {
            //Greek characters in Unicode are part of the Greek and Coptic block, which is defined in the range U+0370 to U+03FF.
            if (c >= '\u0370' && c <= '\u03FF')
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the Description Attribute of an Enum Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo? fi = value.GetType().GetField(value.ToString());

            if (fi is null)
            {
                return value.ToString(); // Return the enum value's name if the field info is not found
            }

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        public static PropertyInfo? GetPropertyFromPath(this Type type, string propertyPath)
        {
            string[] propertyNames = propertyPath.Split('.');
            Type currentType = type;

            foreach (string propertyName in propertyNames)
            {
                //Distinguish if the current type is an Enumerable then get the 



                PropertyInfo? property = currentType.GetProperty(propertyName);

                if (property is null)
                {
                    return null; // Property not found
                }

                currentType = property.PropertyType;
            }
            var lastPropertyName = propertyNames.LastOrDefault() ?? throw new NotSupportedException($"Property Path {propertyPath} is not supported please use '.' Notation to traverse the object");
            return currentType.GetProperty(lastPropertyName); // Return the last property found
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers.Tests.CommonExtensions.Tests
{
    public class CommonExtensionsTests
    {
        /// <summary>
        /// Asserts wheather the Item has been added or not to the list , according to the Provided Condition
        /// </summary>
        /// <param name="condition">The Condition Boolean</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddIf_ShouldAddItemsOnCondition(bool condition)
        {
            List<int> list = new() { 1, 2, 3, 4, 5 };
            int itemToAdd = 13;
            list.AddIf(condition, itemToAdd);
            Assert.Equal(condition,list.Contains(itemToAdd));
        }

        /// <summary>
        /// Asserts whether the items have been added or not to the list according to being of the same type
        /// </summary>
        [Fact]
        public void AddIfSameType_ShouldAddItemsOfSameTypeAndPreventNonSameType()
        {
            List<int> list = new() { 1, 2 };
            int intToAdd = 5;
            string stringToAdd = "test";

            bool expectedtrue = list.AddIfSameType(intToAdd);
            bool expectedFalse = list.AddIfSameType(stringToAdd);

            Assert.True(list.Count == 3 && list.Contains(intToAdd) && expectedtrue && !expectedFalse);
        }

        /// <summary>
        /// Asserts wheather to add the items to the list , according to them being null or not null
        /// </summary>
        [Fact]
        public void AddNotNull_ShouldAddNotNullValueAndPreventNull()
        {
            List<int?> list = new() { 1, 2 };
            int? intToAdd = 0;
            int? nullInt = null;
            list.AddNotNull(intToAdd);
            list.AddNotNull(nullInt);
            Assert.True(list.Count == 3 && list.Contains(intToAdd));
        }

        /// <summary>
        /// Asserts wheather the Remove Diacritics Extension , removes the Diacritics from a string
        /// </summary>
        /// <param name="stringWithDiacritics">The String Containing Diacritics</param>
        /// <param name="expected">The Expected Result</param>
        [Theory]
        [InlineData("Γιώργος","Γιωργος")]
        [InlineData("Άλφα", "Αλφα")]
        [InlineData("Ένα Δύο Τρια One Two Three ,", "Ενα Δυο Τρια One Two Three ,")]
        public void RemoveDiacritics_ShouldReturnResultWithoutDiacritics(string stringWithDiacritics,string expected)
        {
            string actual = stringWithDiacritics.RemoveDiacritics();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(15, 2, 8)]
        [InlineData(5, 4, 2)]
        [InlineData(1, 3, 1)]
        [InlineData(0, 3, 0)]
        [InlineData(100, 3, 34)]
        public void SplitList_ShouldSplitTheListInGivenChunks(int listLength, int subListSize, int expectedChunks)
        {
            List<int> list = new();
            if (listLength != 0)
            {
                for (int i = 0; i < listLength; i++)
                {
                    list.Add(i);
                }
            }
            int actualChunks = list.SplitList(subListSize).Count();
            Assert.Equal(expectedChunks,actualChunks);
        }

    }
}

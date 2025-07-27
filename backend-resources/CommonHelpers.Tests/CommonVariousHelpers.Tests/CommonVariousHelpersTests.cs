using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers.Tests.CommonVariousHelpers.Tests
{
    public class CommonVariousHelpersTests
    {
        /// <summary>
        /// Asserts wheather a guid Has Been Appended to a certain string (url here)
        /// </summary>
        [Fact]
        public void AppendQueryStringGuidToUrl_ShouldAppendGuid()
        {
            string url = "testString";
            string GuidUrl = CommonHelpers.CommonVariousHelpers.AppendQueryStringGuidToURL(url);
            Assert.True(Guid.TryParse(GuidUrl.Replace($"{url}?", ""),out _));
        }

        /// <summary>
        /// Asserts wheather a List is properly formulated into a single string of values
        /// </summary>
        /// <param name="data"></param>
        /// <param name="expected"></param>
        /// <param name="withSpaces"></param>
        [Theory]
        [InlineData(new string[5] { "1", "2", "3", "4", "5" },"1,2,3,4,5", false)]
        [InlineData(new string[5] { "1", "2", "3", "4", "5" }, "1 , 2 , 3 , 4 , 5", true)]
        [InlineData(new string[1] { "1" }, "1", false)]
        [InlineData(new string[0], "",false)]
        public void GetStringOfList_ShouldJoinItemsProperly(string[] data , string expected , bool withSpaces)
        {
            List<string>? list = data?.ToList() ?? null;
            string actual = CommonHelpers.CommonVariousHelpers.GetStringOfList(list, ',', withSpaces);
            Assert.Equal(expected, actual);
        }
    }
}

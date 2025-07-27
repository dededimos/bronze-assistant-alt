namespace CommonHelpers.Tests.CalculationHelpers.Tests
{
    public class MatchToNearestTests
    {
        [Theory]
        [InlineData(5, new int[5]{1,2,3,4,7},4)]
        [InlineData(13, new int[20] { 1,2,12,17,25,42,64,39,5,1,90,16,125,37,99,98,97,17,17,5 }, 12)]
        [InlineData(5, new int[2] { 1,2 }, 2)]
        [InlineData(100, new int[1] { 1 }, 1)]
        public void MatchToNearest_ShouldReturnNearest(int numberToMatch,int[] listOfNumbers,int expected)
        {
            int actual = CommonHelpers.CalculationsHelpers.MatchToNearest(numberToMatch, listOfNumbers.ToList());

            Assert.Equal(expected, actual);
        }
    }
}
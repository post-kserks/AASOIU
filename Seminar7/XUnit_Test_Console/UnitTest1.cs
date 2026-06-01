using Seminar7;

namespace XUnit_Test_Console
{
    public class UnitTest1
    {
        [Fact]
        public void Calculate_ThreeDaysOverdue_Returns15()
        {
            // Arrange
            var rule = new RegularFineRule();

            // Act
            decimal fine = rule.Calculate(daysOverdue: 3);

            // Assert
            Assert.Equal(15m, fine);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 5)]
        [InlineData(3, 15)]
        [InlineData(10, 50)]
        public void Calculate_VariousDays_ReturnsCorrectFine(int days, decimal expected)
        {
            var rule = new RegularFineRule();
            Assert.Equal(expected, rule.Calculate(days));
        }

        [Fact]
        public void Senior_OverdueByFirstSevenDays_NoFine()
        {
            var rule = new SeniorFineRule();
            Assert.Equal(0m, rule.Calculate(daysOverdue: 7));
        }
    }
}

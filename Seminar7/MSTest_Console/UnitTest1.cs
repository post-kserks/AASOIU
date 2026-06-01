using Seminar7;

namespace MSTest_Console
{
    [TestClass]
    public sealed class UnitTest1
    {
        [TestMethod]
        public void Calculate_ThreeDaysOverdue_Returns15()
        {
            // Arrange
            var rule = new RegularFineRule();

            // Act
            decimal fine = rule.Calculate(daysOverdue: 3);

            // Assert
            Assert.AreEqual(15m, fine);
        }

        [TestMethod]
        [DataRow(0, "0")]
        [DataRow(1, "5")]
        [DataRow(3, "15")]
        [DataRow(10, "50")]
        public void Calculate_VariousDays_ReturnsCorrectFine(int days, string expectedStr)
        {
            // Преобразуем строку в decimal
            decimal expected = decimal.Parse(expectedStr, System.Globalization.CultureInfo.InvariantCulture);
            var rule = new RegularFineRule();
            Assert.AreEqual(expected, rule.Calculate(days));
        }
    }
}

using Seminar7;

namespace NUnit_Test_Console
{
    public class Tests
    {
        [Test]
        public void Calculate_ThreeDaysOverdue_Returns15()
        {
            // Arrange
            var rule = new RegularFineRule();

            // Act
            decimal fine = rule.Calculate(daysOverdue: 3);

            // Assert
            Assert.That(fine, Is.EqualTo(15m));
        }

        [TestCase(0, 0)]
        [TestCase(1, 5)]
        [TestCase(3, 15)]
        [TestCase(10, 50)]
        public void Calculate_VariousDays_ReturnsCorrectFine(int days, decimal expected)
        {
            var rule = new RegularFineRule();
            Assert.That(rule.Calculate(days), Is.EqualTo(expected));
        }
    }
}

using Seminar7;

namespace XUnit_Test_Console
{
    public class FamilyFineRuleTests
    {
        [Fact]
        public void Calculate_WithinGracePeriod_ReturnsZero()
        {
            var rule = new FamilyFineRule();
            Assert.Equal(0m, rule.Calculate(daysOverdue: 3));
        }

        [Fact]
        public void Calculate_AfterGracePeriod_ChargesTwoRublesPerDay()
        {
            var rule = new FamilyFineRule();
            // 8 - 5 льготных = 3 платных дня × 2 рубля = 6
            Assert.Equal(6m, rule.Calculate(daysOverdue: 8));
        }

        [Fact]
        public void Calculate_CapAt100Rubles()
        {
            var rule = new FamilyFineRule();
            // (100 - 5) * 2 = 190, should be 100
            Assert.Equal(100m, rule.Calculate(daysOverdue: 100));
        }
    }
}

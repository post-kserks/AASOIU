using Moq;
using Seminar7;
using Shouldly;

namespace XUnit_Test_Console
{
    public class BonusCalculatorTests
    {
        [Fact]
        public void Calculate_FullTimeSeniorInDecember_Returns15PercentAndLogsEvent()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                Name = "Иванов",
                Salary = 100_000m,
                YearsOfExperience = 7,
                IsFullTime = true
            };

            // Создаём моки
            var repositoryMock = new Mock<IEmployeeRepository>();
            var clockMock = new Mock<IClock>();
            var loggerMock = new Mock<ILogger>();

            // Настраиваем возвращаемые значения (роль Stub)
            repositoryMock.Setup(r => r.GetById(1)).Returns(employee);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2025, 12, 15));

            var calculator = new BonusCalculator(
                repositoryMock.Object, // .Object — собственно объект-дублёр
                clockMock.Object,
                loggerMock.Object);

            // Act
            decimal bonus = calculator.Calculate(employeeId: 1);

            // Assert — состояние
            bonus.ShouldBe(15_000m);

            // Assert — взаимодействие (роль Mock)
            loggerMock.Verify(
                l => l.Log(It.Is<string>(s => s.Contains("Иванов"))),
                Times.Once);
        }

        [Fact]
        public void Calculate_NotDecember_ReturnsZero()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                Name = "Иванов",
                Salary = 100_000m,
                YearsOfExperience = 7,
                IsFullTime = true
            };

            var repositoryMock = new Mock<IEmployeeRepository>();
            var clockMock = new Mock<IClock>();
            var loggerMock = new Mock<ILogger>();

            repositoryMock.Setup(r => r.GetById(1)).Returns(employee);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2025, 6, 15)); // June

            var calculator = new BonusCalculator(
                repositoryMock.Object,
                clockMock.Object,
                loggerMock.Object);

            // Act
            decimal bonus = calculator.Calculate(employeeId: 1);

            // Assert
            bonus.ShouldBe(0m);
        }
    }

    public class FineCalculatorTests : IDisposable
    {
        private readonly FineCalculator _calculator;

        public FineCalculatorTests() // Перед каждым тестом
        {
            _calculator = new FineCalculator(new RegularFineRule());
        }

        public void Dispose() { /* После каждого теста */ }

        [Fact]
        public void Calculate_ThreeDays_Returns15()
        {
            Assert.Equal(15m, _calculator.Calculate(3));
        }
    }
}

using System;

namespace Seminar7;

public class BonusCalculator
{
    private readonly IEmployeeRepository _repository;
    private readonly IClock _clock;
    private readonly ILogger _logger;

    public BonusCalculator(IEmployeeRepository repository, IClock clock, ILogger logger)
    {
        _repository = repository;
        _clock = clock;
        _logger = logger;
    }

    public decimal Calculate(int employeeId)
    {
        Employee employee = _repository.GetById(employeeId) 
            ?? throw new ArgumentException($"Сотрудник {employeeId} не найден");

        if (_clock.Now.Month != 12) return 0m;

        _logger.Log($"Расчёт премии для {employee.Name}");

        if (!employee.IsFullTime)
            return employee.Salary * 0.07m;

        return employee.YearsOfExperience > 5
            ? employee.Salary * 0.15m
            : employee.Salary * 0.12m;
    }
}

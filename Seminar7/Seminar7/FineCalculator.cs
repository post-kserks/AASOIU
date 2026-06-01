namespace Seminar7;

public class FineCalculator
{
    private readonly IFineRule _rule;

    public FineCalculator(IFineRule rule)
    {
        _rule = rule;
    }

    public decimal Calculate(int daysOverdue)
    {
        return _rule.Calculate(daysOverdue);
    }
}

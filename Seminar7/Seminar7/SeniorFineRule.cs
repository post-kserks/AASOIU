namespace Seminar7;

public class SeniorFineRule : IFineRule
{
    public decimal Calculate(int daysOverdue)
    {
        if (daysOverdue <= 7) return 0m;
        return (daysOverdue - 7) * 3m;
    }
}

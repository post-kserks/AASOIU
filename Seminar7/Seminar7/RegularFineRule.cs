namespace Seminar7;

public class RegularFineRule : IFineRule
{
    public decimal Calculate(int daysOverdue) => daysOverdue * 5m;
}

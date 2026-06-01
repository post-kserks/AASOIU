using System;

namespace Seminar7;

public class FamilyFineRule : IFineRule
{
    public decimal Calculate(int daysOverdue)
    {
        if (daysOverdue <= 5) return 0m;
        
        decimal fine = (daysOverdue - 5) * 2m;
        return Math.Min(fine, 100m);
    }
}

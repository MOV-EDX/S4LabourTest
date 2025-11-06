using PayrollLogic.Enums;

namespace PayrollLogic.Model.Interface
{
    public interface IEmployee
    {
        string Forename { get; }
        string Surname { get; }
        DateOnly EmploymentStartDate { get; }
        PayFrequency PayFrequency { get; }
        decimal RateOfPay { get; }
        SickPay SickPayScheme { get; }
        Deduction Deduction { get; }

        string GetFullName();
        void AddDeduction(Deduction type);
        decimal CalculateSalary(DateTime weekStart, int hours, int minutes, int sickDays);
    }
}

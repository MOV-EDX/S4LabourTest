using PayrollLogic.Enums;

namespace PayrollLogic.Model.Interface
{
    public interface IEmployee
    {
        string Forename { get; }
        string Surname { get; }
        DateOnly EmploymentStartDate { get; }
        PayFrequency PayFrequency { get; }

        /// <summary>
        /// This can be either the annual salary, weekly pay or hourly rate depending on the pay frequency.
        /// </summary>
        decimal RateOfPay { get; }
        SickPay SickPayScheme { get; }
        Deduction Deduction { get; }

        /// <summary>
        /// Gets the full name of the employee which is the concatentation of the forename and surname.
        /// </summary>
        /// <returns>The full name of the employee.</returns>
        string GetFullName();

        /// <summary>
        /// Add a deduction to the employee. A employee can have multiple deductions.
        /// </summary>
        /// <param name="type">The type of deduction to be applied to the employee.</param>
        void AddDeduction(Deduction type);

        /// <summary>
        /// Calculates the salary / labour cost of the employee for the given week.
        /// </summary>
        /// <param name="weekStart">The date which the current week commences on.</param>
        /// <param name="hours">The number of hours which have been worked. This must be 0 or greater.</param>
        /// <param name="minutes">The number of minutes which have been worked. This must be 0 or greater.</param>
        /// <param name="sickDays">The number of sick days. This must be 0 or greater.</param>
        /// <returns>The total labour cost rounded to 2 decimal places.</returns>
        decimal CalculateSalary(DateTime weekStart, int hours, int minutes, int sickDays);
    }
}

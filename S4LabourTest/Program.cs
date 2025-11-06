using PayrollLogic.Enums;
using PayrollLogic.Model;
using PayrollLogic.Model.Interface;

namespace S4LabourTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IEmployee salaryWithCOSP = Employee.CreateEmployee("Eric", "Wimp", new DateTime(2020, 01, 01), PayFrequency.Annual, 26000m, SickPay.COSP);
            salaryWithCOSP.AddDeduction(Deduction.Pension);
            salaryWithCOSP.AddDeduction(Deduction.BikeScheme);

            IEmployee salaryWithoutCOSP = Employee.CreateEmployee("Peter", "Parker", new
            DateTime(2025, 01, 01), PayFrequency.Annual, 25397m, SickPay.SSP);

            IEmployee weeklyWithoutCOSP = Employee.CreateEmployee("Clark", "Kent", new
            DateTime(2025, 06, 01), PayFrequency.Weekly, 480m, SickPay.SSP);

            IEmployee hourlyWithCOSP = Employee.CreateEmployee("Bruce", "Wayne", new
            DateTime(2020, 01, 01), PayFrequency.Hourly, 9m, SickPay.COSP);

            IEmployee hourlyWithoutCOSP = Employee.CreateEmployee("Wade", "Wilson", new
            DateTime(2024, 01, 01), PayFrequency.Hourly, 8m, SickPay.SSP);

            DateTime weekStart = DateTime.Now.Date;
            DayOfWeek dow = weekStart.DayOfWeek;
            switch (dow)
            {
                case DayOfWeek.Sunday: { weekStart = weekStart.AddDays(-6); break; }
                case DayOfWeek.Saturday: { weekStart = weekStart.AddDays(-5); break; }
                case DayOfWeek.Friday: { weekStart = weekStart.AddDays(-4); break; }
                case DayOfWeek.Thursday: { weekStart = weekStart.AddDays(-3); break; }
                case DayOfWeek.Wednesday: { weekStart = weekStart.AddDays(-2); break; }
                case DayOfWeek.Tuesday: { weekStart = weekStart.AddDays(-1); break; }
            }

            Console.WriteLine($"{salaryWithCOSP.GetFullName()}: {salaryWithCOSP.CalculateSalary(weekStart, 40, 0, 7)}");
            Console.WriteLine($"{salaryWithoutCOSP.GetFullName()}: {salaryWithoutCOSP.CalculateSalary(weekStart, 24, 0, 2)} ");
            Console.WriteLine($"{weeklyWithoutCOSP.GetFullName()}: {weeklyWithoutCOSP.CalculateSalary(weekStart, 40, 0, 0)} ");
            Console.WriteLine($"{hourlyWithCOSP.GetFullName()}: {hourlyWithCOSP.CalculateSalary(weekStart, 24, 0, 2)} ");
            Console.WriteLine($"{hourlyWithoutCOSP.GetFullName()}: {hourlyWithoutCOSP.CalculateSalary(weekStart, 40, 0, 0)} ");

            Console.ReadLine();
        }
    }
}

using PayrollLogic.Constants;
using PayrollLogic.Enums;
using PayrollLogic.Model.Interface;

namespace PayrollLogic.Model
{
    public class Employee : IEmployee
    {
        private decimal BasicPayRate { get; set; }

        private Employee() { }

        public static Employee CreateEmployee(string forename, string surname, DateTime employmentStartDate, PayFrequency payFrequency, decimal rateOfPay, SickPay sickPay)
        {
            if (string.IsNullOrWhiteSpace(forename))
            {
                throw new ArgumentException($"{nameof(Forename)} can't be null, empty or whitespace");
            }

            if (string.IsNullOrWhiteSpace(surname)) 
            {
                throw new ArgumentException($"{nameof(Surname)} can't be null, empty or whitespace");
            }

            var employee = new Employee
            {
                Forename = forename,
                Surname = surname,
                EmploymentStartDate = DateOnly.FromDateTime(employmentStartDate),
                PayFrequency = payFrequency,
                RateOfPay = rateOfPay,
                SickPayScheme = sickPay
            };

            employee.BasicPayRate = employee.PayFrequency switch
            {
                PayFrequency.Annual => employee.RateOfPay / 52,
                PayFrequency.Weekly => employee.RateOfPay,
                PayFrequency.Hourly => employee.RateOfPay,
                _ => throw new InvalidOperationException($"{nameof(PayFrequency)} has not been specified")
            };

            return employee;
        }

        public string Forename { get; private set; } = string.Empty;
        public string Surname { get; private set; } = string.Empty;
        public DateOnly EmploymentStartDate { get; private set; }
        public PayFrequency PayFrequency { get; private set; }
        public decimal RateOfPay { get; private set; }
        public SickPay SickPayScheme { get; private set; }
        public Deduction Deduction { get; private set; }

        public void AddDeduction(Deduction type)
        {
            Deduction |= type;
        }

        public decimal CalculateSalary(DateTime weekStart, int hours, int minutes, int sickDays)
        {
            if (hours < 0)
            {
                throw new ArgumentException($"{nameof(hours)} must be 0 or greater");
            }

            if (minutes < 0)
            {
                throw new ArgumentException($"{nameof(minutes)} must be 0 or greater");
            }

            if (sickDays < 0)
            {
                throw new ArgumentException($"{nameof(sickDays)} must be 0 or greater");
            }

            var weeklyPay = CalculateWeeklyPay(hours, minutes, sickDays);
            var sickPay = CalculateSickPay(DateOnly.FromDateTime(weekStart), sickDays);
            var totalDeductions = CalculateDeductions(weeklyPay, sickPay);

            var totalWeeklyPay = CalculateTotalWeeklyPay(weeklyPay, sickPay, totalDeductions);
            var nationalInsurance = CalculateNIContribution(totalWeeklyPay);
            var holidayAccural = CalculateHolidayAccural(totalWeeklyPay, nationalInsurance);

            var finalLabourCost = totalWeeklyPay + nationalInsurance + holidayAccural;

            return Math.Round(finalLabourCost, 2);
        }

        public string GetFullName()
        {
            return $"{Forename} {Surname}";
        }

        private decimal CalculateHolidayAccural(decimal totalWeeklyPay, decimal nationalInsurance)
        {
            return (totalWeeklyPay + nationalInsurance) * 0.1207m;
        }

        private decimal CalculateNIContribution(decimal totalWeeklyPay)
        {
            var calculatedNI = (totalWeeklyPay - NationalInsurance.Threshold) * NationalInsurance.Percentage;
            return calculatedNI > 0 ? calculatedNI : 0;
        }

        private decimal CalculateTotalWeeklyPay(decimal weeklyPay, decimal sickPay, decimal totalDeductions)
        {
            return weeklyPay + sickPay - (totalDeductions);
        }

        private decimal CalculateWeeklyPay(int hours, int minutes, int sickDays)
        {
            var weeklyPay = 0m;

            if (PayFrequency is PayFrequency.Annual || PayFrequency is PayFrequency.Weekly)
            {
                weeklyPay = sickDays switch
                {
                    <= 0 => BasicPayRate,
                    < 5 and > 0 => BasicPayRate - (BasicPayRate / 5) * sickDays,
                    >= 5 => 0
                };
            }
            else
            {
                weeklyPay = BasicPayRate * hours + (BasicPayRate * minutes / 60);
            }

            return weeklyPay > 0 ? weeklyPay : 0;
        }

        private decimal CalculateSickPay(DateOnly weekStarting, int sickDays)
        {
            if (sickDays < 0)
            {
                throw new ArgumentException($"{nameof(sickDays)} can't be less than 0");
            }

            // Check if the employee has been set as COSP, and have worked for at least 5 years at the company
            if (SickPayScheme is SickPay.COSP && PayFrequency is not PayFrequency.Hourly && (EmploymentStartDate < weekStarting.AddYears(-5)))
            {
                return sickDays switch
                {
                    < 5 and > 0 => BasicPayRate / 5 * sickDays,
                    >= 5 => BasicPayRate,
                    _ => 0
                };
            }

            // Calculate the SSP, this is paid up to a maximum of 7 days and must be less than the SSP rate
            if (sickDays > 0)
            {
                if (sickDays <= 7)
                {
                    return StatutorySickPay.Rate / 7 * sickDays;
                }
                else
                {
                    return StatutorySickPay.Rate;
                }
            }

            return 0;
        }

        private decimal CalculateDeductions(decimal weeklyPayRate, decimal sickPay)
        {
            var totalDeduction = 0m;

            if (Deduction.HasFlag(Deduction.Pension))
            {
                totalDeduction += (weeklyPayRate + sickPay) * 0.02m;
            }

            if (Deduction.HasFlag(Deduction.BikeScheme))
            {
                totalDeduction += 75m;
            }

            return totalDeduction;
        }
    }
}

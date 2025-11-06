using PayrollLogic.Enums;
using PayrollLogic.Model;
using System.Security.Cryptography;

namespace PayrollLogic.Tests
{
    [TestClass]
    public sealed class EmployeeTests
    {
        [TestMethod]
        [DataRow(new[] { Deduction.BikeScheme })]
        [DataRow(new[] { Deduction.BikeScheme, Deduction.Pension })]
        public void AddDeductionTest(IEnumerable<Deduction> deductions)
        {
            // Arrange
            var employee = Employee.CreateEmployee("Test", "Testing", DateTime.Now, PayFrequency.Annual, 1m, SickPay.SSP);

            // Act
            foreach (var deduction in deductions)
            {
                employee.AddDeduction(deduction);
            }

            // Assert
            foreach (var deduction in deductions)
            {
                Assert.IsTrue(employee.Deduction.HasFlag(deduction));
            }

            // HasFlag always returns true for 0 (None) values
            Assert.IsFalse(employee.Deduction.Equals(Deduction.None));
        }

        [TestMethod]
        public void GetFullNameTest()
        {
            // Arrange
            var employee = Employee.CreateEmployee("Test", "Testing", DateTime.Now, PayFrequency.Annual, 1m, SickPay.SSP);

            // Act
            var result = employee.GetFullName();

            // Assert
            Assert.AreEqual("Test Testing", result);
        }

        /// <summary>
        /// Tests that an annual employee who has been with the company for less than 3 months is using SSP
        /// </summary>
        [TestMethod]
        public void CalculateSalary_AnnualCOSPEmployee_LessThan5Years_Test()
        {
            // Arrange
            var employee = Employee.CreateEmployee("Test", "Testing", DateTime.Now.AddMonths(-3), PayFrequency.Annual, 20000m, SickPay.COSP);

            // Act
            var result = employee.CalculateSalary(DateTime.Now, 35, 0, 1);

            // Assert
            Assert.AreEqual(result, 402.28m);
        }

        [TestMethod]
        [DataRow(1, 479.56)]
        [DataRow(5, 479.56)]
        [DataRow(0, 479.56)]
        public void CalculateSalary_AnnualCOSPEmployee_6Years_Test(int sickDays, double expectedSalary)
        {
            // Arrange
            var employee = Employee.CreateEmployee("Test", "Testing", DateTime.Now.AddYears(-6), PayFrequency.Annual, 20000m, SickPay.COSP);

            // Act
            var result = employee.CalculateSalary(DateTime.Now, 35, 0, sickDays);

            // Assert
            Assert.AreEqual(result, (decimal) expectedSalary);
        }

        [TestMethod]
        [DataRow(new[] { Deduction.BikeScheme }, 382.90)]
        [DataRow(new[] { Deduction.BikeScheme, Deduction.Pension }, 372.98)]
        public void CalculateSalary_AnnualCOSPEmployee_DeductionsTest(IEnumerable<Deduction> appliedDeductions, double expectedSalary)
        {
            // Arrange
            var employee = Employee.CreateEmployee("Test", "Testing", DateTime.Now.AddYears(-6), PayFrequency.Annual, 20000m, SickPay.COSP);

            foreach (var deduction in appliedDeductions)
            {
                employee.AddDeduction(deduction);
            }

            // Act
            var result = employee.CalculateSalary(DateTime.Now, 35, 0, 0);

            // Assert
            Assert.AreEqual(result, (decimal)expectedSalary);
        }

        [TestMethod]
        [DataRow(1, 569.58)]
        [DataRow(7, 700.76)]
        [DataRow(8, 700.76)]
        public void CalculateSalary_HourlyEmployee_Test(int sickDays, double expectedSalary)
        {
            // Arrange
            var employee = Employee.CreateEmployee("Test", "Testing", DateTime.Now.AddYears(-1), PayFrequency.Hourly, 17.50m, SickPay.SSP);

            // Act
            var result = employee.CalculateSalary(DateTime.Now, 25, 0, sickDays);

            // Assert
            Assert.AreEqual(result, (decimal)expectedSalary);
        }

        [TestMethod]
        [DataRow(-1, 1, 1)]
        [DataRow(1, -1, 1)]
        [DataRow(1, 1, -1)]
        public void CalculateSalary_InvalidArguments_Test(int hours, int minutes, int sickDays)
        {
            // Arrange
            var employee = Employee.CreateEmployee("Test", "Testing", DateTime.Now.AddYears(-1), PayFrequency.Hourly, 17.50m, SickPay.SSP);

            // Act, Assert
            Assert.ThrowsException<ArgumentException>(() => employee.CalculateSalary(DateTime.Now, hours, minutes, sickDays));
        }

        [TestMethod]
        public void CreateEmployeeTest()
        {
            // Arrange, Act
            var employee = Employee.CreateEmployee("Test", "Testing", DateTime.Now.AddYears(-1), PayFrequency.Hourly, 17.50m, SickPay.SSP);

            // Assert
            Assert.IsNotNull(employee);
        }

        [TestMethod]
        [DataRow("", "Test")]
        [DataRow(" ", "Test")]
        [DataRow(null, "Test")]
        [DataRow("Test", "")]
        [DataRow("Test", " ")]
        [DataRow("Test", null)]
        public void CreateEmployee_InvalidName_Test(string forename, string surname)
        {
            // Arrange, Act, Assert
            Assert.ThrowsException<ArgumentException>(() => Employee.CreateEmployee(forename, surname, DateTime.Now, PayFrequency.Annual, 20000m, SickPay.SSP));
        }
    }
}

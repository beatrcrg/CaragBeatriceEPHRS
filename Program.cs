using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EmployeePayrollHRMS
{
    // ===== Data Model (10 columns) =====
    public class Employee
    {
        public int Id { get; set; }                 // 1
        public string FirstName { get; set; }       // 2
        public string LastName { get; set; }        // 3
        public string Department { get; set; }      // 4
        public string JobTitle { get; set; }        // 5
        public decimal MonthlySalary { get; set; }  // 6
        public double TaxRate { get; set; }         // 7
        public DateTime HireDate { get; set; }      // 8
        public bool IsActive { get; set; }          // 9
        public string Email { get; set; }           // 10

        public decimal TaxAmount => MonthlySalary * (decimal)TaxRate;
        public decimal NetPay => MonthlySalary - TaxAmount;
    }

    class Program
    {
        // Starts EMPTY (0 pre-defined records)
        static List<Employee> employees = new List<Employee>();
        static int nextId = 1001;

        // >>> Line used for the deliberate merge conflict (Step 4) <<<
        const string AppTitle = "EMPLOYEE PAYROLL & HR MANAGEMENT SYSTEM (FEATURE BRANCH)";

        static void Main()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("==============================================");
                Console.WriteLine("  " + AppTitle);
                Console.WriteLine("==============================================");
                Console.WriteLine("  1. Create Employee");
                Console.WriteLine("  2. Read / Display All Employees");
                Console.WriteLine("  3. Search / Filter Employees");
                Console.WriteLine("  4. Update Employee");
                Console.WriteLine("  5. Delete Employee");
                Console.WriteLine("  6. Exit");
                Console.Write("Choose an option (1-6): ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": CreateEmployee(); break;
                    case "2": DisplayEmployees(employees, "ALL EMPLOYEES"); break;
                    case "3": SearchMenu(); break;
                    case "4": UpdateEmployee(); break;
                    case "5": DeleteEmployee(); break;
                    case "6": Console.WriteLine("Goodbye!"); return;
                    default: Console.WriteLine("Invalid option. Try again."); break;
                }
            }
        }

        // ===== CREATE =====
        static void CreateEmployee()
        {
            Console.WriteLine("\n--- CREATE EMPLOYEE ---");
            var emp = new Employee
            {
                Id = nextId++,
                FirstName = ReadString("First Name: "),
                LastName = ReadString("Last Name: "),
                Department = ReadString("Department (IT/HR/Sales/Finance): "),
                JobTitle = ReadString("Job Title: "),
                MonthlySalary = ReadDecimal("Monthly Salary: "),
                TaxRate = ReadTaxRate("Tax Rate (e.g., 0.12 for 12%): "),
                HireDate = ReadDate("Hire Date (yyyy-MM-dd): "),
                IsActive = ReadBool("Is Active? (y/n): "),
                Email = ReadString("Email: ")
            };
            employees.Add(emp);
            Console.WriteLine($"\n[SUCCESS] Employee {emp.FirstName} {emp.LastName} created with ID {emp.Id}.");
        }

        // ===== READ =====
        static void DisplayEmployees(IEnumerable<Employee> list, string title)
        {
            var data = list.OrderBy(e => e.Id).ToList();
            Console.WriteLine($"\n--- {title} ({data.Count} record/s) ---");
            if (!data.Any())
            {
                Console.WriteLine("No records found.");
                return;
            }

            string header = string.Format("{0,-5} {1,-10} {2,-10} {3,-10} {4,-18} {5,12} {6,6} {7,-11} {8,-8} {9,-28} {10,12}",
                "ID", "First", "Last", "Dept", "Job Title", "Salary", "Tax", "Hire Date", "Active", "Email", "Net Pay");
            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));
            foreach (var e in data)
            {
                Console.WriteLine("{0,-5} {1,-10} {2,-10} {3,-10} {4,-18} {5,12:N2} {6,6:P0} {7,-11:yyyy-MM-dd} {8,-8} {9,-28} {10,12:N2}",
                    e.Id, e.FirstName, e.LastName, e.Department, e.JobTitle, e.MonthlySalary,
                    e.TaxRate, e.HireDate, e.IsActive ? "Active" : "Inactive", e.Email, e.NetPay);
            }
            Console.WriteLine($"\nTotal Monthly Payroll (Net): {data.Sum(e => e.NetPay):N2}");
        }

        // ===== SEARCH / FILTER (LINQ) =====
        static void SearchMenu()
        {
            Console.WriteLine("\n--- SEARCH / FILTER ---");
            Console.WriteLine("1. By ID");
            Console.WriteLine("2. By Name (first or last)");
            Console.WriteLine("3. By Department");
            Console.WriteLine("4. By Active Status");
            Console.WriteLine("5. By Minimum Salary");
            Console.Write("Choose: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    int id = ReadInt("Enter ID: ");
                    DisplayEmployees(employees.Where(e => e.Id == id), $"RESULT: ID = {id}");
                    break;
                case "2":
                    string name = ReadString("Enter name keyword: ");
                    DisplayEmployees(employees.Where(e =>
                        e.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                        e.LastName.Contains(name, StringComparison.OrdinalIgnoreCase)),
                        $"RESULT: NAME contains '{name}'");
                    break;
                case "3":
                    string dept = ReadString("Enter department: ");
                    DisplayEmployees(employees.Where(e =>
                        e.Department.Equals(dept, StringComparison.OrdinalIgnoreCase)),
                        $"RESULT: DEPARTMENT = {dept}");
                    break;
                case "4":
                    bool active = ReadBool("Active employees? (y = Active, n = Inactive): ");
                    DisplayEmployees(employees.Where(e => e.IsActive == active),
                        $"RESULT: {(active ? "ACTIVE" : "INACTIVE")} EMPLOYEES");
                    break;
                case "5":
                    decimal min = ReadDecimal("Minimum monthly salary: ");
                    DisplayEmployees(employees.Where(e => e.MonthlySalary >= min),
                        $"RESULT: SALARY >= {min:N2}");
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        // ===== UPDATE =====
        static void UpdateEmployee()
        {
            Console.WriteLine("\n--- UPDATE EMPLOYEE ---");
            int id = ReadInt("Enter Employee ID to update: ");
            var emp = employees.FirstOrDefault(e => e.Id == id);
            if (emp == null)
            {
                Console.WriteLine($"[ERROR] No employee with ID {id}.");
                return;
            }

            Console.WriteLine("(Press Enter to keep the current value)");
            emp.FirstName = ReadOptional($"First Name [{emp.FirstName}]: ", emp.FirstName);
            emp.LastName = ReadOptional($"Last Name [{emp.LastName}]: ", emp.LastName);
            emp.Department = ReadOptional($"Department [{emp.Department}]: ", emp.Department);
            emp.JobTitle = ReadOptional($"Job Title [{emp.JobTitle}]: ", emp.JobTitle);

            string sal = ReadOptional($"Monthly Salary [{emp.MonthlySalary}]: ", "");
            if (decimal.TryParse(sal, NumberStyles.Number, CultureInfo.InvariantCulture, out var s) && s >= 0)
                emp.MonthlySalary = s;

            string tax = ReadOptional($"Tax Rate [{emp.TaxRate}]: ", "");
            if (double.TryParse(tax, NumberStyles.Float, CultureInfo.InvariantCulture, out var t) && t >= 0 && t <= 1)
                emp.TaxRate = t;

            string hd = ReadOptional($"Hire Date [{emp.HireDate:yyyy-MM-dd}]: ", "");
            if (DateTime.TryParse(hd, out var d)) emp.HireDate = d;

            string act = ReadOptional($"Is Active (y/n) [{(emp.IsActive ? "y" : "n")}]: ", "");
            if (act.Equals("y", StringComparison.OrdinalIgnoreCase)) emp.IsActive = true;
            else if (act.Equals("n", StringComparison.OrdinalIgnoreCase)) emp.IsActive = false;

            emp.Email = ReadOptional($"Email [{emp.Email}]: ", emp.Email);

            Console.WriteLine($"\n[SUCCESS] Employee {emp.Id} updated.");
            DisplayEmployees(employees.Where(e => e.Id == id), "UPDATED RECORD");
        }

        // ===== DELETE =====
        static void DeleteEmployee()
        {
            Console.WriteLine("\n--- DELETE EMPLOYEE ---");
            int id = ReadInt("Enter Employee ID to delete: ");
            var emp = employees.FirstOrDefault(e => e.Id == id);
            if (emp == null)
            {
                Console.WriteLine($"[ERROR] No employee with ID {id}.");
                return;
            }

            if (ReadBool($"Delete {emp.FirstName} {emp.LastName}? (y/n): "))
            {
                employees.RemoveAll(e => e.Id == id);
                Console.WriteLine($"[SUCCESS] Employee {id} deleted. Remaining: {employees.Count}");
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }
        }

        // ===== INPUT HELPERS (validation) =====
        static string ReadString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(s)) return s;
                Console.WriteLine("Input cannot be empty.");
            }
        }

        static string ReadOptional(string prompt, string current)
        {
            Console.Write(prompt);
            string s = Console.ReadLine()?.Trim();
            return string.IsNullOrWhiteSpace(s) ? current : s;
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int v)) return v;
                Console.WriteLine("Enter a valid whole number.");
            }
        }

        static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal v) && v >= 0)
                    return v;
                Console.WriteLine("Enter a valid non-negative amount.");
            }
        }

        static double ReadTaxRate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out double v) && v >= 0 && v <= 1)
                    return v;
                Console.WriteLine("Enter a rate between 0 and 1 (e.g., 0.12).");
            }
        }

        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParse(Console.ReadLine(), out DateTime v)) return v;
                Console.WriteLine("Enter a valid date (yyyy-MM-dd).");
            }
        }

        static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine()?.Trim().ToLower();
                if (s == "y" || s == "yes" || s == "true") return true;
                if (s == "n" || s == "no" || s == "false") return false;
                Console.WriteLine("Enter y or n.");
            }
        }
    }
}

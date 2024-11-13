using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Spectre.Console;
using Figgle;

namespace BankAccountwithGenericAndJson
{
    // Interface for Bank Account Operations
    public interface IBankAccount<T>
    {
        void Deposit(T amount);
        void Withdraw(T amount);
        void CheckBalance();
    }

    // Bank Account Class
    public class BankAccount<T> : IBankAccount<T> where T : struct
    {
        private decimal Balance;
        public string AccountName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public BankAccount(string accountName, string username, string password, decimal balance)
        {
            AccountName = accountName;
            Username = username;
            Password = password;
            Balance = balance;
        }

        // Deposit Method with Validation and Error Handling
        public void Deposit(T amount)
        {
            try
            {
                decimal value = Convert.ToDecimal(amount);
                if (value <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Error: Deposit amount must be positive.[/]");
                    return;
                }
                Balance += value;
                AnsiConsole.MarkupLine($"[green]Deposited {value}. New balance: {Balance}[/]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Withdraw Method with Validation and Error Handling
        public void Withdraw(T amount)
        {
            try
            {
                decimal value = Convert.ToDecimal(amount);
                if (value <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Error: Withdrawal amount must be positive.[/]");
                    return;
                }
                if (value > Balance)
                {
                    AnsiConsole.MarkupLine("[red]Error: Insufficient funds.[/]");
                    return;
                }
                Balance -= value;
                AnsiConsole.MarkupLine($"[green]Withdrew {value}. New balance: {Balance}[/]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // CheckBalance Method
        public void CheckBalance()
        {
            AnsiConsole.MarkupLine($"[blue]Your current balance is: {Balance}[/]");
        }

        // Save Account Data to JSON
        public void SaveToJson()
        {
            var accountData = new { Username, Password, AccountName, Balance = Balance };
            string json = JsonSerializer.Serialize(accountData);
            File.WriteAllText($"{AccountName}.json", json);
            AnsiConsole.MarkupLine($"[green]Account data saved to {AccountName}.json[/]");
        }
    }

    // Authentication Class for Login and Logout
    public class Auth
    {
        public static BankAccount<decimal>? Login()
        {
            Console.Clear();
            AnsiConsole.Write(new FigletText("Bank System Login").Centered().Color(ConsoleColor.Cyan));

            Console.Write("Enter username: ");
            string username = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter password: ");
            string password = Console.ReadLine() ?? string.Empty;

            try
            {
                // Check if the JSON file exists
                if (!File.Exists("UserAccount.json"))
                {
                    AnsiConsole.MarkupLine("[red]Error: Account data file not found.[/]");
                    return null;
                }

                // Load account data from JSON
                string json = File.ReadAllText("UserAccount.json");
                var accountData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                // Validate JSON structure
                if (accountData == null ||
                    !accountData.ContainsKey("Username") ||
                    !accountData.ContainsKey("Password") ||
                    !accountData.ContainsKey("AccountName") ||
                    !accountData.ContainsKey("Balance"))
                {
                    AnsiConsole.MarkupLine("[red]Error: Invalid account data format.[/]");
                    return null;
                }

                // Extract values safely from JsonElement
                string storedUsername = accountData["Username"].GetString() ?? string.Empty;
                string storedPassword = accountData["Password"].GetString() ?? string.Empty;
                string accountName = accountData["AccountName"].GetString() ?? "DefaultAccount";
                decimal balance = accountData["Balance"].GetDecimal();

                // Check credentials
                if (storedUsername == username && storedPassword == password)
                {
                    AnsiConsole.MarkupLine("[green]Login successful![/]");
                    return new BankAccount<decimal>(accountName, username, password, balance);
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Invalid username or password.[/]");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public static void Logout()
        {
            AnsiConsole.MarkupLine("[yellow]Logging out...[/]");
            System.Threading.Thread.Sleep(1000);
            Console.Clear();
        }
    }

    // Main Application
    public class Program
    {
        static void Main(string[] args)
        {
            AnsiConsole.Write(new FigletText("Bank System").Centered().Color(ConsoleColor.Cyan));
            BankAccount<decimal>? account = null;

            while (account == null) // Prompt user to login
            {
                account = Auth.Login();
            }

            bool exit = false;
            while (!exit)
            {
                ShowMenu();
                string choice = Console.ReadLine()!;

                switch (choice)
                {
                    case "1":
                        AnsiConsole.MarkupLine("[yellow]Enter amount to deposit:[/]");
                        var depositAmount = Convert.ToDecimal(Console.ReadLine());
                        account.Deposit(depositAmount);
                        break;
                    case "2":
                        AnsiConsole.MarkupLine("[yellow]Enter amount to withdraw:[/]");
                        var withdrawAmount = Convert.ToDecimal(Console.ReadLine());
                        account.Withdraw(withdrawAmount);
                        break;
                    case "3":
                        account.CheckBalance();
                        break;
                    case "4":
                        account.SaveToJson();
                        break;
                    case "5":
                        Auth.Logout();
                        account = null;
                        while (account == null)
                        {
                            account = Auth.Login();
                        }
                        break;
                    case "6":
                        exit = true;
                        AnsiConsole.MarkupLine("[green]Exiting program.[/]");
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]Invalid choice. Please select an option from 1 to 6.[/]");
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            AnsiConsole.MarkupLine("[blue]Choose an option:[/]");
            AnsiConsole.MarkupLine("[yellow]1. Deposit[/]");
            AnsiConsole.MarkupLine("[yellow]2. Withdraw[/]");
            AnsiConsole.MarkupLine("[yellow]3. Check Balance[/]");
            AnsiConsole.MarkupLine("[yellow]4. Save Account to JSON[/]");
            AnsiConsole.MarkupLine("[yellow]5. Logout[/]");
            AnsiConsole.MarkupLine("[yellow]6. Exit[/]");
        }
    }
}



//BankAccountwithGenericAndJson
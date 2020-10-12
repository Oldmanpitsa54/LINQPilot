﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace OOPExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var banks = GenerateBanks();
            var users = GenerateUsers(banks);

            var MostPoorUsers = users.OrderBy(x => x.Transactions.Sum(y => y.Value)).Take(4).Select(x => new
            {
                TransactionSum = x.Transactions.Sum(y => y.Value),
                User = x
            });

            //1) Сделать выборку всех Пользователей, имя + фамилия которых длиннее чем 12 символов.

            var NameMoreThan12SymbUsers = users.Where(x => (x.FirstName + x.LastName).Length > 12);

            //2) Сделать выборку всех транзакций (в результате должен получится список из 1000 транзакций)

            var allTransactions = banks.SelectMany(x => x.Transactions);

            //3) Вывести Банк: и всех его пользователей (Имя + фамилия + количество транзакий в гривне) отсортированных по Фамилии по убиванию. в таком виде :
            //   Имя банка 
            //   ***************
            //   Игорь Сердюк 
            //   Николай Басков

            var groupedByBankUsers = users.GroupBy(x => x.Bank);
            foreach (var group in groupedByBankUsers)
            {
                Console.WriteLine(group.Key.Name);
                Console.WriteLine("**************");

                foreach (var user in group.OrderByDescending(x => x.LastName))
                {
                    Console.WriteLine($"{user.FirstName} {user.LastName} {user.Transactions.Count(y => y.Currency == Currency.UAH)}, bank: {user.Bank.Name}");
                }
            }

            //4) Сделать выборку всех Пользователей типа Admin, у которых счет в банке, в котором больше всего транзакций
            var admins = users.Where(x => x.Type == UserType.Admin);
            var mostTransactionsBank = banks
                .OrderBy(x => x.Transactions.Count)
                .ThenBy(x => x.Name)
                .FirstOrDefault();

            //5) Найти Пользователей(НЕ АДМИНОВ), которые произвели больше всего транзакций в определенной из валют (UAH,USD,EUR)
            //то есть найти трёх пользователей: 1й который произвел больше всего транзакций в гривне, второй пользователь, который произвел больше всего транзакций в USD 
            //и третьего в EUR

            var MostTransUAH = users.OrderByDescending(x => x.Transactions.Count(y => y.Currency == Currency.UAH)).First();
            var MostTransUSD = users.OrderByDescending(x => x.Transactions.Count(y => y.Currency == Currency.USD)).First();
            var MostTransEUR = users.OrderByDescending(x => x.Transactions.Count(y => y.Currency == Currency.EUR)).First();

            var MostTrList = new List<User>();
            MostTrList.Add(MostTransUAH);
            MostTrList.Add(MostTransUSD);
            MostTrList.Add(MostTransEUR);

            foreach (User user in MostTrList)
            {
                Console.WriteLine("Most transactions has " + user.LastName);
            }


            //6) Сделать выборку транзакций банка, у которого больше всего Pemium пользователей
            var PremiumUsers = users.Where(x => x.Type == UserType.Pemium);
            var groupedByBankPremiumUsers = PremiumUsers
                .GroupBy(x => x.Bank)
                .Select(x => new { Bank = x.Key, Count = x.Count() });


            foreach (var group in groupedByBankPremiumUsers)
            {
                Console.WriteLine($"Bank name: {group.Bank.Name}, count: {group.Count}");
            }
            var MostrPrBank = groupedByBankPremiumUsers
            .OrderByDescending(x => x.Count).First();

            foreach (Transaction trans in MostrPrBank.Bank.Transactions)
            {
                Console.WriteLine(trans.Value);
            }

        }

        public class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<Transaction> Transactions { get; set; }
            public UserType Type { get; set; }
            public Bank Bank { get; set; }
        }

        public class Transaction
        {
            public decimal Value { get; set; }

            public Currency Currency { get; set; }
        }

        public static List<Transaction> GetTenTransactions()
        {
            var result = new List<Transaction>();
            var sign = random.Next(0, 1);
            var signValue = sign == 0 ? -1 : 1;
            for (var i = 0; i < 10; i++)
            {
                result.Add(new Transaction
                {
                    Value = (decimal)random.NextDouble() * signValue * 100m,
                    Currency = GetRandomCurrency(),
                });
            }

            return result;
        }

        public class Bank
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public List<Transaction> Transactions { get; set; }
        }

        public enum UserType
        {
            Default = 1,
            Pemium = 2,
            Admin = 3
        }

        public static UserType GetRandomUserType()
        {
            int userTypeInt = random.Next(1, 4);

            return (UserType)userTypeInt;
        }

        public enum Currency
        {
            USD = 1,
            UAH = 2,
            EUR = 3
        }

        public static Currency GetRandomCurrency()
        {
            int userTypeInt = random.Next(1, 4);

            return (Currency)userTypeInt;
        }

        public static List<Bank> GenerateBanks()
        {
            var banksCount = random.Next(BANKS_MIN, BANKS_MAX);
            var result = new List<Bank>();

            for (int i = 0; i < banksCount; i++)
            {
                result.Add(new Bank
                {
                    Id = i + 1,
                    Name = RandomString(random.Next(NAME_MIN_LENGTH, NAME_MAX_LENGTH)),
                    Transactions = new List<Transaction>()
                });
            }

            return result;
        }

        public static List<User> GenerateUsers(List<Bank> banks)
        {
            var result = new List<User>();
            int bankId = 0;
            Bank bank = null;
            List<Transaction> transactions = null;
            for (int i = 0; i < 100; i++)
            {
                bankId = random.Next(0, banks.Count);
                bank = banks[bankId];
                transactions = GetTenTransactions();
                result.Add(new User
                {
                    Bank = bank,
                    FirstName = RandomString(random.Next(NAME_MIN_LENGTH, NAME_MAX_LENGTH)),
                    Id = i + 1,
                    LastName = RandomString(random.Next(NAME_MIN_LENGTH, NAME_MAX_LENGTH)),
                    Type = GetRandomUserType(),
                    Transactions = transactions
                });
                bank.Transactions.AddRange(transactions);
            }

            return result;
        }

        private const int BANKS_MIN = 2;
        private const int BANKS_MAX = 5;

        private const int NAME_MAX_LENGTH = 10;
        private const int NAME_MIN_LENGTH = 4;

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
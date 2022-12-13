using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SibersDatabase.Models
{
    public class Employee
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
    }
}

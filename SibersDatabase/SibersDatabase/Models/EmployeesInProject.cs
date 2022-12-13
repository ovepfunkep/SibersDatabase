using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SibersDatabase.Models
{
    public class EmployeesInProject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
    }
}

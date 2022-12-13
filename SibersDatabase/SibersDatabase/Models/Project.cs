using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SibersDatabase.Models
{
    public class Project
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string Name { get; set; }
        public string CustomerCompanyName { get; set; }
        public string ConctractorCompanyName { get; set; }
        public int HeadId { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateEnded { get; set; }
        public int Priority { get; set; }
    }
}

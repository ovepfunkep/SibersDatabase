using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SibersDatabase.Models;

namespace SibersDatabase.ViewModels.General
{
    public class Getters
    {
        public static async Task<List<EmployeesInProject>> GetEmployeesInProjectAsync(int ProjectId)
        {
            List<EmployeesInProject> result = await App.Db.EmployeesInProjectsTableMethods.GetAsync<EmployeesInProject>(
                    EIP => EIP.ProjectId == ProjectId);
            if (result == null) result = new List<EmployeesInProject>();
            return result;
        }
    }
}

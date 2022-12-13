using SibersDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Android;
using Xamarin.Essentials;
using static SibersDatabase.App;
using SibersDatabase.Data;
using System.Threading.Tasks;
using static SibersDatabase.Data.MainDB;

namespace SibersDatabase.ViewModels.EmployeeViewModels
{
    public class EmployeeSelectingViewModel
    {
        static Repository<EmployeesInProject> EIPMethods = Db.EmployeesInProjectsTableMethods;
        static Repository<Employee> EmpMethods = Db.EmployeesTableMethods;
        static Repository<Project> ProjMethods = Db.ProjectsTableMethods;

        public class CollectionItem
        {
            public Employee Employee { get; set; }
            public bool IsChecked { get; set; }

            public CollectionItem(Employee employee, bool isChecked)
            {
                Employee = employee;
                IsChecked = isChecked;
            }

            public static List<CollectionItem> GetAll(List<Employee> employeesToShow, List<EmployeesInProject> employeesToSelect = null)
            {
                List<CollectionItem> result = new List<CollectionItem>();
                List<int> employeesToSelectIds = employeesToSelect == null? new List<int>() : 
                                                  employeesToSelect.ConvertAll(EIP => EIP.EmployeeId);
                foreach (Employee employee in employeesToShow)
                    result.Add(new CollectionItem(employee, employeesToSelectIds.Count(id => id == employee.Id) == 1));
                return result;
            }
        }

        public static async Task SaveChangesToDBAsync(List<CollectionItem> itemsCollection, int projectId)
        {

            try
            {
                List<EmployeesInProject> employeesInProjectList = await EIPMethods.GetAsync<EmployeesInProject>(predicate: EIP => EIP.ProjectId == projectId);
                List<int> selectedEmployeesIds = itemsCollection.Where(item => item.IsChecked == true).ToList().ConvertAll(item => item.Employee.Id);
                List<int> noNeedEmployees = employeesInProjectList.ConvertAll<int>(EIP => EIP.EmployeeId).Except(selectedEmployeesIds).ToList();

                foreach (var noNeedEmployeeId in noNeedEmployees)
                    await EIPMethods.DeleteAsync<EmployeesInProject>(employeesInProjectList.Where(EIP => EIP.EmployeeId == noNeedEmployeeId).First().Id);

                employeesInProjectList = await EIPMethods.GetAsync<EmployeesInProject>(predicate: EIP => EIP.ProjectId == projectId);
                List<Employee> employeesList = new List<Employee>();
                foreach (EmployeesInProject EIP in employeesInProjectList)
                    employeesList.Add(await EmpMethods.GetAsync(EIP.EmployeeId));
                List<int> missingEmployeesInProjectIds = selectedEmployeesIds.Except(employeesList.ConvertAll<int>(employee => employee.Id)).ToList();

                foreach (var missingEmployeeId in missingEmployeesInProjectIds)
                    await EIPMethods.InsertAsync(new EmployeesInProject()
                    {
                        EmployeeId = missingEmployeeId,
                        ProjectId = projectId
                    });
            }
            catch { }
        }

        public static async Task ChangeHeadIdToAsync(int projectId, int newId)
        {
            Project project = await ProjMethods.GetAsync(projectId);
            project.HeadId = newId;
            await ProjMethods.UpdateAsync(project);
        }

        public static async Task UpdateEmployeesInProjectAsync(List<EmployeesInProject> employeesInProjects, int projectId)
        {
            List<CollectionItem> itemsList = new List<CollectionItem>();
            List<Employee> employees = await EmpMethods.GetAsync<Employee>(emp => employeesInProjects.Any(EIP => EIP.EmployeeId == emp.Id));
            foreach (Employee employee in employees)
                itemsList.Add(new CollectionItem(employee, true));
            await SaveChangesToDBAsync(itemsList, projectId);
        }

        public static async Task DeleteProjectAsync(int projectId)
        {
            List<EmployeesInProject> employeesInProject = await EIPMethods.GetAsync<EmployeesInProject>(EIP => EIP.ProjectId == projectId);
            foreach (EmployeesInProject employeeInProject in employeesInProject)
                await EIPMethods.DeleteAsync<EmployeesInProject>(employeeInProject.Id);
            await ProjMethods.DeleteAsync<Project>(projectId);
        }
    }
}

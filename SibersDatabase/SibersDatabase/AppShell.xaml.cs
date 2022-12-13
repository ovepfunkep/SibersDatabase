using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SibersDatabase.Views.EmployeePages;
using SibersDatabase.Views.ProjectPages;

namespace SibersDatabase
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EmployeesViewPage), typeof(EmployeesViewPage));
            Routing.RegisterRoute(nameof(ProjectsViewPage), typeof(ProjectsViewPage));
            Routing.RegisterRoute(nameof(EmployeeEditPage), typeof(EmployeeEditPage));
            Routing.RegisterRoute(nameof(ProjectEditPage), typeof(ProjectEditPage));
            Routing.RegisterRoute(nameof(EmployeesSelectingPage), typeof(EmployeesSelectingPage));
        }
    }
}
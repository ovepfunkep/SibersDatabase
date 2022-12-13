using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SibersDatabase.Models;
using SibersDatabase.Data;
using Android.Widget;

namespace SibersDatabase.Views.EmployeePages
{
    [QueryProperty(nameof(EmployeeId),nameof(EmployeeId))]
    public partial class EmployeeEditPage : ContentPage
    {
        public string EmployeeId
        {
            set
            {
                LoadEmployee(value);
            }
        }

        public EmployeeEditPage()
        {
            BindingContext = new Models.Employee();
            InitializeComponent();
        }

        private async void LoadEmployee(string id)
        {
            try
            {
                Models.Employee employee = await App.Db.EmployeesTableMethods.GetAsync(Convert.ToInt32(id));
                BindingContext = employee;
            } catch { }
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            Models.Employee employee = (Models.Employee)BindingContext;
            if (!string.IsNullOrEmpty(employee.Name.Trim(' ')) &&
                !string.IsNullOrEmpty(employee.Surname.Trim(' ')) &&
                !string.IsNullOrEmpty(employee.MiddleName.Trim(' ')) &&
                !string.IsNullOrEmpty(employee.Email.Trim(' ')))
            {
                if (employee.Id == 0) await App.Db.EmployeesTableMethods.InsertAsync(employee);
                else await App.Db.EmployeesTableMethods.UpdateAsync(employee);
                await Shell.Current.GoToAsync("..");
            }
            else await this.DisplayAlert("Missing arguments", "Please fill all the fields", "Okay..");
        }
    }
}
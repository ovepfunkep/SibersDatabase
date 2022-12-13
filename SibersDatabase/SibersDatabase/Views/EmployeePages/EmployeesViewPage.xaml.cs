using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SibersDatabase.Views.ProjectPages;
using SibersDatabase.Models;
using System.Collections.Generic;

namespace SibersDatabase.Views.EmployeePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmployeesViewPage : ContentPage
    {
        List<Employee> employeesStorage;
        public List<Employee> employeesShown { get; set; }

        public EmployeesViewPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            employeesStorage = await App.Db.EmployeesTableMethods.GetAsync();
            employeesShown = employeesStorage;
            collectionView.ItemsSource = employeesShown;
        }

        private void Filter_Clicked(object sender, EventArgs e)
        {
            FlyoutItem flyout = new FlyoutItem
            {
                Title = "Filter",
                IsEnabled = true
            };
            flyout.IsVisible = true;
        }

        private async void collectionView_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                Employee employee = (Employee)e.CurrentSelection.FirstOrDefault();
                await Shell.Current.GoToAsync($"{nameof(EmployeeEditPage)}?{nameof(EmployeeEditPage.EmployeeId)}={employee.Id}");
            }
        }

        private async void AddButton_ClickedAsync(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync(nameof(EmployeeEditPage));

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue)) employeesShown = employeesStorage;
            else
            {
                string oldText = e.OldTextValue ?? "";
                List<Employee> newEmployees = e.NewTextValue.Length > oldText.Length ?
                                                 employeesShown : employeesStorage;
                employeesShown = newEmployees.Where(item => $" {item.Name} {item.Surname} {item.MiddleName} ".ToLower()
                                                                                                             .Contains(e.NewTextValue.ToLower())).ToList();
            }
            collectionView.ItemsSource = employeesShown;
        }

    }
}
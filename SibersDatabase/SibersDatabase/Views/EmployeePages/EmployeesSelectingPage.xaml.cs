using SibersDatabase.Models;
using SibersDatabase.Views.ProjectPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static SibersDatabase.App;
using static SibersDatabase.ViewModels.EmployeeViewModels.EmployeeSelectingViewModel;
using static SibersDatabase.ViewModels.General.Getters;

namespace SibersDatabase.Views.EmployeePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ProjectId), nameof(ProjectId))]
    [QueryProperty(nameof(Choosing), nameof(Choosing))]
    public partial class EmployeesSelectingPage : ContentPage
    {
        private int projectId;
        public string ProjectId
        {
            set
            {
                projectId = Convert.ToInt32(value);
            }
        }

        private string choosing;
        public string Choosing 
        {
            set 
            {
                choosing = value;
                SelectEmployeesAsync();
            }
            get
            {
                return choosing;
            }
        }

        private List<CollectionItem> itemsCollectionStorage;
        public List<CollectionItem> ItemsCollectionStorage
        {
            get { return itemsCollectionStorage; }
            set 
            { 
                itemsCollectionStorage = value;
                ItemsCollectionShown = value;
            }
        }
        public List<CollectionItem> ItemsCollectionShown { get; set; }

        public EmployeesSelectingPage()
        {
            InitializeComponent();
        }

        int headId;
        private async void SelectEmployeesAsync()
        {
            List<Employee> employees = await Db.EmployeesTableMethods.GetAsync();
            List<EmployeesInProject> employeesInProject = await GetEmployeesInProjectAsync(projectId);

            headId = (await Db.ProjectsTableMethods.GetAsync(projectId)).HeadId;
            if (Choosing == "Head")
            {
                employees = employees.Where(emp => employeesInProject.Any(EIP => EIP.EmployeeId == emp.Id)).ToList();
                employeesInProject = headId == 0 ? new List<EmployeesInProject>() : employeesInProject.FindAll(EIP => EIP.EmployeeId == headId);
            }
            ItemsCollectionStorage = new List<CollectionItem>(CollectionItem.GetAll(employees, employeesInProject));

            BindingContext = this;
        }

        private async void ButtonSave_ClickedAsync(object sender, EventArgs e)
        {
            if (Choosing == "Employees")
            {
                if (headId != 0 && ItemsCollectionStorage.Find(item => item.Employee.Id == headId).IsChecked == false) await ChangeHeadIdToAsync(projectId, 0);
                await SaveChangesToDBAsync(ItemsCollectionStorage, projectId);
            }
            else
            {
                CollectionItem chosenItem = ItemsCollectionStorage.Find(item => item.IsChecked);
                int selectedHeadId = chosenItem == null ? 0 : chosenItem.Employee.Id;
                if (selectedHeadId != 0)
                {   if (selectedHeadId != headId) await ChangeHeadIdToAsync(projectId, selectedHeadId);}
                else if (headId != 0) await ChangeHeadIdToAsync(projectId, 0);
            }
            await Shell.Current.GoToAsync($"..?{nameof(ProjectEditPage.ProjectId)}={projectId}");
        }

        int boxesTicked = 0;
        private void checkBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (Choosing == "Head")
            {
                if (e.Value)
                {
                    if (boxesTicked == 1) checkBox.IsChecked = false;
                    boxesTicked++;
                }
                else boxesTicked--;
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue)) ItemsCollectionShown = ItemsCollectionStorage;
            else
            {
                string oldText = e.OldTextValue ?? "";
                List<CollectionItem> collectionToTake = e.NewTextValue.Length > oldText.Length ?
                    ItemsCollectionShown : ItemsCollectionStorage;
                ItemsCollectionShown = collectionToTake.Where(item =>
                    ($"{item.Employee.Surname} {item.Employee.Name} " +
                    $"{item.Employee.MiddleName} {item.Employee.Email}").ToLower().Contains(e.NewTextValue.ToLower())).ToList();
            }
            employees.ItemsSource = ItemsCollectionShown;
        }
    }
}

using SibersDatabase.Models;
using SibersDatabase.Views.EmployeePages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static SibersDatabase.ViewModels.EmployeeViewModels.EmployeeSelectingViewModel;
using static SibersDatabase.ViewModels.General.Getters;

namespace SibersDatabase.Views.ProjectPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(HeadId), nameof(HeadId))]
    [QueryProperty(nameof(ProjectId), nameof(ProjectId))]
    public partial class ProjectEditPage : ContentPage
    {
        public Command GoBackCommand { get; set; }

        private int projectId { get; set; } = -1;
        public string ProjectId
        {
            set
            {
                int newId = Convert.ToInt32(value);
                if (projectId != newId)
                {
                    projectId = newId;
                    LoadProjectAsync();
                }
            }
        }

        private int headId { get; set; } = -1;
        public string HeadId
        {
            set
            {
                int intHeadId = Convert.ToInt32(value);
                if (headId != intHeadId)
                {
                    headId = intHeadId;
                    SelectHeadAsync();
                }
            }
        }

        Project projectSaved { get; set; }
        List<EmployeesInProject> employeesInProjectSaved;

        public ProjectEditPage()
        {
            InitializeComponent();
            GoBackCommand = new Command(async () => await ClearUnsavedData());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshEmployeesAsync();
        }

        //Выбрать и отобразить руководителя проекта
        private async void SelectHeadAsync()
        {
            ((Project)pageContent.BindingContext).HeadId = headId;

            if (headId != 0) headEditor.BindingContext = await App.Db.EmployeesTableMethods.GetAsync(headId);
            else headEditor.BindingContext = new { Surname = "Choose employee" };
        }

        private async void LoadProjectAsync()
        {
            if (projectId == 0)
            {
                projectSaved = new Project
                {
                    Name = await GenerateProjectName(),
                    ConctractorCompanyName = "Null",
                    CustomerCompanyName = "Null",
                    DateStarted = DateTime.Now,
                    DateEnded = DateTime.Now
                };
                employeesInProjectSaved = new List<EmployeesInProject>();
                pageContent.BindingContext = projectSaved;
                HeadId = "0";
                await App.Db.ProjectsTableMethods.InsertAsync(projectSaved);
                projectId = (await App.Db.ProjectsTableMethods.GetAsync<Project>(Proj => Proj.Name == "Null")).First().Id;
            }
            else
            {
                projectSaved = await App.Db.ProjectsTableMethods.GetAsync(projectId);
                employeesInProjectSaved = await GetEmployeesInProjectAsync(projectId);
            }
            pageContent.BindingContext = projectSaved;
        }

        private async Task<string> GenerateProjectName()
        {
            List<string> existingNames = (await App.Db.ProjectsTableMethods.GetAsync()).ConvertAll(project => project.Name);
            int count = existingNames.Count;
            string name = $"Project {count}";
            while (existingNames.Contains(name)) name = $"Project {++count}";
            return name;
        }

        //Обновить список сотрудников данного проекта
        private async Task RefreshEmployeesAsync()
        {
            List<EmployeesInProject> employeesInProject = await GetEmployeesInProjectAsync(projectId);

            int EmployeesCount = employeesInProject.Count;
            employeesEditor.BindingContext = new
            {
                Text = EmployeesCount == 0 ? "Choose" :
                EmployeesCount.ToString()
            };

            if (projectId != 0)
                HeadId = (await App.Db.ProjectsTableMethods.GetAsync(projectId)).HeadId.ToString();
            else HeadId = "0";
        }

        private async void Save_ClickedAsync(object sender, EventArgs e)
        {
            Project newProject = (Project)pageContent.BindingContext;
            List<Project> existingProjects = await App.Db.ProjectsTableMethods.GetAsync();
            if ((existingProjects).Any(project => project.Name == newProject.Name && project.Id != newProject.Id))
                await DisplayAlert("Name is already taken", $"Name \"{newProject.Name}\" is already taken.", "Okay, I'll swap");
            else
            {
                if (AreFieldsDataCorrect(newProject))
                {
                    employeesInProjectSaved = await GetEmployeesInProjectAsync(projectId);
                    if (newProject.Id == 0) await App.Db.ProjectsTableMethods.InsertAsync(newProject);
                    else await App.Db.ProjectsTableMethods.UpdateAsync(newProject);
                    await Shell.Current.GoToAsync("..");
                }
                else await this.DisplayAlert("Missing arguments", "Please fill all the fields", "Okay..");
            }
        }

        private async void Priority_TextChangedAsync(object sender, FocusEventArgs e)
        {
            Editor Esender = (Editor)sender;
            if (Esender.Text.Any(ch => !char.IsDigit(ch))) await this.DisplayAlert("Priority wrong value", "Sorry, but priority can not store non-digits", "Fine.");
            if (!string.IsNullOrEmpty(Esender.Text)) Esender.Text = string.Join("", Esender.Text.ToList<char>().FindAll(ch => char.IsDigit(ch)));
        }

        private async void headEditor_FocusedAsync(object sender, FocusEventArgs e)
        {
            headEditor.Unfocus();
            if ((await GetEmployeesInProjectAsync(projectId)).Count > 0)
                await Shell.Current.GoToAsync($"{nameof(EmployeesSelectingPage)}?{nameof(EmployeesSelectingPage.Choosing)}=Head&" +
                                                                               $"{nameof(EmployeesSelectingPage.ProjectId)}={projectId}");
        }

        private async void employeesEditor_FocusedAsync(object sender, FocusEventArgs e)
        {
            employeesEditor.Unfocus();
            await Shell.Current.GoToAsync($"{nameof(EmployeesSelectingPage)}?{nameof(EmployeesSelectingPage.Choosing)}=Employees&" +
                                                                     $"{nameof(EmployeesSelectingPage.ProjectId)}={projectId}");
        }

        private async Task ClearUnsavedData()
        {
            if (projectSaved.Name == "Null") await DeleteProjectAsync(projectId);
            else await UpdateEmployeesInProjectAsync(employeesInProjectSaved, projectId);
        }

        private bool AreFieldsDataCorrect(Project project)
        {
            bool IsFieldCorrect(string field) => !string.IsNullOrEmpty(field?.Trim(' ')) && field != "Null";

            return IsFieldCorrect(project.Name) && IsFieldCorrect(project.CustomerCompanyName) &&
                IsFieldCorrect(project.ConctractorCompanyName) && !(project.HeadId == 0);
        }

        private async void ButtonDelete_Clicked(object sender, EventArgs e)
        {
            await DeleteProjectAsync(projectId);
            await Shell.Current.GoToAsync("..");
        }
    }
}
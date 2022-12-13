using SibersDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SibersDatabase.Views.ProjectPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectsViewPage : ContentPage
    {
        List<Project> ProjectsStorage;
        List<Project> ProjectsShown { get; set; }

        public ProjectsViewPage()
        {
            InitializeComponent();
        }

        //При появлении отобразить список проектов
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            ProjectsStorage = await App.Db.ProjectsTableMethods.GetAsync();
            ProjectsShown = ProjectsStorage;
            collectionView.ItemsSource = ProjectsShown;
        }

        private void Filter_Clicked(object sender, EventArgs e)
        {
            FlyoutItem flyout = new FlyoutItem();
            flyout.Title = "Filter";
            flyout.IsEnabled = true;
        }

        //Открыть страницу редактирования проекта по ID проекта
        private async void collectionView_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                Project project = (Project)e.CurrentSelection.FirstOrDefault();
                await Shell.Current.GoToAsync($"{nameof(ProjectEditPage)}?{nameof(ProjectEditPage.ProjectId)}={project.Id}");
            }
        }

        private async void AddButton_ClickedAsync(object sender, EventArgs e) =>
                await Shell.Current.GoToAsync($"{nameof(ProjectEditPage)}?{nameof(ProjectEditPage.ProjectId)}={0}");

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                ProjectsShown = ProjectsStorage;
                priorityFilterChanged(null,null);
            }
            else
            {
                string oldText = e.OldTextValue ?? "";
                List<Project> newProjects = e.NewTextValue.Length > oldText.Length ?
                                                 ProjectsShown : ProjectsStorage;
                ProjectsShown = newProjects.Where(item => item.Name.ToLower().Contains(e.NewTextValue.ToLower())).ToList();
            }
            collectionView.ItemsSource = ProjectsShown;
        }

        bool Expanded = false;
        private void ButtonFilter_Clicked(object sender, EventArgs e)
        {
            ((Grid)((StackLayout)((Button)sender).Parent).Parent).RowDefinitions[0].Height = Expanded ? 50 : 180;
            Expanded = !Expanded;

        }

        private void priorityFilterChanged(object sender, TextChangedEventArgs e)
        {
            int priorityFrom = string.IsNullOrEmpty(entryFrom.Text) ? 0 : Convert.ToInt32(entryFrom.Text);
            int priorityTo = string.IsNullOrEmpty(entryTo.Text) ? int.MaxValue : Convert.ToInt32(entryTo.Text);
            ProjectsShown = ProjectsStorage; 
            ProjectsShown = ProjectsShown.Where(project => (priorityTo >= project.Priority) && (project.Priority >= priorityFrom)).ToList();
            collectionView.ItemsSource = ProjectsShown;
        }
    }
}
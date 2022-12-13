using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SibersDatabase.Views;
using SibersDatabase.Data;
using System.IO;

[assembly: ExportFont("Comfortaa-Bold.ttf", Alias = "Comfortaa")]
namespace SibersDatabase
{
    public partial class App : Application
    {
        static MainDB db { get; set; }
        public static MainDB Db
        {
            get
            {
                if (db == null)
                    db = new MainDB(Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "MainDataBase.db3"));
                return db;
            }
        }
        
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using AppLauncher.Models;
using AppLauncher.Commands;
using AppLauncher.Data;
using AppLauncher.Services;
using System.IO;
using System.Diagnostics;


namespace AppLauncher.ViewModels
{
    [Serializable]
    public class MainViewModel : BaseViewModel
    {
        DataManager dm = new DataManager();
        DataContainer dc;

        public OpenFileDialogService openFileDilog = new OpenFileDialogService();
        public ObservableCollection<CategoryViewModel> Categories_VM { get; set; }
        public ObservableCollection<ApplicationViewModel> Applications_VM { get; set; }

        TaskCompletionSource<bool> closeEventHandled = new TaskCompletionSource<bool>();

        public MainViewModel()
        {
            dc = dm.Load() as DataContainer;
            if (dc == null)
            {
                Categories_VM = new ObservableCollection<CategoryViewModel>();
                Categories_VM.Add(new CategoryViewModel(new Category()
                {
                    Name = "Office",
                    Applications = new HashSet<Application>()
                }));
                Applications_VM = new ObservableCollection<ApplicationViewModel>();
                dc = new DataContainer();
                dc.StoredCategorys_VM = Categories_VM;
                dm.Save(dc);
            }
            else
            {
                Categories_VM = dc.StoredCategorys_VM;
                Applications_VM = new ObservableCollection<ApplicationViewModel>();//dc.StoredApplications_VM;
                SelectedCategory = Categories_VM[0];
                LoadApplications();
            }
        }

        #region Loaders

        private void LoadApplications()
        {
            Applications_VM.Clear();
            foreach (var app in SelectedCategory.Applications)
            {
                Applications_VM.Add(new ApplicationViewModel(app));
            }
        }
        #endregion
        
        #region Categories

        private CategoryViewModel selectedCategory;
        public CategoryViewModel SelectedCategory
        {
            get { return selectedCategory; }
            set { selectedCategory = value; LoadApplications(); OnPropertyChanged(nameof(SelectedCategory)); }
        }

        private RelayCommand addCategory;
        public RelayCommand AddCategory => addCategory ?? (addCategory = new RelayCommand(obj =>
        {
            Category category = new Category()
            {
                Name = "New Category",
                Applications = new HashSet<Application>()
            };
            Categories_VM.Add(new CategoryViewModel(category));
            dm.Save(dc);
            SelectedCategory = Categories_VM[Categories_VM.Count - 1];
        }));

        private RelayCommand saveCategory;
        public RelayCommand SaveCategory => saveCategory ?? (saveCategory = new RelayCommand(obj =>
        {
            if (SelectedCategory != null)
            {
                dm.Save(dc);
                ShowMessage("Category saved.", 64);
            }
            else
                ShowMessage("Select Category to save!", 48);

        }));

        private RelayCommand delCategory;
        public RelayCommand DelCategory => delCategory ?? (delCategory = new RelayCommand(obj =>
        {
            if (SelectedCategory != null)
            {
                Applications_VM.Clear();
                Categories_VM.Remove(SelectedCategory);
                dm.Save(dc);
                if (Categories_VM.Count > 0)
                    SelectedCategory = Categories_VM[0];
                ShowMessage("Category successfully deleted.", 64);
            }
            else
                ShowMessage("Select category to delete!", 48);

        }));

        #endregion

        #region Applications

        private ApplicationViewModel selectedApplication;
        public ApplicationViewModel SelectedApplication
        {
            get { return selectedApplication; }
            set { selectedApplication = value; OnPropertyChanged(nameof(SelectedApplication)); }
        }

        private RelayCommand addApplication;
        public RelayCommand AddApplication => addApplication ?? (addApplication = new RelayCommand(obj =>
        {
            if (SelectedCategory != null)
            {
                string path = openFileDilog.OpenFileDialog();
                if (path != null)
                {
                    Application application = new Application()
                    {
                        Title = "New Application",
                        Name = Path.GetFileName(path),
                        Path = path,
                        ProcessId = default,
                        Category = SelectedCategory.Category
                    };

                    if (SelectedCategory.Applications.Count == 0 ||
                    SelectedCategory.Applications.FirstOrDefault(c => c.Path == path) == null)
                    {
                        _ = SelectedCategory.Applications.Add(application);
                        
                        UpdateAndSave();

                        ShowMessage($"Application {application.Name} successfully added" +
                            $"\nto category {SelectedCategory.Name}", 64);
                    }
                    else
                        ShowMessage($"Category {SelectedCategory.Name} already contains" +
                            $"\napplication {application.Name}!", 48);
                }
                else
                    ShowMessage($"No applications were added to category" +
                        $"\n {SelectedCategory.Name}", 64);
            }
            else
                ShowMessage("Select Category to add application!", 48);
            
        }));

        private void UpdateAndSave()
        {
            dm.Save(dc);
            LoadApplications();
        }

        private RelayCommand delApplication;
        public RelayCommand DelApplication => delApplication ?? (delApplication = new RelayCommand(obj =>
        {
            if (SelectedCategory != null && SelectedApplication != null)
            {
                _ = SelectedCategory.Applications.Remove(SelectedApplication.Application);
                UpdateAndSave();
                ShowMessage($"Application was successfully removed" +
                    $"\nfrom category {SelectedCategory.Name}", 64);
            }

        }));

        private RelayCommand saveApplication;
        public RelayCommand SaveApplication => saveApplication ?? (saveApplication = new RelayCommand(obj =>
        {
            dm.Save(dc);
        }));

        #endregion

        #region Start-Stop

        private RelayCommand startApplication;

        public RelayCommand StartApplication => startApplication ?? (startApplication = new RelayCommand(obj =>
        {
            ControlApplication();
        }));

        public async void ControlApplication()
        {
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(SelectedApplication.Path);
            Process[] allProcesses = Process.GetProcesses();
            foreach (var p in allProcesses)
            {
                if (p.ProcessName == nameWithoutExtension)
                {
                    ShowMessage("Only one instance of application can be started at time!", 48);
                    return;
                }
            }
            Process appProcess = new Process();
            appProcess.StartInfo = new ProcessStartInfo(SelectedApplication.Path);
            appProcess.EnableRaisingEvents = true;
            appProcess.Exited += Application_Exitted;
            appProcess.Start();
            SelectedApplication.ProcessId = appProcess.Id;
            await Task.WhenAny(closeEventHandled.Task);
        }

        private void Application_Exitted(object sender, EventArgs e)
        {
            string applicationName = "";
            Process appProcess = sender as Process;
            if (SelectedApplication.ProcessId== appProcess.Id)
                SelectedApplication.ProcessId = 0;
            else
            {
                foreach (var application in Categories_VM
                    .SelectMany(category => category.Applications
                    .Where(application => application.ProcessId == appProcess.Id)))
                {
                    application.ProcessId = 0;
                    applicationName = application.Name;
                    break;
                }
            }            
            ShowMessage($"Application {applicationName} seccessfully closed!", 64);
            closeEventHandled = new TaskCompletionSource<bool>();
            closeEventHandled.SetResult(true);
        }

        private RelayCommand stopApplication;
        public RelayCommand StopApplication => stopApplication ?? (stopApplication = new RelayCommand(obj =>
        {
            Process appProcess = Process.GetProcessById(SelectedApplication.ProcessId);
            if (appProcess != null)
            {
                appProcess.CloseMainWindow();
                appProcess.Close();
            }
        }));

        #endregion

        #region Messaging
        private void ShowMessage(string message, int imageIndex)
        {
            System.Windows.MessageBox.Show(message, "Info",
                System.Windows.MessageBoxButton.OK,
                (System.Windows.MessageBoxImage)imageIndex);
        }
        #endregion



    }
}

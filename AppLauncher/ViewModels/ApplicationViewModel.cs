using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLauncher.Models;

namespace AppLauncher.ViewModels
{
    [Serializable]
    public class ApplicationViewModel : BaseViewModel
    {
        private Application _application;
        public ApplicationViewModel (Application application)
        {
            _application = application;
        }

        public Application Application
        {
            get { return _application; }
            set { _application = value; OnPropertyChanged(nameof(Application)); }
        }

        public string Title
        {
            get { return _application.Title; }
            set { _application.Title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string Name
        {
            get { return _application.Name; }
            set { _application.Name = value; OnPropertyChanged(nameof(Name)); }
        }

        public string Path
        {
            get { return _application.Path; }
            set { _application.Path = value; OnPropertyChanged(nameof(Path)); }
        }

        public int ProcessId
        {
            get { return _application.ProcessId; }
            set { _application.ProcessId = value; OnPropertyChanged(nameof(ProcessId)); }
        }

        public Category Category
        {
            get { return _application.Category; }
            set { _application.Category = value; OnPropertyChanged(nameof(Category)); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Globalization;
using AppLauncher.Models;
using AppLauncher.ViewModels;

namespace AppLauncher.DataBindings
{
    [Serializable]
    public class ApplicationModelToViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<ApplicationViewModel> Applications_VM = value as ObservableCollection<ApplicationViewModel>;
            ObservableCollection<Application> Applications = value as ObservableCollection<Application>;
            foreach (ApplicationViewModel application in Applications_VM)
            {
                Applications.Add(application.Application);
            }
            return Applications;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<Application> Applications = value as ObservableCollection<Application>;
            ObservableCollection<ApplicationViewModel> Applications_VM = value as ObservableCollection<ApplicationViewModel>;           
            foreach (Application application in Applications)
            {
                Applications_VM.Add(new ApplicationViewModel(application));
            }
            return Applications;
        }
    }
}

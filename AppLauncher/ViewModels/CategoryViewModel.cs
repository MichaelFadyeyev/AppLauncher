using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLauncher.Models;

namespace AppLauncher.ViewModels
{
    [Serializable]
    public class CategoryViewModel : BaseViewModel
    {
        private Category _category;

        public CategoryViewModel(Category category)
        {
            _category = category;
        }

        public Category Category
        {
            get { return _category; }
            set { _category = value; OnPropertyChanged(nameof(Category)); }
        }

        public string Name
        {
            get { return _category.Name; }
            set { _category.Name = value; OnPropertyChanged(nameof(Name)); }
        }

        public HashSet<Application> Applications
        {
            get { return _category.Applications; }
            set { _category.Applications = value; OnPropertyChanged(nameof(Applications)); }
        }
    }
}

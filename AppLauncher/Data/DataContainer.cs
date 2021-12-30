using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLauncher.ViewModels;
using System.Collections.ObjectModel;

namespace AppLauncher.Data
{
    [Serializable]
    public class DataContainer
    {
        public ObservableCollection<CategoryViewModel> StoredCategorys_VM { set; get; }
    }
}

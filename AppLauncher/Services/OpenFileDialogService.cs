using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;


namespace AppLauncher.Services
{
    [Serializable]
    public class OpenFileDialogService
    {
        public string OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Applications (*.exe)|*.exe";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog()==true)
            {
                return openFileDialog.FileName;
            }
            return string.Empty;
        }
    }
}

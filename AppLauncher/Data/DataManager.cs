using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using AppLauncher.ViewModels;


namespace AppLauncher.Data
{
    [Serializable]
    public class DataManager
    {
        private string _path = @"../../Data/dm.dat";
        private BinaryFormatter _formatter = new BinaryFormatter();

        public void Save(object obj)
        {
            using (FileStream fs = new FileStream(_path, FileMode.Create))
            {
                _formatter.Serialize(fs, obj);
            }
        }

        public object Load()
        {
            if (File.Exists(_path))
            {
                using (FileStream fs = new FileStream(_path, FileMode.Open))
                {
                    return _formatter.Deserialize(fs) as DataContainer;
                }
            }
            return null;
        }
    }
}

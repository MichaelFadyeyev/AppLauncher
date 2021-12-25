using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLauncher.Models
{
    [Serializable]
    public class Category
    {
        public string Name { get; set; }
        public virtual HashSet<Application> Applications { get; set; }
    }
}

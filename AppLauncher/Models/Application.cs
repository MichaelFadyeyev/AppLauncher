using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLauncher.Models
{
    [Serializable]
    public class Application
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int ProcessId { get; set; }
        public virtual Category Category { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uwp
{
    public class WindowEventArgs : EventArgs
    {
        public string windowName { get; set; }

        public WindowEventArgs(string _windowName)
        {
            windowName = _windowName;
        }
    }
}

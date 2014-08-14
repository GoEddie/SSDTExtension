using System;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace GoEddieUk.SqlServerTddHelper.UI
{
    class AlertBox
    {
        private readonly IVsUIShell _uiShell;

        public AlertBox(IVsUIShell uiShell)
        {
            _uiShell = uiShell;
        }

        public void ShowMessage(string message)
        {
            
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(_uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "Sql Server Tdd Helper",
                       message,
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));

        }
    }
}

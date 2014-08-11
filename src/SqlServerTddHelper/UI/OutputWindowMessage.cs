using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;


namespace GoEddieUk.SqlServerTddHelper.UI
{
    class OutputWindowMessage
    {
        public void WriteMessage(string message)
        {

            IVsOutputWindow outputWindow =
            Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

            Guid guidGeneral = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
            IVsOutputWindowPane pane;
            int hr = outputWindow.CreatePane(guidGeneral, "General", 1, 0);
            hr = outputWindow.GetPane(guidGeneral, out pane);
            pane.Activate();
            pane.OutputString(message);


        }

    }
}

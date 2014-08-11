using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Text;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using TSQLHelper;


namespace GoEddieUk.SqlServerTddHelper
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidSqlServerTddHelperPkgString)]
    public sealed class SqlServerTddHelperPackage : Package
    {
        public SqlServerTddHelperPackage()
        {
        }
                
        protected override void Initialize()
        {
            base.Initialize();

            AddCommandHandler();
        }

        private void AddCommandHandler(){
             
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                CommandID menuCommandID2 = new CommandID(GuidList.guidToolsOptionsCmdSet, (int)PkgCmdIDList.cmdidGEUKConfigureDeploy);
                MenuCommand menuItem2 = new MenuCommand(MenuItemCallback2, menuCommandID2);
                mcs.AddCommand(menuItem2);

                CommandID menuCommandID = new CommandID(GuidList.guidSqlServerTddHelperCmdSet, (int)PkgCmdIDList.cmdidGEUKDeployFile);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );

                CommandID menuCommandID3 = new CommandID(GuidList.guidGenScriptCmdSet, (int)PkgCmdIDList.cmdidGEUKGenScript);
                MenuCommand menuItem3 = new MenuCommand(MenuItemCallback3, menuCommandID3);
                mcs.AddCommand(menuItem3);

            }

        }

        private string GetSelectedSolutionExplorerFileName()
        {
            var dte = GetService(typeof(SDTE)) as EnvDTE.DTE;
            
            try
            {
                var filename = dte.SelectedItems.Item(1).ProjectItem.FileNames[0];
            
                return filename;
            }
            catch (Exception ex)
            {
                new UI.OutputWindowMessage().WriteMessage(string.Format("error: " + ex));
            }

            return null;
        }

        string get_last_part_of_name(string proc_name)
        {
            var parts = proc_name.Replace("\"", "").Replace("[", "").Replace("]", "").Split(new[] { '.' });
            return parts[parts.Length - 1];

        }

        private void MenuItemCallback3(object sender, EventArgs e)
        {
            try
            {
                var project = GetCurrentProject();
                var filename = GetSelectedSolutionExplorerFileName();

                if (String.IsNullOrEmpty(filename))
                {
                    new UI.OutputWindowMessage().WriteMessage("Couldn't get filename");
                    return;
                }

                if (!filename.EndsWith(".sql"))
                {
                    new UI.OutputWindowMessage().WriteMessage("This only works with .sql files - boo hoo hoo");
                    return;
                }

                var procname = new ScriptProperties().GetProcNameFromSqlFile(filename);
                if (string.IsNullOrEmpty(procname))
                {
                    new UI.OutputWindowMessage().WriteMessage("Couldn't get proc name - boo hoo hoo");
                    return;
                }

                WriteDeployeFile(procname, filename, new FileInfo(project.FullName).DirectoryName);

            }
            catch (Exception)
            {
                new UI.OutputWindowMessage().WriteMessage("Please select a project to configure");
            }

        }

        private void WriteDeployeFile(string procname, string proc_file, string output_dir)
        {

            var script = GetFileContents(proc_file);
            if (string.IsNullOrEmpty(script))
            {
                new UI.OutputWindowMessage().WriteMessage("Could not read script file: " + proc_file);
                return;
            }

            var output_script = new StringBuilder();
            output_script.AppendFormat(SqlStrings.DropExistingProc, get_last_part_of_name(procname), procname);
            output_script.AppendFormat(SqlStrings.Script, script);
            
            var output_script_path = Path.Combine(output_dir, string.Format("{0}.sql", get_last_part_of_name(procname)));

            try
            {
                WriteFileContents(output_script_path, output_script.ToString());
                new UI.OutputWindowMessage().WriteMessage("Written file: " + output_script_path);
            }
            catch (Exception e)
            {
                new UI.OutputWindowMessage().WriteMessage("Could not write file: " + output_script_path + " - error: " + e.Message);
                return;
            }
        }

        private void WriteFileContents(string file_name, string contents)
        {
            using (var writer = new StreamWriter(file_name))
            {
                writer.Write(contents);
            }
        }

        private string GetFileContents(string proc_file)
        {
            using (var reader = new StreamReader(proc_file))
            {
                return reader.ReadToEnd();
            }
        }


        private void MenuItemCallback2(object sender, EventArgs e)
        {
            try
            {
                var project = GetCurrentProject();
               

            }
            catch (Exception)
            {
                new UI.OutputWindowMessage().WriteMessage("Please select a project to configure");
            }
            
        }

        private Project GetCurrentProject()
        {
            IntPtr hierarchyPointer, selectionContainerPointer;
            Object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint projectItemId;

            IVsMonitorSelection monitorSelection =
                    (IVsMonitorSelection)Package.GetGlobalService(
                    typeof(SVsShellMonitorSelection));

            monitorSelection.GetCurrentSelection(out hierarchyPointer,
                                                 out projectItemId,
                                                 out multiItemSelect,
                                                 out selectionContainerPointer);

            IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(
                                                 hierarchyPointer,
                                                 typeof(IVsHierarchy)) as IVsHierarchy;

            if (selectedHierarchy != null)
            {
                ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(
                                                  projectItemId,
                                                  (int)__VSHPROPID.VSHPROPID_ExtObject,
                                                  out selectedObject));
            }

                Project selectedProject = ((selectedObject as dynamic).ProjectItems.ContainingProject) as EnvDTE.Project;

                return selectedProject;
            
         }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            //new UI.AlertBox(GetService(typeof(SVsUIShell)) as IVsUIShell).ShowMessage("Hello");
        }

    }
}

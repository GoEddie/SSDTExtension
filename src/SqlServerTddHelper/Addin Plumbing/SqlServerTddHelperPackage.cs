﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Transactions;
using EnvDTE;
using GoEddieUk.SqlServerTddHelper.Config;
using GoEddieUk.SqlServerTddHelper.DirtyVSApi;
using GoEddieUk.SqlServerTddHelper.Gateways;
using GoEddieUk.SqlServerTddHelper.UI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using TSQLHelper;

namespace GoEddieUk.SqlServerTddHelper
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidSqlServerTddHelperPkgString)]
    public sealed class SqlServerTddHelperPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            AddCommandHandler();
        }

        private void AddCommandHandler()
        {
            var mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs) 
                return;


            CreateMenuItem(mcs, GuidList.guidToolsOptionsCmdSet, (int) PkgCmdIDList.cmdidGEUKConfigureDeploy, ShowToolsOptions);
            CreateMenuItem(mcs, GuidList.guidSqlServerTddHelperCmdSet, (int) PkgCmdIDList.cmdidGEUKDeployFile, DeploySingleFileCallback);
            CreateMenuItem(mcs, GuidList.guidGenScriptCmdSet, (int) PkgCmdIDList.cmdidGEUKGenScript, GenerateScriptCallback);
        }

        private void CreateMenuItem(OleMenuCommandService mcs, Guid cmdsetGuid, int cmdId, EventHandler callback)
        {
            var commandId = new CommandID(cmdsetGuid, cmdId);
            var menuCommand = new MenuCommand(callback, commandId);
            mcs.AddCommand(menuCommand);
        }

        private string GetSelectedSolutionExplorerFileName()
        {
            var dte = GetService(typeof (SDTE)) as DTE;

            try
            {
                var filename = dte.SelectedItems.Item(1).ProjectItem.FileNames[0];

                return filename;
             
            }
            catch (Exception ex)
            {
                OutputWindowMessage.WriteMessage(string.Format("error: " + ex));
            }

            return null;
        }

        

        private void GenerateScriptCallback(object sender, EventArgs e)
        {
            try
            {
                var project = GetCurrentProject();
                var settings = Config.Configuration.GetSettings(project);
                if (null == settings)
                {
                    OutputWindowMessage.WriteMessage("Cancelled");
                    return;
                }

                ThreadPool.QueueUserWorkItem(GenerateScript, project);
            }
            catch (Exception ex)
            {
                OutputWindowMessage.WriteMessage("Unable to generate script error: " + ex.Message);
            }
        }

        private void GenerateScript(object state)
        {
            try
            {
                var project = state as Project;

                var filename = GetSelectedSolutionExplorerFileName();

                if (String.IsNullOrEmpty(filename))
                {
                    OutputWindowMessage.WriteMessage("Couldn't GetConfig filename");
                    return;
                }

                if (!filename.EndsWith(".sql"))
                {
                    OutputWindowMessage.WriteMessage("This only works with .sql files - boo hoo hoo");
                    return;
                }

                var procname = ScriptProperties.GetScriptDetail(File.ReadAllText(filename)).Name;
                if (string.IsNullOrEmpty(procname))
                {
                    OutputWindowMessage.WriteMessage("Couldn't GetConfig proc name - boo hoo hoo");
                    return;
                }
                var settings = Config.Configuration.GetSettings(project);

                WriteDeployFile(project.FullName, procname, filename, settings.DeploymentFolder);
                
            }
            catch (Exception e)
            {
                OutputWindowMessage.WriteMessage("Unable to generate script: " + e.Message);
            }
        }

        private void WriteDeployFile(string projectFile, string procName, string procFile, string outputDir)
        {
            var script = GetFileContents(procFile);
            if (string.IsNullOrEmpty(script))
            {
                OutputWindowMessage.WriteMessage("Could not read script file: " + procFile);
                return;
            }

            var variables = new SsdtVariableProvider().GetVariables(projectFile);

            foreach (SqlCmdVariable v in variables)
            {
                script = script.Replace(v.Name, v.Value);
            }

            var outputScript = new StringBuilder();
            outputScript.AppendFormat(DeploymentScriptGenerator.BuildDeploy(script));
            
            string outputScriptPath = Path.Combine(outputDir,
                string.Format("{0}.sql", DeploymentScriptGenerator.GetLastPartOfName(procName)));

            try
            {
                WriteFileContents(outputScriptPath, outputScript.ToString());
                OutputWindowMessage.WriteMessage("Written file: " + outputScriptPath);
            }
            catch (Exception e)
            {
                OutputWindowMessage.WriteMessage("Could not write file: " + outputScriptPath + " - error: " + e.Message);
            }
        }

        private void WriteFileContents(string fileName, string contents)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.Write(contents);
            }
        }

        private string GetFileContents(string procFile)
        {
            using (var reader = new StreamReader(procFile))
            {
                return reader.ReadToEnd();
            }
        }


        private void ShowToolsOptions(object sender, EventArgs e)
        {
            try
            {
                var project = GetCurrentProject();

                Config.Configuration.ShowConfig(project);

            }
            catch (Exception)
            {
                OutputWindowMessage.WriteMessage("Configuration Failed");
            }
        }

        private Project GetCurrentProject()
        {
            IntPtr hierarchyPointer, selectionContainerPointer;
            Object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint projectItemId;

            var monitorSelection =
                (IVsMonitorSelection) GetGlobalService(
                    typeof (SVsShellMonitorSelection));

            monitorSelection.GetCurrentSelection(out hierarchyPointer,
                out projectItemId,
                out multiItemSelect,
                out selectionContainerPointer);

            var selectedHierarchy = Marshal.GetTypedObjectForIUnknown(
                hierarchyPointer,
                typeof (IVsHierarchy)) as IVsHierarchy;

            if (selectedHierarchy != null)
            {
                ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(
                    projectItemId,
                    (int) __VSHPROPID.VSHPROPID_ExtObject,
                    out selectedObject));
            }


            var selectedProject = ((selectedObject as dynamic).ProjectItems.ContainingProject) as Project;

            return selectedProject;
        }

        private void DeploySingleFileCallback(object sender, EventArgs e)
        {
            try
            {
                var project = GetCurrentProject();
                var settings = Config.Configuration.GetSettings(project);
                if (null == settings)
                {
                    OutputWindowMessage.WriteMessage("Cancelled");
                    return;
                }
                ThreadPool.QueueUserWorkItem(DeploySingleFile, project);
            }
            catch (Exception)
            {
                OutputWindowMessage.WriteMessage("Deploying file Failed");
            }
        }

        private void DeploySingleFile(object state)
        {
            try
            {
                var project = state as Project;

                var variables = new SsdtVariableProvider().GetVariables(project.FullName);

                var settings = Config.Configuration.GetSettings(project);

                var filename = GetSelectedSolutionExplorerFileName();

                if (String.IsNullOrEmpty(filename))
                {
                    OutputWindowMessage.WriteMessage("Couldn't GetConfig filename");
                    return;
                }

                if (!filename.EndsWith(".sql"))
                {
                    OutputWindowMessage.WriteMessage("Single file deploy only works with .sql files - boo hoo hoo");
                    return;
                }

                var procname = ScriptProperties.GetScriptDetail(File.ReadAllText(filename)).Name;

                if (string.IsNullOrEmpty(procname))
                {
                    OutputWindowMessage.WriteMessage("Couldn't GetConfig proc name - boo hoo hoo");
                    return;
                }

                var fileContents = GetFileContents(filename);

                foreach (SqlCmdVariable v in variables)
                {
                    fileContents = fileContents.Replace(v.Name, v.Value);
                }


                if (!DtcAvailable())
                {
                    OutputWindowMessage.WriteMessage("Unable to deploy file, deploy uses msdtc to protect changes. Please ensure the service is enabled and running");
                    return;
                }
                
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        var script = DeploymentScriptGenerator.BuildDeploy(fileContents);
                        var batches = script.Split( new string[] {"\r\nGO\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var batch in batches)
                        {
                            new SqlGateway(settings.ConnectionString).Execute(batch);
                        }

                        scope.Complete();


                        OutputWindowMessage.WriteMessage(string.Format("Deployed File: {0}\r\n", filename));
                    }
                    catch (NullReferenceException)
                    {
                        OutputWindowMessage.WriteMessage(string.Format("Unable to deploy file {0}\r\n", filename));
                    }
                    catch (Exception ex)
                    {
                        OutputWindowMessage.WriteMessage(string.Format("Unable to deploy file {0} error : {1}\r\n",
                            filename, ex.Message));
                    }
                }
            }
            catch (Exception e)
            {
                OutputWindowMessage.WriteMessage("Deploying file Failed: " + e.Message);
            }

        }

        private bool DtcAvailable()
        {
            var controller = new ServiceController();
            controller.ServiceName = "msdtc";
            return (controller.Status == ServiceControllerStatus.Running);
        }
    }
}   
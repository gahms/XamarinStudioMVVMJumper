using System;
using System.Collections.Generic;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using System.Linq;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui.Dialogs;

namespace XamarinStudioMVVMJumper
{
    public class GoToCounterpartHandler : CommandHandler
    {
        protected override void Run ()
        {
            var editor = IdeApp.Workbench.ActiveDocument.Editor;
            var filename = editor.FileName.FileNameWithoutExtension;
            string newFilename = null;

            var page = StripPostfix(filename, "Page");
            var viewModel = StripPostfix(filename, "ViewModel");
            var serviceInterface = StripPrefixAndPostfix(filename, "I", "Service");
            var serviceImpl = StripPostfix(filename, "Service");

            if (page != null)
            {
                newFilename = $"{page}ViewModel.cs";
            }
            else if (viewModel != null)
            {
                newFilename = $"{viewModel}Page.xaml";
            }
            else if (serviceInterface != null)
            {
                newFilename = $"{serviceInterface}Service.cs";
            }
            else if (serviceImpl != null)
            {
                newFilename = $"I{serviceImpl}Service.cs";
            }

            var docs = FindDocWithFilename(newFilename);
            var count = docs.Count();
            if (count == 0)
            {
                IdeApp.Workbench.StatusBar.ShowMessage($"NOT found: {newFilename}");
            }
            else if (count == 1)
            {
                var file = docs.First();
                var doc = IdeApp.Workbench.OpenDocument(file.FilePath, file.Project, true).Result;                
            }
            else
            {
                IdeApp.Workbench.StatusBar.ShowMessage($"More than 1 found: {newFilename}");
            }
        }

        private string StripPostfix(string filename, string postfix)
        {
            if (filename.EndsWith(postfix, StringComparison.Ordinal))
            {
                return filename.Substring(0, filename.Length - postfix.Length);
            }
            else
            {
                return null;
            }

        }

        private string StripPrefixAndPostfix(string filename, string prefix, string postfix)
        {
            if (filename.StartsWith(prefix, StringComparison.Ordinal)
                && filename.EndsWith(postfix, StringComparison.Ordinal))
            {
                return filename.Substring(
                    prefix.Length, filename.Length - postfix.Length - prefix.Length);
            }
            else
            {
                return null;
            }
        }

        private IEnumerable<ProjectFile> FindDocWithFilename(string filename)
        {
            var files = IdeApp.Workspace.GetAllProjects().SelectMany(
                p => p.Files.Where(f => f.FilePath.FileName == filename));

            return files;
       }

        protected override void Update (CommandInfo info)
        {
            info.Enabled = IdeApp.Workbench.ActiveDocument?.Editor != null;
        }   
    }
}

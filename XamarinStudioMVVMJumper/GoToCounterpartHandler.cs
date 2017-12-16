using System;
using System.Collections.Generic;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;

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

            if (page != null)
            {
                newFilename = $"{page}ViewModel.cs";
            }
            else if (viewModel != null)
            {
                newFilename = $"{viewModel}Page.xaml";
            }

            var doc = FindDocWithFilename(newFilename);
            if (doc == null)
            {
                IdeApp.Workbench.StatusBar.ShowMessage($"NOT found: {newFilename}");
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

        private MonoDevelop.Ide.Gui.Document FindDocWithFilename(string filename)
        {
            foreach (var proj in IdeApp.Workspace.GetAllProjects())
            {
                foreach (var file in proj.Files)
                {                    
                    if (file.FilePath.FileName == filename)
                    {
                        return IdeApp.Workbench.OpenDocument(file.FilePath, file.Project, true).Result;
                    }
                }
            }

            return null;
        }

        protected override void Update (CommandInfo info)
        {
            info.Enabled = IdeApp.Workbench.ActiveDocument?.Editor != null;
        }   
    }
}

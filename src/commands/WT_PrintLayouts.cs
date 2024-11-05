// #! csharp

/*
    Script to print all of the kayouts existing in the current model to one PDF.
    This script depends on the WT_LayoutTools.cs library for some of its functionality.
*/

using System;
using System.IO;

using WorkflowTools;

var doc = Rhino.RhinoDoc.ActiveDoc;

var sfd = new Rhino.UI.SaveFileDialog();
sfd.Title = "Where do you want to save the PDF?";
sfd.DefaultExt = ".pdf";
sfd.Filter = ".pdf";
sfd.FileName = ".pdf";
var rc = sfd.ShowSaveDialog();
Console.WriteLine(sfd.FileName);

var dpi = 300;

string[] skip = {"Template"};

LayoutTools.PrintLayouts(doc, sfd.FileName, dpi, skip);
// #! csharp

//from https://github.com/mcneel/aectech_2024_scripteditor/blob/main/samples/part3/project/BatchPrinting_Command.cs 

using System;
using System.IO;

using Rhino;

using WorkflowTools;

var doc = RhinoDoc.ActiveDoc;

var sfd = new Rhino.UI.SaveFileDialog();
sfd.DefaultExt = ".pdf";
sfd.Filter = ".pdf";
sfd.FileName = ".pdf";
var rc = sfd.ShowSaveDialog();
Console.WriteLine(sfd.FileName);

var dpi = 300;

string[] skip = {"Template"};

LayoutTools.PrintLayouts(doc, sfd.FileName, dpi, skip);
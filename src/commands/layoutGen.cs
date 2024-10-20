// NOTE:
// - Reference to RhinoCommmon.dll is added by default
// - Use #r "nuget: <package name>, <package version>" to install and reference nuget packages.
//       e.g. #r "nuget: Rhino.Scripting, 0.7.0"
//       e.g. #r "nuget: RestSharp, 106.11.7"
// - Use #r "<assembly name>" to reference other assemblies
//       e.g. #r "System.Text.Json.dll"
//       e.g. #r "path/to/your/Library.dll"
//       e.g. #r "path/to/your/Plugin.gha"

using System;
using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;

var numCells = 5;
var doc = RhinoDoc.ActiveDoc;

var width = 297;
var height = 210;
var margin = 5;
var dWidth = 100;
var dHeight = 100;

for (int i = 0; i < numCells; i ++)
{
    var objs = doc.Objects.FindByUserString("cell", i.ToString(), false);
    var filter = new  ObjectEnumeratorSettings();
    filter.NameFilter = "cell" + i.ToString()
    var cell = doc.Objects.FindByFilter()
    
    var numObjs = objs.Length;
    Console.WriteLine("Number of objects: {0}", objs.Length);

    var oldAV = doc.Views.ActiveView;

    for ( int j = 0; j < numObjs; j ++) 
    {
        var obj = objs[j];
        var bb = objs[j].Geometry.GetBoundingBox(false);
        //doc.Objects.AddBox( new Box(bb));

        var cell = obj.Attributes.GetUserString("cell");
        var area = obj.Attributes.GetUserString("area");
        var level = obj.Attributes.GetUserString("level");

        var pv = doc.Views.AddPageView("Entity: " + i.ToString() + "-" + j.ToString(), width, height);

        var planDetail = pv.AddDetailView("Plan", new Point2d(margin,height-dHeight), new Point2d(margin+dWidth,height-margin), Rhino.Display.DefinedViewportProjection.Top);

        var vp = planDetail.Viewport;
        var res = vp.ZoomBoundingBox(bb);
        planDetail.CommitViewportChanges();
        //planDetail.CommitChanges();

        // text
        doc.Views.ActiveView = pv;
        doc.Objects.AddTextDot("Entity: " + i.ToString() + "-" + j.ToString(),new Point3d(100,100,0));

        doc.Views.ActiveView = oldAV;
    }

    

}




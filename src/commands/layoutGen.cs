// NOTE:
// - Reference to RhinoCommmon.dll is added by default
// - Use #r "nuget: <package name>, <package version>" to install and reference nuget packages.
//       e.g. #r "nuget: Rhino.Scripting, 0.7.0"
//       e.g. #r "nuget: RestSharp, 106.11.7"
// - Use #r "<assembly name>" to reference other assemblies
//       e.g. #r "System.Text.Json.dll"
//       e.g. #r "path/to/your/Library.dll"
//       e.g. #r "path/to/your/Plugin.gha"
// #! csharp

using System;

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;

var doc = RhinoDoc.ActiveDoc;

// some layout parameters
var width = 297;
var height = 210;
var margin = 5;
var dWidth = 100;
var dHeight = 100;

var zone_param_text_height = 6;
var zone_param_fontName = "Arial";

var displayMode = Rhino.Display.DisplayModeDescription.FindByName("Rendered_WS");
if( null == displayMode)
    displayMode = Rhino.Display.DisplayModeDescription.FindByName("Rendered");

//keep the view you had before starting the script
var og_activeView = doc.Views.ActiveView;
var og_layer_index = doc.Layers.CurrentLayerIndex;

//check if we have the detail layers available?
Layer layouts_layer = doc.Layers.FindName("Layouts");
Layer details_layer = doc.Layers.FindName("Details");
Layer txt_layer = doc.Layers.FindName("Text");


// We will check if the parent layer exists
// We could also check if the other layers exists, but we will leave that as an exercise to the participants

if(null == layouts_layer) 
{
    
    int layouts_layer_index = doc.Layers.Add("Layouts", System.Drawing.Color.Black);
    layouts_layer = doc.Layers.FindIndex(layouts_layer_index);

    int details_layer_index = doc.Layers.Add("Details", System.Drawing.Color.Black);
    details_layer = doc.Layers.FindIndex(details_layer_index);
    details_layer.ParentLayerId = layouts_layer.Id;

    int txt_layer_index = doc.Layers.Add("Text", System.Drawing.Color.Black);
    txt_layer = doc.Layers.FindIndex(txt_layer_index);
    txt_layer.ParentLayerId = layouts_layer.Id;

}

// go through zones
var zones = doc.Objects.FindByLayer("ZONES");
Console.WriteLine("Nº of Zones: {0}",zones.Length);

for (int i = 0; i < zones.Length; i ++) 
{
    int zone_index = i + 1; // zone object name is 1-4
    var zone_name = zone_index.ToString();
    var zone = Array.Find(zones, zone => zone.Name == zone_name); //find the zone with the current name
    var zone_bb = zone.Geometry.GetBoundingBox(false); // get the zone BB
    
    var pv = doc.Views.AddPageView("Zone: " + zone_name, width, height); //add page view aka layout

    var detail = pv.AddDetailView("View", new Point2d(margin,height-dHeight), new Point2d(margin+dWidth,height-margin), Rhino.Display.DefinedViewportProjection.Perspective); // add a detail to the layout
    detail.Attributes.LayerIndex = details_layer.Index; // change to detail layer

    // edit detail vp
    var vp = detail.Viewport;
    vp.ChangeToParallelProjection(true);
    vp.DisplayMode = displayMode;
    vp.ZoomBoundingBox(zone_bb);
    detail.CommitViewportChanges();

    doc.Views.ActiveView = pv; //switch the active view to the layout so new objects land here

    // text
    var txt_oa = new Rhino.DocObjects.ObjectAttributes();
    txt_oa.LayerIndex = txt_layer.Index; // change to the text layer
    doc.Objects.AddTextDot("Zone: " + zone_name, new Point3d(100,100,0), txt_oa );

    // get zone data
    var builtGFA = zone.Attributes.GetUserString("BuiltGFA");
    var landuseGFA = zone.Attributes.GetUserString("LanduseGFA");
    var totalGFA = zone.Attributes.GetUserString("TotalGFA");
    var landuses = zone.Attributes.GetUserString("Landuses");
    var ratio = zone.Attributes.GetUserString("Ratio");

    var builtGFA_plane = Plane.WorldXY;
    builtGFA_plane.Origin = new Point3d(150, height - 20, 0);
    var builtGFA_text = "Built GFA: " + builtGFA + " m2";
    doc.Objects.AddText(builtGFA_text, builtGFA_plane, zone_param_text_height, zone_param_fontName, false, false, txt_oa);

    var landuseGFA_plane = Plane.WorldXY;
    landuseGFA_plane.Origin = new Point3d(150, height - 30, 0);
    var landuseGFA_text = "Landuse GFA: " + landuseGFA + " m2";
    doc.Objects.AddText(landuseGFA_text, landuseGFA_plane, zone_param_text_height, zone_param_fontName, false, false, txt_oa);

    var totalGFA_plane = Plane.WorldXY;
    totalGFA_plane.Origin = new Point3d(150, height - 40, 0);
    var totalGFA_text = "Total GFA: " + totalGFA + " m2";
    doc.Objects.AddText(totalGFA_text, totalGFA_plane, zone_param_text_height, zone_param_fontName, false, false, txt_oa);

    var ratio_plane = Plane.WorldXY;
    ratio_plane.Origin = new Point3d(150, height - 50, 0);
    var ratio_text = "Ratio: " + ratio + " %";
    doc.Objects.AddText(ratio_text, ratio_plane, zone_param_text_height, zone_param_fontName, false, false, txt_oa);

    doc.Views.ActiveView = og_activeView; //reset view


}
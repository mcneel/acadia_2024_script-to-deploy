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
using System.Collections.Generic;

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

var zone_param_text_height = 3;
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

// collect objects
var zones = doc.Objects.FindByLayer("ZONES");
Console.WriteLine("Nº of Zones: {0}",zones.Length);
var plots = doc.Objects.FindByLayer("PLOTS");
Console.WriteLine("Nº of Plots: {0}",plots.Length);
var bldgs = doc.Objects.FindByLayer("BUILDING PADS");
Console.WriteLine("Nº of Bldgs: {0}",bldgs.Length);

for (int i = 0; i < zones.Length; i ++) 
{
    int zone_index = i + 1; // zone object name is 1-4
    var zone_name = zone_index.ToString();
    Console.WriteLine("--- Zone {0} ---",zone_name);
    var zone = Array.Find(zones, zone => zone.Name == zone_name); //find the zone with the current name
    var zone_bb = zone.Geometry.GetBoundingBox(false); // get the zone BB
    zone_bb.Inflate(10);
        
    var pv = doc.Views.AddPageView("Zone: " + zone_name, width, height); //add page view aka layout

    doc.Views.ActiveView = pv; //switch the active view to the layout so new objects land here

    var detail = pv.AddDetailView("View", new Point2d(margin,height-dHeight), new Point2d(width - margin,height-margin), Rhino.Display.DefinedViewportProjection.Perspective); // add a detail to the layout
    detail.Attributes.LayerIndex = details_layer.Index; // change to detail layer

    // edit detail vp
    var vp = detail.Viewport;
    vp.ChangeToParallelProjection(true);
    vp.DisplayMode = displayMode;
    vp.ZoomBoundingBox(zone_bb);
    detail.CommitViewportChanges();

    

    // text
    var txt_oa = new Rhino.DocObjects.ObjectAttributes();
    txt_oa.LayerIndex = txt_layer.Index; // change to the text layer

    // get zone metrics
    var builtGFA = zone.Attributes.GetUserString("BuiltGFA");
    var landuseGFA = zone.Attributes.GetUserString("LanduseGFA");
    var landuseGFA_uses = landuseGFA.Split(',', StringSplitOptions.None);
    var totalGFA = zone.Attributes.GetUserString("TotalGFA");
    var landuses = zone.Attributes.GetUserString("Landuses");
    var ratio = zone.Attributes.GetUserString("Ratio");

    var zone_plane = Plane.WorldXY;
    zone_plane.Origin = new Point3d(margin, (height - dHeight) - zone_param_text_height * 2 , 0);
    var zone_text = "Zone " + zone_name;
    doc.Objects.AddText(zone_text, zone_plane, zone_param_text_height * 2, zone_param_fontName, false, false, txt_oa);

    var zone_param_text = "Built GFA: " + builtGFA + " m2";
    zone_param_text += "\n";
    zone_param_text += "Landuse:";
    zone_param_text += "\n";

    var landuse = landuseGFA.Split(',');
    for( int j = 0; j < landuse.Length; j ++ )
    {
        zone_param_text += "    " + landuse[j] + "m2";
        zone_param_text += "\n";
    }

    zone_param_text += "Total GFA: " + totalGFA + " m2";
    zone_param_text += "\n";
    zone_param_text += "Ratio: " + ratio + " %";
    zone_param_text += "\n";

    var zone_param_plane = Plane.WorldXY;
    zone_param_plane.Origin = new Point3d(margin, (height - dHeight) - zone_param_text_height * 2 - 10, 0);
    doc.Objects.AddText(zone_param_text, zone_param_plane, zone_param_text_height, zone_param_fontName, false, false, txt_oa);

    // PLOTS

    var zone_plots = Array.FindAll(plots, plot => plot.Name.StartsWith(zone_name));
    Console.WriteLine("Nº Plots in Zone {0}: {1}",zone_name, zone_plots.Length);

    //go through plots

    foreach (var plot in zone_plots)
    {

        var plot_bb = plot.Geometry.GetBoundingBox(false); // get the zone BB
        plot_bb.Inflate(10);

        //get plot metrics
        var p_builtGFA = plot.Attributes.GetUserString("BuiltGFA");
        var p_landuseGFA = plot.Attributes.GetUserString("LanduseGFA");
        var p_landuseGFA_uses = p_landuseGFA.Split(',', StringSplitOptions.None);
        var p_totalGFA = plot.Attributes.GetUserString("TotalGFA");
        var p_landuses = plot.Attributes.GetUserString("Landuses");
        var p_ratio = plot.Attributes.GetUserString("Ratio");

        var p_pv = doc.Views.AddPageView("Plot: " + plot.Name, width, height); //add page view aka layout

        doc.Views.ActiveView = p_pv; //switch the active view to the layout so new objects land here

        var p_detail = p_pv.AddDetailView("View", new Point2d(margin,height-dHeight), new Point2d(width - margin,height-margin), Rhino.Display.DefinedViewportProjection.Perspective); // add a detail to the layout
        p_detail.Attributes.LayerIndex = details_layer.Index; // change to detail layer

        // edit detail vp
        var p_vp = p_detail.Viewport;
        p_vp.ChangeToParallelProjection(true);
        p_vp.DisplayMode = displayMode;
        p_vp.ZoomBoundingBox(plot_bb);
        p_detail.CommitViewportChanges();

        var plot_plane = Plane.WorldXY;
        plot_plane.Origin = new Point3d(margin, (height - dHeight) - zone_param_text_height * 2 , 0);
        var plot_text = "Plot " + plot.Name;
        doc.Objects.AddText(plot_text, plot_plane, zone_param_text_height * 2, zone_param_fontName, false, false, txt_oa);

    }

    doc.Views.ActiveView = og_activeView; //reset view

}
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

using WorkflowTools;

var doc = RhinoDoc.ActiveDoc;

// layout parameters

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

var layout_params = new LayoutTools.LayoutParams {

    PageWidth = 297,
    PageHeight = 210,
    DetailHeight = 100,
    TitleTextHeight = 6,
    TextHeight = 3,
    Margin = 5,
    DetailsLayerIndex = details_layer.Index,
    TextLayerIndex = txt_layer.Index,
    FontName = "Arial",
    DisplayMode = displayMode
};

// collect objects
var zones = doc.Objects.FindByLayer("ZONES");
Console.WriteLine("Nº of Zones: {0}",zones.Length);
var plots = doc.Objects.FindByLayer("PLOTS");
Console.WriteLine("Nº of Plots: {0}",plots.Length);
var bldgs = doc.Objects.FindByLayer("BUILDING PADS");
Console.WriteLine("Nº of Bldgs: {0}",bldgs.Length);

for (int i = 0; i < zones.Length; i ++) 
{
    // ZONES
    int zone_index = i + 1; // zone object name is 1-4
    var zone_name = zone_index.ToString();
    Console.WriteLine("--- Zone {0} ---",zone_name);
    var zone = Array.Find(zones, zone => zone.Name == zone_name); //find the zone with the current name

    LayoutTools.CreateLayout(doc, zone, layout_params);

    var zone_plots = Array.FindAll(plots, plot => plot.Name.StartsWith(zone_name));
    Console.WriteLine("Nº Plots in Zone {0}: {1}",zone_name, zone_plots.Length);

    // PLOTS
    foreach (var plot in zone_plots)
        LayoutTools.CreateLayout(doc, plot, layout_params);

    doc.Views.ActiveView = og_activeView; //reset view

}
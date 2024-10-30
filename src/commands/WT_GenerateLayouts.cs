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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;

using WorkflowTools;

// create and start a Stopwatch instance
Stopwatch stopwatch = Stopwatch.StartNew();

var doc = RhinoDoc.ActiveDoc;

// layout parameters

var activeDisplayMode = Rhino.Display.DisplayModeDescription.FindByName("Rendered_WS");
if( null == activeDisplayMode)
    activeDisplayMode = Rhino.Display.DisplayModeDescription.FindByName("Rendered");

var hiddenDisplayMode = Rhino.Display.DisplayModeDescription.FindByName("Arctic_WS");
if( null == hiddenDisplayMode)
    hiddenDisplayMode = Rhino.Display.DisplayModeDescription.FindByName("Arctic");

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
    ActiveDisplayMode = activeDisplayMode,
    HiddenDisplayMode = hiddenDisplayMode

};

// collect objects
var zones = doc.Objects.FindByLayer("ZONES");
Console.WriteLine("Nº of Zones: {0}",zones.Length);

// very simple check to see if we've added user strings to objects
if(zones[0].Attributes.UserStringCount == 0) return;

var plots = doc.Objects.FindByLayer("PLOTS");
Console.WriteLine("Nº of Plots: {0}",plots.Length);
var bldg_pads = doc.Objects.FindByLayer("BUILDING PADS");
Console.WriteLine("Nº of Bldg Pads: {0}",bldg_pads.Length);
var bldg_parts = doc.Objects.FindByObjectType(ObjectType.Extrusion);

doc.Views.EnableRedraw(false, false, false);

for (int i = 0; i < zones.Length; i ++) 
{
    // ZONES
    int zone_index = i + 1; // zone object name is 1-4
    var zone_name = zone_index.ToString();
    Console.WriteLine("--- Zone {0} ---",zone_name);
    var zone = Array.Find(zones, zone => zone.Name == zone_name); //find the zone with the current name

    var plots_out_zone = Array.FindAll(plots, p => p.Name[0].ToString() != zone_name);
    Console.WriteLine("Plots not in Zone: {0}", plots_out_zone.Length);
    var bldgs_out_zone = Array.FindAll(bldg_parts, b => b.Name[0].ToString() != zone_name);
    Console.WriteLine("Buildings not in Zone: {0}", bldgs_out_zone.Length);
    var objs = plots_out_zone.Concat(bldgs_out_zone).ToArray();
    Console.WriteLine("Objects not in Zone: {0}", objs.Length);

    LayoutTools.CreateLayout(doc, zone, bldg_parts, layout_params);

    var zone_plots = Array.FindAll(plots, plot => plot.Name.StartsWith(zone_name));
    Console.WriteLine("Nº Plots in Zone {0}: {1}",zone_name, zone_plots.Length);

    // PLOTS
    foreach (var plot in zone_plots) 
    {
        var bldgs_out_plot = Array.FindAll(bldg_parts, b => !b.Name.StartsWith(plot.Name));
        Console.WriteLine("Buildings not in Plot: {0}", bldgs_out_plot.Length);
        LayoutTools.CreateLayout(doc, plot, bldg_parts, layout_params);
    }

    //doc.Views.ActiveView = og_activeView; //reset view
    doc.Views.EnableRedraw(true, true, true);
    stopwatch.Stop();
    Console.WriteLine("Time to generate layouts (ms): ",stopwatch.ElapsedMilliseconds);

}
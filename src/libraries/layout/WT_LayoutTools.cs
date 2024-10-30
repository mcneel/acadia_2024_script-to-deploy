// #! csharp
using System;
using System.Collections.Generic;
using System.Linq;

using Rhino;
using Rhino.FileIO;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Display;

namespace WorkflowTools {

    public static class LayoutTools
    {
        public struct LayoutParams
        {
            public int PageWidth {get; set;}
            public int PageHeight {get; set;}
            public int DetailWidth {get; set;}
            public int DetailHeight {get; set;}
            public int TitleTextHeight {get; set;}
            public int TextHeight {get; set;}
            public int Margin {get; set;}
            public int DetailsLayerIndex {get; set;}
            public int TextLayerIndex {get; set;}
            public string FontName {get; set;}
            public DisplayModeDescription ActiveDisplayMode {get; set;}
            public DisplayModeDescription HiddenDisplayMode {get; set;}
        }

        //TODO: Change other object display modes in detail? https://developer.rhino3d.com/api/rhinocommon/rhino.docobjects.objectattributes/setdisplaymodeoverride
        //TODO: Hide objects in detail? https://developer.rhino3d.com/api/rhinocommon/rhino.docobjects.objectattributes/addhideindetailoverride#(guid)
        public static void CreateLayout(RhinoDoc doc, RhinoObject ro, RhinoObject[] ros, LayoutParams lp)
        {

            var obj_out = Array.FindAll(ros, o => !o.Name.StartsWith(ro.Name));
            var obj_in = Array.FindAll(ros, o => o.Name.StartsWith(ro.Name));

            var bb = ro.Geometry.GetBoundingBox(false);
            foreach (var obj in obj_in)
                bb.Union(obj.Geometry.GetBoundingBox(false));

            bb.Inflate(2);

            var type = ro.Attributes.GetUserString("Type");
            var name = ro.Name;

            var pv = doc.Views.AddPageView(type + ": " + name, lp.PageWidth, lp.PageHeight); //add page view aka layout
            //doc.Views.ActiveView = pv; //switch the active view to the layout so new objects land here

            var detail = pv.AddDetailView("View", new Point2d(lp.Margin, lp.PageHeight - lp.DetailHeight), new Point2d( lp.PageWidth - lp.Margin, lp.PageHeight - lp.Margin ), Rhino.Display.DefinedViewportProjection.Perspective); // add a detail to the layout
            detail.Attributes.LayerIndex = lp.DetailsLayerIndex; // change to detail layer

            // edit detail vp
            var vp = detail.Viewport;
            vp.ChangeToParallelProjection(true);
            vp.DisplayMode = lp.ActiveDisplayMode;
            vp.ZoomBoundingBox(bb);
            detail.CommitViewportChanges();
            var vp_id = vp.Id;

            var detail_map = pv.AddDetailView("Map", new Point2d(lp.PageWidth-lp.Margin-100, lp.Margin), new Point2d( lp.PageWidth - lp.Margin, 100 + lp.Margin ), Rhino.Display.DefinedViewportProjection.Top); // add a detail to the layout
            detail_map.Attributes.LayerIndex = lp.DetailsLayerIndex; // change to detail layer

            var vp_map = detail_map.Viewport;
            vp_map.ChangeToParallelProjection(true);
            vp_map.DisplayMode = lp.ActiveDisplayMode;
            vp_map.ZoomExtents();
            detail_map.CommitViewportChanges();
            var vp_map_id = vp_map.Id;

            //shade other objects with secondary style
            foreach (var obj in obj_out) 
            {
                obj.Attributes.SetDisplayModeOverride(lp.HiddenDisplayMode, vp_id);
                obj.Attributes.SetDisplayModeOverride(lp.HiddenDisplayMode, vp_map_id);
                obj.CommitChanges();
            }

            // text
            var txt_oa = new Rhino.DocObjects.ObjectAttributes();
            txt_oa.LayerIndex = lp.TextLayerIndex; // change to the text layer
            txt_oa.Space = ActiveSpace.PageSpace;
            txt_oa.ViewportId = pv.MainViewport.Id;

            // get metrics
            // Could be abstract by getting var ud = ro.Attributes.GetUserStrings(); and iterating over the collection
            var builtGFA = ro.Attributes.GetUserString("BuiltGFA");
            var landuseGFA = ro.Attributes.GetUserString("LanduseGFA");
            var landuseGFA_uses = landuseGFA.Split(',', StringSplitOptions.None);
            var totalGFA = ro.Attributes.GetUserString("TotalGFA");
            var landuses = ro.Attributes.GetUserString("Landuses");
            var ratio = ro.Attributes.GetUserString("Ratio");

            var title_plane = Plane.WorldXY;
            title_plane.Origin = new Point3d(lp.Margin, ( lp.PageHeight - lp.DetailHeight ) - lp.TitleTextHeight , 0);
            var title_text = type + " " + name;
            doc.Objects.AddText(title_text, title_plane, lp.TitleTextHeight, lp.FontName, false, false, txt_oa);

            var param_text = "Built GFA: " + builtGFA + " m2";
            param_text += "\n";
            param_text += "Total GFA: " + totalGFA + " m2";
            param_text += "\n";
            param_text += "Ratio: " + ratio + " %";
            param_text += "\n";
            param_text += "Landuse:";
            param_text += "\n";

            var landuse = landuseGFA.Split(',');
            //var landuseDict = new Dictionary<string, int[]>();
            //int.TryParse(builtGFA, out int builtm2);
            for( int j = 0; j < landuse.Length; j ++ )
            {
                param_text += "    " + landuse[j] + "m2";
                param_text += "\n";
                
                // create dictionary for pie chart
/*
                var parts = landuse[j].Split(':', StringSplitOptions.None);
                int.TryParse(parts[1], out int m2);
                var percentTotal = (int)System.Math.Round( (double)((m2/builtm2) * 100));
                landuseDict.Add(parts[0], new []{m2, percentTotal});
                */
            }

           // var ordered = landuseDict.OrderBy(x => x.Value[1]);


            var param_plane = Plane.WorldXY;
            param_plane.Origin = new Point3d(lp.Margin, (lp.PageHeight - lp.DetailHeight ) - lp.TextHeight * 2 - 10, 0);
            doc.Objects.AddText(param_text, param_plane, lp.TextHeight, lp.FontName, false, false, txt_oa);

        }

        //from https://github.com/mcneel/aectech_2024_scripteditor/blob/main/samples/part3/project/BatchPrinting_Command.cs 
        public static void PrintLayouts(RhinoDoc doc, string path, int dpi, string[] skip) 
        {
            var pdf = FilePdf.Create();
            var pages = doc.Views.GetPageViews();

            foreach (RhinoPageView page in pages)
            {
                // maybe we want to skip some layout?
                if(Array.IndexOf(skip, page.PageName) > -1)
                    continue;

                var capture = new ViewCaptureSettings(page, dpi);
                capture.OutputColor = 0;
                capture.TextDotPointSize = 8.0;
                capture.DefaultPrintWidthMillimeters = 0.2;
                pdf.AddPage(capture);
            }

            pdf.Write(path);

        } 
    }

}
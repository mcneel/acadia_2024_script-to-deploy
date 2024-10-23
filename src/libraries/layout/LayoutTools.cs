// #! csharp
using System;
using System.Collections.Generic;

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
            public DisplayModeDescription DisplayMode {get; set;}
        }

        public static void CreateLayout(RhinoDoc doc, RhinoObject ro, LayoutParams lp)
        {

            var bb = ro.Geometry.GetBoundingBox(false);
            bb.Inflate(10);

            var type = ro.Attributes.GetUserString("Type");
            var name = ro.Name;

            var pv = doc.Views.AddPageView(type + ": " + name, lp.PageWidth, lp.PageHeight); //add page view aka layout
            doc.Views.ActiveView = pv; //switch the active view to the layout so new objects land here

            var detail = pv.AddDetailView("View", new Point2d(lp.Margin, lp.PageHeight - lp.DetailHeight), new Point2d( lp.PageWidth - lp.Margin, lp.PageHeight - lp.Margin ), Rhino.Display.DefinedViewportProjection.Perspective); // add a detail to the layout
            detail.Attributes.LayerIndex = lp.DetailsLayerIndex; // change to detail layer

            // edit detail vp
            var vp = detail.Viewport;
            vp.ChangeToParallelProjection(true);
            vp.DisplayMode = lp.DisplayMode;
            vp.ZoomBoundingBox(bb);
            detail.CommitViewportChanges();

            // text
            var txt_oa = new Rhino.DocObjects.ObjectAttributes();
            txt_oa.LayerIndex = lp.TextLayerIndex; // change to the text layer

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
            for( int j = 0; j < landuse.Length; j ++ )
            {
                param_text += "    " + landuse[j] + "m2";
                param_text += "\n";
            }

            var param_plane = Plane.WorldXY;
            param_plane.Origin = new Point3d(lp.Margin, (lp.PageHeight - lp.DetailHeight ) - lp.TextHeight * 2 - 10, 0);
            doc.Objects.AddText(param_text, param_plane, lp.TextHeight, lp.FontName, false, false, txt_oa);

        }

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
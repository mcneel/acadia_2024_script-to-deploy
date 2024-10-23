// #! csharp

var doc = Rhino.RhinoDoc.ActiveDoc;

foreach(var ro in doc.Objects)
    ro.Attributes.DeleteAllUserStrings();


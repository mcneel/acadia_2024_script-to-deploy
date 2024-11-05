// #! csharp

/*
    USE WITH CAUTION.
    This script is a small utility that removes any Display Mode Overrides an object might have. 
    We use Display Mode Overrides in the Layout Details for shading some objects with a different display mode.
    As we create layouts and destroy them, this override data stays behind and makes the file size much bigger.
    It also makes generating the layouts on Windows very very slow.
    Clearing these out from time to time ensures that we can still generate the layouts on Windows.
    Keep in mind this deletes any overrides, even if the object has an override set in an existing layout. 
    USE WITH CAUTION.
*/

using System;
using Rhino;

var doc = RhinoDoc.ActiveDoc;

foreach (var ro in doc.Objects)
{
    ro.Attributes.RemoveDisplayModeOverride();
    ro.Attributes.RemoveDisplayModeOverride(Guid.Empty);
}

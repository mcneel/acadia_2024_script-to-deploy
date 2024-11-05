#! python3

# Utility script to delete all of the existing layouts in the current model

import System
import System.Collections.Generic
import Rhino

doc = Rhino.RhinoDoc.ActiveDoc
pvs = doc.Views.GetPageViews()
for pv in pvs:
    pv.Close()

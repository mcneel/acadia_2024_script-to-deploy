#! python 2

import rhinoscriptsyntax as rs

objs = rs.AllObjects()
for obj in objs:
    print(obj)
    keys = rs.GetUserText(obj, None, False)
    for key in keys:
        value = rs.GetUserText(obj, key, False)
        rs.SetUserText(obj, key, None, False)

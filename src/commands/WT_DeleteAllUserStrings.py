#! python 2
# https://discourse.mcneel.com/t/deleting-usertext-keys-and-data-from-a-script/54063/8

# Utility script to delete all of the user strings from all object in the current model

import rhinoscriptsyntax as rs

objs = rs.AllObjects()
for obj in objs:
    keys = rs.GetUserText(obj, None, False)
    for key in keys:
        value = rs.GetUserText(obj, key, False)
        rs.SetUserText(obj, key, None, False)

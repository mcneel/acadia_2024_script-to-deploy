![cover](docs/img/banner.png)

# Scripting in Practice: from script to deploy
A workshop presented on Monday, November 11, 2024 at Acadia in Calgary, Canada

This workshop will introduce the Code Editor in Rhino 8 - a new framework designed for user-friendly tool development and deployment). Using a real-life industry case study from an architectural practice, we'll guide you through identifying a suite of tools that streamline project deliverables, with a particular focus on documentation and geometry attributes. The goal of the workshop is to equip you with the skills to integrate these tools as a plugin, easily shareable across teams. We will kick off with an overview of the new Code Editor, followed by hands-on development of tools in Python, C#, and Grasshopper definitions. You will learn how to use Script Editor to package these tools into a Rhino toolbar and deploy them as plugins using Rhino’s Package Manager. We will also go through version control system and enabling automated deployments through continuous integration.​​

## Workshop Tutors

| <img src="docs/img/esther.png" alt="esther" width="100" height="100">  | <img src="docs/img/ivana.png" alt="ivana" width="100" height="100"> | <img src="docs/img/jeffrey.png" alt="jeffrey" width="100" height="100"> | <img src="docs/img/ehsan.png" alt="ehsan" width="100" height="100"> | <img src="docs/img/luis.png" alt="luis" width="100" height="100"> |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| [Esther Rubio Madroñal (Grimshaw)](https://www.linkedin.com/in/esther-rubio-madro%C3%B1al-275776129/) | [Ivana Petrusevski (Grimshaw)](https://www.linkedin.com/in/ivana-petrusevski-77a84b121/) | [Jeffrey Moser (Grimshaw)](https://www.linkedin.com/in/jeffrey-moser-823b39135/) | [Ehsan Iran-Nejad (McNeel)](https://www.linkedin.com/in/eirannejad/) | [Luis E. Fraguada (McNeel)](https://www.linkedin.com/in/fraguada/) |

​
## Project

The scripts and definitions in this repository are a collection of tools we've developed to show off the functionality of the new Rhino ScriptEditor. We have used python, csharp, and gh definitons to create the commands.

The toolkit contains the following scripts:

- [**WT_CalculateMetrics.gh**](src/commands/WT_CalculateMetrics.gh) - This GH definition reads obejcts from the current model, organizes them, calculates certain metrics like GFA, and then populates the object's user strings with these metrics. Finally the objects are updated in the Rhino model.
- [**WT_GenerateLayouts.cs**](src/commands/WT_GenerateLayouts.cs) - This script generates a layout with two details for each Zone and Plot in the reference 3d model. Some of the functionality of this script is referenced from a library called [WT_LayoutTools.cs](src/libraries/layout/WT_LayoutTools.cs)
- [**WT_ExportCSV.gh**](src/commands/WT_ExportToCSV.gh) - This GH definition exports a csv file with the object user strings from the reference model.
- [**WT_PrintLayouts.cs**](src/commands/WT_PrintLayouts.cs) - This script prints all of the existing layouts in the current file to one PDF. Depends on functionality in [WT_LayoutTools.cs](src/libraries/layout/WT_LayoutTools.cs) 
- [**WT_LayoutTools**](src/libraries/layout/WT_LayoutTools.cs) - A library containing some of the functionality used by the scripts in the toolkit.
- [**WT_DeleteAllUserStrings.py**](src/commands/WT_DeleteAllUserStrings.py) - As the name suggests, this script deletes all of the user strings on all of the objects in the current model.
- [**WT_DeleteLayouts.py**](src/commands/WT_DeleteLayouts.py) - This script deletes all of the existing layouts in the current model.
- [**WT_RemoveObjectDisplayModeOverrides.cs**](src/commands/WT_RemoveObjectDisplayModeOverrides.cs) - We found that as we were working on the reference model, objects were accumulating DisplayModeOverrides from the WT_GenerateLayouts script and generating layouts became very slow on Windows. This deletes any Display Mode Overrides from all objects in the current model

Additionally, there are a few other files of interest in the repository:

- [**ref/24.11.05_MasterplanBuildings_Start.3dm.zip**](ref/24.11.05_MasterplanBuildings_Start.3dm.zip) - The reference model we will use for demonstrating how the toolkit works.
- [**ref/Rendered_WS.ini**](ref/Rendered_WS.ini) - The display mode for focused objects in layout details.
- [**ref/Arctic_WS.ini**](ref/Arctic_WS.ini) - The display mode override for objects not in focus in layout details.
- [**.github/workflows/workflow_ci.yml**](.github/workflows/workflow_ci.yml) - The workflow file describing the actions to take on each push or pull request to this repository. Each time there is a new commit, this workflow runs and the result is a yak package artifact. 
- [**.github/workflows/workflow_deploy.yml**](.github/workflows/workflow_deploy.yml) - The workflow file describing the actions to take when we want to deploy this package to the public Package Manager. This workflow is activated on the push of a button. 
​

​

​

​

​

​

​

​

​

​

​

​

​

​

​

​

​

​

​​

​

​​

​

​

​

​

​

​

​

​

​

​

​

​​

Ivana Petrusevski]()
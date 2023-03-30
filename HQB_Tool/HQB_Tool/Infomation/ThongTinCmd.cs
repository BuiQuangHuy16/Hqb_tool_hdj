#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace HqbTool
{
    [Transaction(TransactionMode.Manual)]
    public class ThongTinCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            About about = new About("HQB_Tool", "1.0", "HUYBQ", "Tool for Revit");

            TaskDialog.Show("TinHocHDJ", about.GetAboutInformation());

            return Result.Succeeded;
        }
    }
}
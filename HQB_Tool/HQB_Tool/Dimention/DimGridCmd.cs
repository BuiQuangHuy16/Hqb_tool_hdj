
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Documents;
using Autodesk.Revit.UI.Selection;
using Application = Autodesk.Revit.ApplicationServices.Application;


#endregion

namespace HqbTool
{
    [Transaction(TransactionMode.Manual)]
    public class DimGridCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Khi chạy bằng Add-in Manager thì comment 2 dòng bên dưới để tránh lỗi
            // When running with Add-in Manager, comment the 2 lines below to avoid errors
            //string dllFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //AssemblyLoader.LoadAllRibbonAssemblies(dllFolder);


            FilterGrid filter = new FilterGrid();
            var grids = uidoc.Selection.PickElementsByRectangle(filter, "Pick Grid Lines");

            ReferenceArray refArray = new ReferenceArray();
            XYZ dir = null;

            foreach (Element el in grids)
            {
                Grid gr = el as Grid;

                if (gr == null) continue;
                if (dir == null)
                {
                    Curve crv = gr.Curve;
                    dir = new XYZ(0, 0, 1).CrossProduct((crv.GetEndPoint(0) - crv.GetEndPoint(1))); // Get the direction of the gridline
                }

                Reference gridRef = null;

                // Options to extract the reference geometry needed for the NewDimension method
                Options opt = new Options();
                opt.ComputeReferences = true;
                opt.IncludeNonVisibleObjects = true;
                opt.View = doc.ActiveView;
                foreach (GeometryObject obj in gr.get_Geometry(opt))
                {
                    if (obj is Line)
                    {
                        Line l = obj as Line;
                        gridRef = l.Reference;
                        refArray.Append(gridRef);   // Append to the list of all reference lines 
                    }
                }
            }

            XYZ pickPoint = uidoc.Selection.PickPoint();    // Pick a placement point for the dimension line
            Line line = Line.CreateBound(pickPoint, pickPoint + dir * 100);     // Creates the line to be used for the dimension line

            using (Transaction t = new Transaction(doc, "Make Dim"))
            {
                t.Start();
                if (!doc.IsFamilyDocument)
                {
                    doc.Create.NewDimension(
                      doc.ActiveView, line, refArray);
                }
                t.Commit();
            }

            return Result.Succeeded;

        }
    }

}

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
    public class SheetNumberingCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            Transaction tx = new Transaction(doc, "SheetNumber");
            tx.Start();
            FilteredElementCollector col = new FilteredElementCollector(doc);
            col.OfClass(typeof(ViewSheet));
            List<ViewSheet> vss = new List<ViewSheet>();
            vss = col.Cast<ViewSheet>().OrderBy(x => x.SheetNumber).ToList();
            int i = -1;
            foreach (ViewSheet vs in vss)
            {
                Parameter p1 = vs.LookupParameter("Appears In Sheet List");

                int i1 = p1.AsInteger();

                if (vs.LookupParameter("Appears In Sheet List").AsInteger() == 1)
                {
                    i++;
                    try
                    {
                        Parameter p2 = vs.LookupParameter("STT");
                        p2.Set(i);
                    }
                    catch (NullReferenceException ex)
                    {
                       
                        string sharedParamsPath = @"D:\CHO DUYET_D\TestParamester.txt";
                        string sharedParamsGroup = "Sheet";
                        string nameOfParameter = "STT";
                        ParameterType defType = ParameterType.Integer;
                        List<Category> categories = new List<Category>();
                        categories.Add(Category.GetCategory(doc, BuiltInCategory.OST_Sheets));

                        ParameterUtils.CreateSharedParamater(doc, app,sharedParamsPath,sharedParamsGroup,
                                                             nameOfParameter, defType, BuiltInParameterGroup.PG_DATA,"SttOfSheet",
                                                             categories,true,true,true);
                        
                        Parameter p2 = vs.LookupParameter("STT");

                        p2.Set(i);
                    }
                }
            }

            tx.Commit();
            return Result.Succeeded;
        }
    }
}

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
    public class SelectHowlocoreCmd : IExternalCommand
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


            // pick va lay ve element da chon
            FilterRebar filterRebar = new FilterRebar();
            Reference reference = uidoc.Selection.PickObject(ObjectType.Element, "Please select element");
            if (reference == null)
            {
                return Result.Succeeded;
            }
            if (reference != null)
            {
                Element element = doc.GetElement(reference);

                //Parameter category = element.LookupParameter("Category");
                //ElementId categoryId = category.Id;
                //string categoryName = category.AsValueString();

                //FilterRule rule1 = ParameterFilterRuleFactory.CreateEqualsRule(categoryId, categoryName, true);
                //ElementParameterFilter filterRule1 = new ElementParameterFilter(rule1);



                // lay ve parameter Tpye cua cot
                Parameter type = element.LookupParameter("DIM_WIDTH");
                ElementId elementId = type.Id;

                //string ValueType = type.AsString();
                double s = type.AsDouble();
                string ValueType = Convert.ToString(s);

                FilterRule rule2 = ParameterFilterRuleFactory.CreateEqualsRule(elementId, ValueType, true);
                ElementParameterFilter filterRule2 = new ElementParameterFilter(rule2);



                // tao collector filter

                FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);

                // tao list Filter
                IList<ElementFilter> filters = new List<ElementFilter>();
                //filters.Add(filterRule1);
                filters.Add(filterRule2);


                //LogicalOrFilter logicalOrFilter = new LogicalOrFilter(filters);


                //LogicalOrFilter orFilterFilter = new LogicalOrFilter(filters);

                LogicalAndFilter andFilter = new LogicalAndFilter(filters);

                ICollection<ElementId> elementIds = collector.WherePasses(andFilter).ToElementIds();

                uidoc.Selection.SetElementIds(elementIds);
                
            }
            return Result.Succeeded;

        }
    }
}

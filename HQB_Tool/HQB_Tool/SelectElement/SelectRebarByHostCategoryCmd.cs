
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
    public class SelectRebarByHostCategoryCmd : IExternalCommand
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
            FilterRebar_vn filterRebar = new FilterRebar_vn();
            Reference reference = uidoc.Selection.PickObject(ObjectType.Element, filterRebar, "Please select Rebar");
            if (reference == null)
            {
                return Result.Succeeded;
            }
            if (reference != null)
            {
                Element element = doc.GetElement(reference);

                // tao CategroryFilter

                ElementCategoryFilter OST_RebarFilter =
                    new ElementCategoryFilter(BuiltInCategory.OST_Rebar);



                // lay ve parameter ControlMark va ControlMark cua rebar
                Parameter HostCategory = element.LookupParameter("Host Category");
                ElementId HostCategoryId = HostCategory.Id;

                string ValueHost = HostCategory.AsValueString();


                FilterRule rule1 = ParameterFilterRuleFactory.CreateEqualsRule(HostCategoryId, ValueHost, true);
                ElementParameterFilter filterRule1 = new ElementParameterFilter(rule1);



                // tao collector filter

                FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);

                // tao list Filter
                IList<ElementFilter> filters = new List<ElementFilter>();
                filters.Add(OST_RebarFilter);
                filters.Add(filterRule1);

                LogicalAndFilter andFilter = new LogicalAndFilter(filters);

                ICollection<ElementId> elementIds = collector.WherePasses(andFilter).ToElementIds();

                uidoc.Selection.SetElementIds(elementIds);
                
            }
            return Result.Succeeded;

        }
    }
}

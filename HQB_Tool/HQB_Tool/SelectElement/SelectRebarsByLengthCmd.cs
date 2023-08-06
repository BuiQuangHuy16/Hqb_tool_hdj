
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
    public class SelectRebarsByLengthCmd : IExternalCommand
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
            string dllFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            AssemblyLoader.LoadAllRibbonAssemblies(dllFolder);


            // pick va lay ve element da chon
            FilterRebar filterRebar = new FilterRebar();
            IList<Reference> listrReferences = uidoc.Selection.PickObjects(ObjectType.Element, filterRebar);

            // tạo collector filter
            FilteredElementCollector collector =
                new FilteredElementCollector(doc, doc.ActiveView.Id);

            IList<ElementFilter> filters = new List<ElementFilter>();

            foreach (Reference r in listrReferences)
            {
                Element e = doc.GetElement(r);
                Parameter length
                    = e.LookupParameter("DIM_LENGTH");
                ElementId lengthId = length.Id; //{820171}
                string valueLength = length.AsString();

                FilterRule rule1 =
                    ParameterFilterRuleFactory.CreateEqualsRule(lengthId, valueLength, true);
                ElementParameterFilter filterRule1 =
                    new ElementParameterFilter(rule1);
                filters.Add(filterRule1);
            }
            LogicalOrFilter orFilter = new LogicalOrFilter(filters);

            ICollection<ElementId> elementIds = collector.WherePasses(orFilter).ToElementIds();

            uidoc.Selection.SetElementIds(elementIds);

            return Result.Succeeded;

        }
    }
}

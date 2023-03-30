#region Namespaces

using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace HqbTool
{
    [Transaction(TransactionMode.Manual)]
    public class CreateColumnFromAutoCadCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            string dllFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            AssemblyLoader.LoadAllRibbonAssemblies(dllFolder);


            using (TransactionGroup tranGroup = new TransactionGroup(doc))
            {
                tranGroup.Start("Create Column from File CADLink");

                CreateColumnFromAutoCadViewModel viewModel = new CreateColumnFromAutoCadViewModel(uidoc);

                CreateColumnFromAutoCadWindow window = new CreateColumnFromAutoCadWindow(viewModel);

                bool? dialog = window.ShowDialog();

                if (dialog == null || dialog == false)
                {
                    tranGroup.RollBack();
                    return Result.Cancelled;
                }
                tranGroup.Assimilate();
                
            }
            return Result.Succeeded;

        }
    }
}
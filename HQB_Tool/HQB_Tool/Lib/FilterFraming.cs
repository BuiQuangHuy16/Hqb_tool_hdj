using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace HqbTool
{
    public class FilterFraming : ISelectionFilter
    {
        public bool AllowElement(Element elem1)
        {
            int idIntegerValue2 = elem1.Category.Id.IntegerValue;
            if (idIntegerValue2 == (int)BuiltInCategory.OST_StructuralFraming)
            {
                return true;
            }

            return false;
        }


        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
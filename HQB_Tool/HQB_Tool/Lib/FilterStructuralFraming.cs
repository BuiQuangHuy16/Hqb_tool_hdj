using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace HqbTool
{
    public class FilterStructuralFraming : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            int idIntegerValue = elem.Category.Id.IntegerValue;
            if (idIntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
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
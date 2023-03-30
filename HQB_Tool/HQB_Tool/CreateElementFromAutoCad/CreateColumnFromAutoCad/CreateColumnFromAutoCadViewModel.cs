using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using ComboBox = Autodesk.Revit.UI.ComboBox;
using View = Autodesk.Revit.DB.View;


namespace HqbTool
{
    public class CreateColumnFromAutoCadViewModel : ViewModelBase

    {
        public UIDocument UiDoc;
        public Document Doc;
       

        public CreateColumnFromAutoCadViewModel(UIDocument uiDoc)
        {
            UiDoc = uiDoc;
            Doc = UiDoc.Document;

            Initialize();

        }
        
        private void Initialize()
        {
            Reference reference 
                = UiDoc.Selection.PickObject(ObjectType.Element, new ImportInstanceSelectionFilter(), "Please pick Link CAD");
            SelectedCadLink 
                = Doc.GetElement(reference) as ImportInstance;
            AllLayers 
                = CadUtils.GetAllLayer(SelectedCadLink);
            if (AllLayers.Any())
                SelecetedLayer = AllLayers[0];

            AlllFamiliesColumn 
                = new FilteredElementCollector(Doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Where(f => f.FamilyCategory.Name.Equals("Structural Columns")
                            || f.FamilyCategory.Name.Equals("Columns")).ToList();
            if (AlllFamiliesColumn.Any())
                SelecetedFamily = AlllFamiliesColumn[0];

            AllLevel = new FilteredElementCollector(Doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();
            AllLevel = AllLevel.OrderBy(lv => lv.Elevation).ToList();
            SelectedBaseLevel = AllLevel.First();
            SelectedTopLevel = AllLevel.Last();
        }
        

        internal ImportInstance SelectedCadLink;
        private double _percent;


        #region khai bao binding

        public List<string> AllLayers { get; set; }
        public string SelecetedLayer { get; set; }
        public List<Family> AlllFamiliesColumn { get; set; } = new List<Family>();
        public Family SelecetedFamily { get; set; }
        public List<Level> AllLevel { get; set; } = new List<Level>();
        public Level SelectedBaseLevel { get; set; }
        public Level SelectedTopLevel { get; set; }
        public double BaseOffSet { get; set; }
        public double TopOffSet { get; set; }

        public double Percent
        {
            get => _percent;
            set
            {
                _percent = value;
                OnPropertyChanged();
            }
        }

        #endregion khai bao binding


    }


}
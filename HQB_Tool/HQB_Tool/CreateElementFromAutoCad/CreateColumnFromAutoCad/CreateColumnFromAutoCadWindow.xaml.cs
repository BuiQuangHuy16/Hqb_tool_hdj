using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Line = Autodesk.Revit.DB.Line;
using MessageBox = System.Windows.Forms.MessageBox;

namespace HqbTool
{
    /// <summary>
    /// </summary>
    public partial class CreateColumnFromAutoCadWindow : Window
    {
        public CreateColumnFromAutoCadViewModel viewModel;
        private TransactionGroup transG;


        public CreateColumnFromAutoCadWindow(CreateColumnFromAutoCadViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
            transG = new TransactionGroup(viewModel.Doc);
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            #region Lấy về maximum những element cần chạy

            List<PlanarFace> hatchToCrateColumn
                = CadUtils.GetHatchHaveName(viewModel.SelectedCadLink, viewModel.SelecetedLayer);

            List<ColumnData> allColumnDatas = new List<ColumnData>();
            foreach (PlanarFace hatch in hatchToCrateColumn)
            {
                ColumnData columnData = new ColumnData(hatch);
                allColumnDatas.Add(columnData);
            }
            #endregion
            ProgressWindow.Maximum = allColumnDatas.Count;
            transG.Start("Run Process");

            // code here

            List<ElementId> newColumns = new List<ElementId>();
            double value = 0;
            foreach (ColumnData columnData in allColumnDatas)
            {
                if (transG.HasStarted())
                {
                    #region Stetup cho thanh ProgressBar nhảy % tiến trình"

                    value = value + 1;
                    try
                    {
                        Show();
                    }
                    catch (Exception)
                    {
                        Close();
                        if (transG.HasStarted()) transG.RollBack();
                        System.Windows.MessageBox.Show("Progress is Cancel!", "Stop Progress",
                            MessageBoxButton.OK, MessageBoxImage.Stop);
                        break;
                    }

                    viewModel.Percent
                        = value / ProgressWindow.Maximum * 100;

                    ProgressWindow.Dispatcher?.Invoke(() => ProgressWindow.Value = value,
                        DispatcherPriority.Background);
                    #endregion

                    FamilySymbol familySymbol = FamilySymbolUtils.GetFamilySymbolColumn(viewModel.SelecetedFamily,
                        columnData.CanhNgan,
                        columnData.CanhDai,
                        "b", "h");
                    if (familySymbol == null) continue;
                    using (Transaction tran = new Transaction(viewModel.Doc, "Create Column"))
                    {
                        tran.Start();
                        DeleteWarningSuper warningSuper = new DeleteWarningSuper();
                        FailureHandlingOptions failOpt = tran.GetFailureHandlingOptions();
                        failOpt.SetFailuresPreprocessor(warningSuper);
                        tran.SetFailureHandlingOptions(failOpt);

                        FamilyInstance familyInstance = viewModel.Doc.Create
                            .NewFamilyInstance(columnData.TamCot,
                                familySymbol,
                                viewModel.SelectedBaseLevel,
                                StructuralType.Column);
                        familyInstance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM)
                                    .Set(viewModel.SelectedBaseLevel.Id);
                        familyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM)
                            .Set(viewModel.SelectedTopLevel.Id);
                        familyInstance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM)
                            .Set(AlphaBIMUnitUtils.MmToFeet(viewModel.BaseOffSet));
                        familyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM)
                            .Set(AlphaBIMUnitUtils.MmToFeet(viewModel.TopOffSet));
                        Line axis = Line.CreateUnbound(columnData.TamCot, XYZ.BasisZ);
                        ElementTransformUtils.RotateElement(viewModel.Doc,
                            familyInstance.Id,
                            axis,
                            columnData.GocXoay);
                        newColumns.Add(familyInstance.Id);
                        tran.Commit();

                    }


                }
                else
                {
                    break;
                }
            }

            if (transG.HasStarted())
            {
                transG.Commit();
                DialogResult = true;

                MessageBox.Show(string.Concat("Success: ", newColumns.Count, " elements!"),
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                viewModel.UiDoc.Selection.SetElementIds(newColumns);
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (transG.HasStarted())
            {
                DialogResult = false;
                transG.RollBack();
                System.Windows.MessageBox.Show("Progress is Cancel!", "Stop Progress",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }





    }
}






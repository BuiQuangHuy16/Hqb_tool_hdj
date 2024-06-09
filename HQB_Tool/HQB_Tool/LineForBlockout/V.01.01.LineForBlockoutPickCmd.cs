
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
    public class LineForBlockoutPick : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            try
            {
                // Viết code trong này để bắt lỗi nếu xảy ra lỗi

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Application app = uiapp.Application;
                Document doc = uidoc.Document;

                // Khi chạy bằng Add-in Manager thì comment 2 dòng bên dưới để tránh lỗi
                // When running with Add-in Manager, comment the 2 lines below to avoid errors
                //string dllFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                //AssemblyLoader.LoadAllRibbonAssemblies(dllFolder);

                // pick va lay ve element da chon
                FilterStructuralFraming filterStructuralFraming = new FilterStructuralFraming();
                Reference reference = uidoc.Selection.PickObject(ObjectType.Element);
                if (reference == null)
                {
                    return Result.Succeeded;
                }
                if (reference != null)
                {

                    Element element = doc.GetElement(reference);
                    Options options = new Options();
                    GeometryElement geometryElement = element.get_Geometry(options);

                    Category c = doc.Settings.Categories.get_Item(
                        BuiltInCategory.OST_Lines);
                    CategoryNameMap subcats = c.SubCategories;

                    List<string> allLines = new List<string>();
                    

                    foreach (Category lineStyle in subcats)
                    {

                        allLines.Add(lineStyle.Name);

                    }

                    // lấy về face hiện hành trên view 
                    Face resultFace = null;
                    // Pick point nằm trong BO
                    XYZ checkPoint = uidoc.Selection.PickPoint();
                    foreach (GeometryObject geoObj in geometryElement)
                    {
                        Solid geoSolid = geoObj as Solid;
                        if (geoSolid != null)
                        {
                            // Tìm face của wall đang hiển thị trên view
                            XYZ direction = doc.ActiveView.ViewDirection;
                            foreach (Face geomFace in geoSolid.Faces)
                            {
                                if (direction.IsAlmostEqualTo(geomFace.ComputeNormal(UV.Zero), 1e-7))
                                {
                                    resultFace = geomFace;
                                }
                            }



                            if (resultFace != null)
                            {
                                // Tìm ra edge array BO (các edge array có chu vi nhỏ hơn chu vi lớn nhất)
                                List<double> totalEdgeLengths = new List<double>();
                                foreach (EdgeArray edgeLoop in resultFace.EdgeLoops)
                                {
                                    double tongDai = 0;
                                    foreach (Edge edge in edgeLoop)
                                    {
                                        double length = edge.ApproximateLength;
                                        tongDai += length;
                                    }

                                    totalEdgeLengths.Add(tongDai);
                                }

                                int vitri = 0;
                                double max = totalEdgeLengths[0];
                                for (int i = 0; i < totalEdgeLengths.Count; i++)
                                {
                                    if (totalEdgeLengths[i] > max)
                                    {
                                        max = totalEdgeLengths[i];
                                        vitri = i;
                                    }
                                }
                                List<EdgeArray> blockout = new List<EdgeArray>();


                                for (int i = 0; i < resultFace.EdgeLoops.Size; i++)
                                {
                                    if (i != vitri)
                                    {
                                        blockout.Add(resultFace.EdgeLoops.get_Item(i));
                                    }
                                }

                                //XYZ pickPoint = uidoc.Selection.PickPoint();

                                EdgeArray resultBO = null;

                                // Tìm điểm project point từ pickpoint lên mặt phẳng chứa wall face
                                var pointOnWallFace = blockout[0].get_Item(0).AsCurve().GetEndPoint(0);
                                var wallPlane = new Plane3DLib(pointOnWallFace, doc.ActiveView.ViewDirection);
                                var pickpoint2 = wallPlane.ProjectPointOnPlane(checkPoint);

                                // Tìm ra BO chứa điểm pick point
                                foreach (EdgeArray edgeArray in blockout)
                                {


                                    bool containPickPoint = true;
                                    foreach (Edge edge in edgeArray)
                                    {

                                        Line line = edge.AsCurve() as Line;

                                        XYZ lineDirection = line.Direction;
                                        XYZ crossProduct = lineDirection.CrossProduct(direction);

                                        XYZ p1 = MathLib.GetProjectPointOnCurve(line, pickpoint2);
                                        Line l3 = Line.CreateBound(p1, pickpoint2);
                                        XYZ l3Direction = l3.Direction;
                                        Line resultLine = null;
                                        l3Direction.AngleTo(crossProduct);
                                        if (!l3Direction.IsAlmostEqualTo(crossProduct, 1e-7))
                                        {
                                            containPickPoint = false;
                                        }

                                    }

                                    if (containPickPoint)
                                    {
                                        // 'edgeArray' contains pick point, do something here ...

                                        resultBO = edgeArray;

                                        List<XYZ> points = new List<XYZ>();
                                        foreach (Edge edge in edgeArray)
                                        {
                                            Curve asCurve = edge.AsCurve();
                                            XYZ endPoint = asCurve.GetEndPoint(0);
                                            points.Add(endPoint);
                                        }

                                        Line l1 = Line.CreateBound(points[0], points[2]);
                                        Line l2 = Line.CreateBound(points[1], points[3]);

                                        using (Transaction tran = new Transaction(doc))
                                        {
                                            tran.Start("Line B.O.");

                                            DetailCurve curve1 = doc.Create.NewDetailCurve(doc.ActiveView, l1);
                                            GraphicsStyle curve1LineStyle = curve1.LineStyle as GraphicsStyle;
                                            curve1LineStyle.GraphicsStyleCategory.Name

                                            DetailCurve curve2 = doc.Create.NewDetailCurve(doc.ActiveView, l2);

                                            tran.Commit();
                                        }
                                    }

                                }

                            }


                        }
                    }

                }

            }
            catch (Exception e)
            {
                // Bị lỗi
                TaskDialog.Show("Error", e.Message + '\n' + e.StackTrace);
            }

            return Result.Succeeded;
        }

    }
}

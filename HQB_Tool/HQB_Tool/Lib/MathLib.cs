using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace HqbTool
{
    public class MathLib
    {
        /// <summary>
        ///     Compare two number
        /// </summary>
        /// <param name="d1">first member</param>
        /// <param name="d2">second member</param>
        /// <param name="epsilon">tolerance</param>
        /// <returns>return true or fault</returns>
        public static bool IsEqual(double d1, double d2, double epsilon)
        {
            bool isEqual = false;
            double d = Math.Abs(d1 - d2);
            if (d < epsilon) isEqual = true;
            return isEqual;
        }

        /// <summary>
        ///     function find the intersect point between two curve, in the same plane: curve1, curve2
        /// </summary>
        /// <param name="curve1"> curve1</param>
        /// <param name="curve2">rebar to change</param>
        public static XYZ GetIntersectPoint(Curve curve1, Curve curve2)
        {
            XYZ intersectPoint;
            XYZ p10 = curve1.GetEndPoint(0);
            XYZ p11 = curve1.GetEndPoint(1);
            XYZ p20 = curve2.GetEndPoint(0);
            XYZ p21 = curve2.GetEndPoint(1);
            // Project point
            XYZ v110 = p11 - p10;
            XYZ u210 = p20 - p10;
            double cos1 = (v110.DotProduct(u210)) / (v110.GetLength() * u210.GetLength());
            XYZ projectPoint = p10 + (u210.GetLength() * cos1) * v110.Normalize();
            // test projectPoint
            XYZ vh = projectPoint - p20;
            double d = vh.DotProduct(v110);
            d = d / (vh.GetLength() * v110.GetLength());
            // intersection point
            double h = vh.GetLength(); // chieu cao ( do dai doan thang tu P20 vuong goc voi curve1)
            XYZ v210 = p21 - p20;
            double cos = ((vh.DotProduct(v210))) / (vh.GetLength() * v210.GetLength());
            intersectPoint = p20 + (h / cos) * (v210.Normalize());
            // test intersection 
            XYZ v1 = intersectPoint - p10;
            XYZ v2 = v1.CrossProduct(v110) / (v1.GetLength() * v110.GetLength());
            double v = v2.GetLength();
            return intersectPoint;
        }

        /// <summary>
        ///     Get project point on the curve of given point
        /// </summary>
        /// <param name="curve"> curve</param>
        /// <param name="point"> point </param>
        public static XYZ GetProjectPointOnCurve(Curve curve, XYZ point)
        {
            XYZ projectPoint = new XYZ();
            XYZ p10 = curve.GetEndPoint(0);
            XYZ p11 = curve.GetEndPoint(1);
            // Project point
            XYZ v = p11 - p10;
            XYZ u = point - p10;
            if (u.GetLength() < 0.00001) return p10;
            if (u.GetLength() > 0.00001)
            {
                double cos1 = (v.DotProduct(u)) / (v.GetLength() * u.GetLength());
                projectPoint = p10 + (u.GetLength() * cos1) * v.Normalize();
            }

            return projectPoint;
        }

        /// <summary>
        ///     Get distance from point to the curve
        /// </summary>
        /// <param name="point"> point </param>
        /// <param name="curve"> curve</param>
        public static double DistanceFromPointToCurve(XYZ point, Curve curve)
        {
            XYZ p = GetProjectPointOnCurve(curve, point);
            XYZ v = point - p;
            double d = v.GetLength();
            return d;
        }

        /// <summary>
        ///     Check a point is inside a curve or not
        /// </summary>
        /// <param name="curve"> curve </param>
        /// <param name="point"> point to check </param>
        /// <param name="epsilon">Tolerant</param>
        public static bool IsPointInsideCurve(Curve curve, XYZ point, double epsilon)
        {
            bool flag = false;
            XYZ p0 = curve.GetEndPoint(0);
            XYZ p1 = curve.GetEndPoint(1);
            XYZ v0 = point - p0;
            XYZ v1 = point - p1;
            if (v0.GetLength() < epsilon || v1.GetLength() < epsilon) return true;
            if (IsEqual(curve.Length, v0.GetLength() + v1.GetLength(), epsilon)) flag = true;
            return flag;
        }

        /// <summary>
        ///     Check a two point is in the same side of a curve or not
        /// </summary>
        /// <param name="curve"> curve </param>
        /// <param name="firstPoint"> firstPoint to check </param>
        /// <param name="secondPoint"> secondPoint to check </param>
        public static bool IsSameSide(Curve curve, XYZ firstPoint, XYZ secondPoint)
        {
            bool flag = false;
            XYZ p1 = GetProjectPointOnCurve(curve, firstPoint);
            XYZ p2 = GetProjectPointOnCurve(curve, secondPoint);
            XYZ v1 = p1 - firstPoint;
            XYZ v2 = p2 - secondPoint;
            double d = v1.DotProduct(v2);
            if (d < 0) flag = false;
            if (d > 0) flag = true;
            return flag;
        }

        public static double RoundOff(double d, double r)
        {
            int c = (int)(d / r);
            double v = c * r;
            return v;
        }
    }

    public class Plane3DLib
    {
        public Plane3DLib()
        {

        }

        /// <summary>
        ///     Construction function
        /// </summary>
        /// <param name="origin"> origin of plane </param>
        /// <param name="normal"> normal of plane </param>
        public Plane3DLib(XYZ origin, XYZ normal)
        {
            Origin = origin;
            Normal = normal.Normalize();
        }

        /// <summary>
        ///     Construction function
        /// </summary>
        /// <param name="curve1"> curve1, plane will contains this curve </param>
        /// <param name="curve2"> curve2</param>
        public Plane3DLib(Curve curve1, Curve curve2)
        {
            XYZ p10 = curve1.GetEndPoint(0);
            XYZ p11 = curve1.GetEndPoint(1);
            XYZ p20 = curve2.GetEndPoint(0);
            XYZ p21 = curve2.GetEndPoint(1);
            XYZ v10 = p11 - p10;
            XYZ u10 = p21 - p20;
            Origin = p10;
            Normal = (v10.CrossProduct(u10)).Normalize();
        }

        public XYZ Origin { get; set; }
        public XYZ Normal { get; set; }

        /// <summary>
        ///     Get distance from point to plane
        /// </summary>
        /// <param name="point"> point </param>
        public double DistanceFromPointToPlane(XYZ point)
        {
            XYZ vector = point - Origin;
            double distance = Math.Abs(vector.DotProduct(Normal) / Normal.GetLength());
            return distance;
        }

        /// <summary>
        ///     Get projection from a point to plane
        /// </summary>
        /// <param name="point"> point </param>
        public XYZ ProjectPointOnPlane(XYZ point)
        {
            XYZ p = null;
            double d = DistanceFromPointToPlane(point);
            if (d < double.Epsilon) return point;
            XYZ p1 = point + d * Normal;
            XYZ p2 = point - d * Normal;
            XYZ v1 = p1 - Origin;
            XYZ v2 = p2 - Origin;
            double d1 = 0;
            double d2 = 0;
            if (v1.GetLength() < 0.0001)
            {
                p = p1;
            }
            else
            {
                d1 = v1.DotProduct(Normal) / v1.GetLength();
            }

            if (v2.GetLength() < 0.0001)
            {
                p = p2;
            }
            else
            {
                d2 = v2.DotProduct(Normal) / v2.GetLength();
            }

            if (Math.Abs(d1) < 0.00001) p = p1;
            if (Math.Abs(d2) < 0.00001) p = p2;
            return p;
        }

        public XYZ ProjectPointOnPlane(XYZ point, XYZ vector)
        {
            XYZ p = XYZ.Zero;
            double distance = DistanceFromPointToPlane(point);
            double cos = vector.DotProduct(Normal) / vector.GetLength();
            if (cos.Equals(0)) return p;
            XYZ p1 = point + (distance / cos) * (vector.Normalize());
            XYZ p2 = point - (distance / cos) * (vector.Normalize());
            double d1 = DistanceFromPointToPlane(p1);
            double d2 = DistanceFromPointToPlane(p2);
            if (d1 < d2) p = p1;
            if (d1 > d2) p = p2;
            return p;
        }

        /// <summary>
        ///     Get projection from a curve to plane
        /// </summary>
        /// <param name="curve"> curve </param>
        public Curve ProjectCurveOnPlane(Curve curve)
        {
            Curve projectCurve = null;
            XYZ p0 = curve.GetEndPoint(0);
            XYZ p1 = curve.GetEndPoint(1);
            XYZ pr0 = ProjectPointOnPlane(p0);
            XYZ pr1 = ProjectPointOnPlane(p1);
            Line line = Line.CreateBound(pr0, pr1);
            projectCurve = line as Curve;
            return projectCurve;
        }

        public XYZ ProjectVectorOnPlane(XYZ vector)
        {
            XYZ v = new XYZ();
            XYZ p0 = new XYZ();
            XYZ p1 = p0 + vector;
            XYZ pr0 = ProjectPointOnPlane(p0);
            XYZ pr1 = ProjectPointOnPlane(p1);
            v = pr1 - pr0;
            return v;
        }

        /// <summary>
        ///     Check a point is on plane or not
        /// </summary>
        /// <param name="point"> point to check </param>
        public bool IsPointOnPlane(XYZ point)
        {
            bool flag = false;
            double d = DistanceFromPointToPlane(point);
            if (d < 0.001) flag = true;
            return flag;
        }

        // End class
    }
}

#region Namespaces
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows.Forms;

#endregion

namespace HqbTool
{
    /// <summary>
    /// Tham khảo:
    /// 1. http://bit.ly/2l3Jsf6
    /// 2. https://autode.sk/2mtSaUb
    /// </summary>
    public class RibbonApp : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication app)
        {
            string versionName = app.ControlledApplication.VersionName;
            string versionNumber = app.ControlledApplication.VersionNumber;
            string versionBuild = app.ControlledApplication.VersionBuild;
            string subVersionNumber = app.ControlledApplication.SubVersionNumber;
            //MessageBox.Show("VersionName: " + versionName + "\n" +
            //"VersionNumber: " + versionNumber + "\n" +
            //"VersionBuild: " + versionBuild + "\n" +
            //"SubVersionNumber: " + subVersionNumber + "\n");


            CreateRibbonPanel(app);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }

        private void CreateRibbonPanel(UIControlledApplication a)
        {
            AlphaBIMConstraint alphaConstraint = new AlphaBIMConstraint();
            RibbonUtils ribbonUtils = new RibbonUtils(a.ControlledApplication);
           
            // Tạo Ribbon tab
            string ribbonName = "HQB_Tool";
            a.CreateRibbonTab(ribbonName);

            // Tạo Ribbon Panel
            string panelName = "SelectElement";
            RibbonPanel panel1 = a.CreateRibbonPanel(ribbonName, panelName);
            // Add button vào Panel
            #region Selecement

            SplitButton splitButton1 = ribbonUtils.CreateSplitButton(panel1,
                "SelectRebarby",
                "Split\r\nButton",
                "SplitButton sample",
                "HQB.ico");

            PushButtonData pushButtonData1
                = ribbonUtils.CreatePushButtonData("SelectRebarCmd",
                "Select Rebar\nBy ControlMark", "HQB_Tool.dll",
                "HqbTool.SelectRebarCmd",
                "HQB.ico",
                "Pick Rebar");
            splitButton1.AddPushButton(pushButtonData1);

            PushButtonData pushButtonData2
                = ribbonUtils.CreatePushButtonData("SelectRebarsCmd",
                    "Select Rebars\nBy ControlMark", "HQB_Tool.dll",
                    "HqbTool.SelectRebarsCmd",
                    "HQB.ico",
                    "Pick Rebars");
            splitButton1.AddPushButton(pushButtonData2);

            PushButtonData pushButtonData7
                = ribbonUtils.CreatePushButtonData("SelectRebarByLengthCmd",
                    "Select Rebars\nBy Length", "HQB_Tool.dll",
                    "HqbTool.SelectRebarByLengthCmd",
                    "HQB.ico",
                    "Pick Rebar");
            splitButton1.AddPushButton(pushButtonData7);

            PushButtonData pushButtonData8
                = ribbonUtils.CreatePushButtonData("SelectRebarByHostCategoryCmd",
                    "Select Rebars\nBy ByHostCategory", "HQB_Tool.dll",
                    "HqbTool.SelectRebarByHostCategoryCmd",
                    "HQB.ico",
                    "Pick Rebar");
            splitButton1.AddPushButton(pushButtonData8);



            SplitButton splitButton2 = ribbonUtils.CreateSplitButton(panel1,
                "SelectEmbed",
                "Split\r\nButton",
                "SplitButton sample",
                "HQB.ico");

            PushButtonData pushButtonData3
                = ribbonUtils.CreatePushButtonData("SelectEbedsCmd",
                    "Select Embed\nBy Type", "HQB_Tool.dll",
                    "HqbTool.SelectEbedsCmd",
                    "HQB.ico",
                    "Pick Element");
            splitButton2.AddPushButton(pushButtonData3);

            PushButtonData pushButtonData6
                = ribbonUtils.CreatePushButtonData("SelectElementByTypeCmd",
                    "Select By Type", "HQB_Tool.dll",
                    "HqbTool.SelectElementByTypeCmd",
                    "HQB.ico",
                    "Pick Element");
            splitButton2.AddPushButton(pushButtonData6);

            panelName = "ViewSheet";
            RibbonPanel panel2 = a.CreateRibbonPanel(ribbonName, panelName);

            SplitButton splitButton3 = ribbonUtils.CreateSplitButton(panel2,
                "Numbering",
                "Split\r\nButton",
                "SplitButton sample",
                "HQB.ico");

            PushButtonData pushButtonData4
                = ribbonUtils.CreatePushButtonData("SheetNumberingCmd",
                    "STT-Sheet", "HQB_Tool.dll",
                    "HqbTool.SheetNumberingCmd",
                    "HQB.ico",
                    "Go to Sheet");
            splitButton3.AddPushButton(pushButtonData4);

            #endregion Selecement


            panelName = "Create Element From Cad";
            RibbonPanel panel3 = a.CreateRibbonPanel(ribbonName, panelName);

            SplitButton splitButton4 = ribbonUtils.CreateSplitButton(panel3,
                "CreateElemnt",
                "Split\r\nButton",
                "SplitButton sample",
                "HQB.ico");

            PushButtonData pushButtonData5
                = ribbonUtils.CreatePushButtonData("CreateColumnFromAutoCadCmd",
                    "CreateColumnFromCad", "HQB_Tool.dll",
                    "HqbTool.CreateColumnFromAutoCadCmd",
                    "HQB.ico",
                    "PickFileLink");
            splitButton4.AddPushButton(pushButtonData5);

            return;
            
          
          

        }
    }
}
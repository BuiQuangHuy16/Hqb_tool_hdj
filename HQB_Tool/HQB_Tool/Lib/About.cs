using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace HqbTool
{
   public class About   

    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }

        public About(string name, string version, string author, string description)
        {
            Name = name;
            Version = version;
            Author = author;
            Description = description;
        }

        public string GetAboutInformation()
        {
            string about = "Name: " + Name + Environment.NewLine +
                           "Version: " + Version + Environment.NewLine +
                           "Author: " + Author + Environment.NewLine +
                           "Description: " + Description + Environment.NewLine;
            return about;
        }

    }

}

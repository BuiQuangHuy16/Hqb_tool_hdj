﻿#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Binding = Autodesk.Revit.DB.Binding;
using MessageBox = System.Windows.MessageBox;
using String = System.String;

#endregion

namespace HqbTool
{
    public static class ParameterUtils
    {
        /// <summary>
        /// Lấy về giá trị của parameter p
        /// </summary>
        /// <param name="p">Parameter cần lấy giá trị</param>
        /// <param name="asValueString">asValueString = true: Lấy giá trị parameter được hiện ra màn hình Revit</param>
        /// <returns></returns>
        public static string GetValue(this Parameter p,
            bool asValueString = false)
        {
            if (p == null) return string.Empty;
            if (asValueString && p.StorageType != StorageType.String)
                return p.AsValueString();

            switch (p.StorageType)
            {
                case StorageType.Double:
                    return p.AsDouble().ToString();

                case StorageType.ElementId:
                    try
                    {
                        return p.AsElementId().IntegerValue.ToString();
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.ToString());
                        return string.Empty;
                    }

                case StorageType.Integer:
                    return p.AsInteger().ToString();

                case StorageType.String:
                    return p.AsString();

                case StorageType.None:
                    return p.AsValueString();
            }
            return String.Empty;
        }

        /// <summary>
        /// Gán giá trị cho parameter p
        /// </summary>
        /// <param name="p">Parameter cần xét giá trị.</param>
        /// <param name="value">Giá trị cần set cho parameter p.</param>
        public static void SetValue(this Parameter p, object value)
        {
            // Cannot edit readonly
            if (p.IsReadOnly || value == null) return;

            switch (p.StorageType)
            {
                case StorageType.Double:
                    p.Set(Convert.ToDouble(value));
                    break;

                case StorageType.ElementId:
                    if (value is string)
                    {
                        try
                        {
                            int id = Convert.ToInt32(value);
                            p.Set(new ElementId(id));
                            break;
                        }
                        catch (Exception e)
                        {
                            //MessageBox.Show(e.ToString());
                        }
                    }
                    p.Set(value as ElementId);
                    break;

                case StorageType.Integer:
                    p.Set(Convert.ToInt32(value));
                    break;

                case StorageType.String:
                    p.Set(value.ToString());
                    break;

                case StorageType.None:
                    p.SetValueString(value as string);
                    break;
            }
        }

        /// <summary>
        /// Lấy về tất cả parameter gắn vào đối tượng el
        /// </summary>
        /// <param name="el">Element cần lấy P</param>
        /// <param name="isIncludeTypePara">Có lấy luôn Type Parameter hay không?</param>
        /// <returns></returns>
        public static List<string> GetAllParameters(Element el,
            bool isIncludeTypePara = true)
        {
            List<string> allParameters = new List<string>();
            allParameters.Add("ElementId");

            foreach (Parameter p in el.Parameters)
            {
                allParameters.Add(p?.Definition?.Name);
            }

            if (isIncludeTypePara)
            {
                Element elementType = el.Document.GetElement(el.GetTypeId());

                if (elementType != null)
                    foreach (Parameter p in elementType.Parameters)
                        allParameters.Add(p.Definition.Name);
            }

            return allParameters;
        }

        public static List<string> GetAllParameter_test(Element el)
        {
            List<string> allAllParameter = new List<string>();

            foreach (Parameter p in el.Parameters)
            {
                allAllParameter.Add(p.Definition.Name);
            }

            return allAllParameter;
        }

        /// <summary>
        /// Lấy về tất cả parameter có thể gán giá trị của el
        /// </summary>
        /// <param name="el">Đối tượng cần lấy parameter</param>
        /// <returns></returns>
        public static List<string> GetAllParametersEditable(Element el)
        {
            List<string> allParameters = new List<string>();
            foreach (Parameter p in el.Parameters)
            {
                if (!p.IsReadOnly)
                    allParameters.Add(p?.Definition?.Name);
            }
            return allParameters;
        }

        #region Shared Parameter
        /// <summary>
        /// Tạo mơi sharedParameter
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="app"></param>
        /// <param name="sharedParamsPath">Đường dẫn tới ổ cứng lưu file .txt</param>
        /// <param name="sharedParamsGroup">Group của shrareParameter</param>
        /// <param name="nameOfParameter">Tên của Parameter</param>
        /// <param name="defType">Type của Parameter</param>
        /// <param name="displayGroup"></param>
        /// <param name="description"></param>
        /// <param name="categories"></param>
        /// <param name="isInstanceParameter"></param>
        /// <param name="visible"></param>
        /// <param name="userModifiable"></param>
        public static void CreateSharedParamater(Document doc, Application app,
            string sharedParamsPath, string sharedParamsGroup,
            string nameOfParameter, ParameterType defType,
            BuiltInParameterGroup displayGroup, string description,
            List<Category> categories, bool isInstanceParameter = true,
            bool visible = true, bool userModifiable = true)
        {
            DefinitionFile sharedParameterFile = GetSharedParamsFile(app, sharedParamsPath);
            if (sharedParameterFile == null)
            {
                MessageBox.Show("Error to getting the shared parameter file.");
                return;
            }

            DefinitionGroup group
                = GetOrCreateSharedParamsGroup(sharedParameterFile, sharedParamsGroup);

            if (group == null)
            {
                MessageBox.Show("Error to getting the shared parameter group.");
                return;
            }

            Definition definition = GetOrCreateSharedParamsDefinition(group, nameOfParameter,
                defType, description, visible, userModifiable);

            if (isInstanceParameter)
            {
                CreateInstanceBindingParameter(doc, categories, definition, displayGroup);
            }
            else
            {
                CreateTypeBindingParameter(doc, categories, definition, displayGroup);
            }
        }

        private static DefinitionFile GetSharedParamsFile(Application app,
            string sharedParamsPath)
        {
            if (string.IsNullOrEmpty(sharedParamsPath))
            {
                MessageBox.Show("No shared params file be selected");
                return null;
            }

            app.SharedParametersFilename = sharedParamsPath;

            // Lấy về file Shared Parameter
            DefinitionFile sharedParametersFile;
            try
            {
                sharedParametersFile = app.OpenSharedParameterFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't open shared params file:" + ex.Message, "Error");
                sharedParametersFile = null;
            }
            return sharedParametersFile;
        }

        private static DefinitionGroup GetOrCreateSharedParamsGroup(DefinitionFile sharedParametersFile, string groupName)
        {
            DefinitionGroup g = sharedParametersFile.Groups.get_Item(groupName);
            if (null == g)
            {
                try
                {
                    g = sharedParametersFile.Groups.Create(groupName);
                }
                catch (Exception)
                {
                    g = null;
                }
            }
            return g;
        }

        private static Definition GetOrCreateSharedParamsDefinition(DefinitionGroup defGroup,
            string nameOfParameter, ParameterType defType, string description,
            bool visible = true, bool userModifiable = true)
        {
            Definition definition = defGroup.Definitions.get_Item(nameOfParameter);
            if (null == definition)
            {
                try
                {
                    ExternalDefinitionCreationOptions opt
                        = new ExternalDefinitionCreationOptions(nameOfParameter, defType);

                    // True if the parameter is visible to the user, 
                    // false if it is hidden and accessible only via the API. The default is true. 
                    opt.Visible = visible;
                    opt.UserModifiable = userModifiable;
                    opt.Description = description;
                    definition = defGroup.Definitions.Create(opt);
                }
                catch (Exception)
                {
                    definition = null;
                }
            }

            return definition;
        }

        private static Guid SharedParamGUID(Application app, string defGroup, string defName)
        {
            Guid guid = Guid.Empty;
            try
            {
                DefinitionFile file = app.OpenSharedParameterFile();
                DefinitionGroup group = file.Groups.get_Item(defGroup);
                Definition definition = group.Definitions.get_Item(defName);
                ExternalDefinition externalDefinition = definition as ExternalDefinition;
                guid = externalDefinition.GUID;
            }
            catch (Exception)
            {
            }
            return guid;
        }

        private static void CreateInstanceBindingParameter(Document doc, List<Category> categories,
            Definition paramDef,
            BuiltInParameterGroup displayGroup)
        {
            // Create the category set for binding and add the category
            CategorySet catSet = doc.Application.Create.NewCategorySet();
            foreach (Category cat in categories)
            {
                if (!cat.AllowsBoundParameters) continue;
                catSet.Insert(cat);
            }

            using (SubTransaction trans = new SubTransaction(doc))
            {
                trans.Start();
                // Bind the param
                try
                {
                    Binding binding = doc.Application.Create.NewInstanceBinding(catSet);
                    doc.ParameterBindings.Insert(paramDef, binding, displayGroup);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    trans.RollBack();
                }
            }
        }

        private static void CreateTypeBindingParameter(Document doc, List<Category> categories,
            Definition paramDef,
            BuiltInParameterGroup displayGroup)
        {
            // Create the category set for binding and add the category
            CategorySet catSet = doc.Application.Create.NewCategorySet();
            foreach (Category cat in categories)
            {
                if (!cat.AllowsBoundParameters) continue;
                catSet.Insert(cat);
            }

            using (SubTransaction trans = new SubTransaction(doc))
            {
                trans.Start();
                // Bind the param
                try
                {
                    Binding binding = doc.Application.Create.NewTypeBinding(catSet);
                    // We could check if already bound, but looks like Insert will just ignore it in such case
                    doc.ParameterBindings.Insert(paramDef, binding, displayGroup);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    trans.RollBack();
                }
            }
        }

        #endregion Shared Parameter
    }
}
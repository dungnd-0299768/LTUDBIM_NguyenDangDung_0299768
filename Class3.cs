using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class Class3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            IList<Element> wallList = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            StringBuilder sb = new StringBuilder();
            foreach (Element wall in wallList) //for each wall
            {
                sb.AppendLine(wall.Name + "::" + wall.Id);
                foreach (Parameter para in wall.Parameters) //for each parameter
                {
                    sb.Append(this.getParameterInfomation(para, doc, ":")); //print parameter information
                    sb.AppendLine();
                }
                sb.AppendLine();
            }
            MessageBox.Show(sb.ToString());
            return Result.Succeeded;
        }
        private string getParameterInfomation(Parameter para, Document doc, string separator)
        {
            string defName = para.Definition.Name;
            string defValue = string.Empty;

            switch (para.StorageType)
            {
                case StorageType.Double:
                    defValue = para.AsValueString();
                    break;
                case StorageType.ElementId:
                    ElementId id = para.AsElementId();
                    if (id.Value >= 0)
                        defValue = doc.GetElement(id)?.Name;
                    else
                        defValue = id.Value.ToString();
                    break;
                case StorageType.Integer:
                    if (SpecTypeId.Boolean.YesNo == para.Definition.GetDataType())
                        defValue = para.AsInteger() == 0 ? "False" : "True";
                    else
                        defValue = para.AsInteger().ToString();
                    break;
                case StorageType.String:
                    defValue = para.AsString();
                    break;
                default:
                    defValue = "Unexposed parameter.";
                    break;
            }

            return defName + separator + defValue;
        }
    }
}

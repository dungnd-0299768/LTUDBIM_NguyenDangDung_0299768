using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

using TaskDialog = Autodesk.Revit.UI.TaskDialog;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class Class1 : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIDocument uidoc = commandData.Application.ActiveUIDocument;
        Document document = uidoc.Document;

        ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();
        TaskDialog.Show("Revit", "Số lượng đối tượng được chọn: " + selectedIds.Count.ToString());

        ICollection<ElementId> selectedWallIds = new List<ElementId>();
        ICollection<ElementId> SelectedFloor = new List<ElementId>();
        ICollection<ElementId> SelectedColumn = new List<ElementId>();

        // Lấy toàn bộ cột kết cấu trong dự án 
        FilteredElementCollector collector = new FilteredElementCollector(document);
        ICollection<Element> elementsColumns = collector.OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_StructuralColumns).ToElements();

        
        foreach (ElementId id in selectedIds)
        {
            Element elements1 = uidoc.Document.GetElement(id);

            //  Kiểm tra nếu là Tường
            if (elements1 is Wall)
            {
                selectedWallIds.Add(id);
            }
            //  Kiểm tra nếu là Sàn
            else if (elements1 is Floor)
            {
                SelectedFloor.Add(id);
            }
            // Kiểm tra nếu là Cột kết cấu
            else if (elements1 is FamilyInstance && elements1.Category != null &&
                     elements1.Category.Id.Value == (long)BuiltInCategory.OST_StructuralColumns)
            {
                SelectedColumn.Add(id);
            }
        }

        // Giữ chọn các đối tượng Tường (hoặc bạn có thể thay bằng SelectedFloor / SelectedColumn tùy ý)
        uidoc.Selection.SetElementIds(selectedWallIds);

        string thongBao = $"Kết quả lọc trong vùng chọn:\n" +
                          $"- Tường: {selectedWallIds.Count}\n" +
                          $"- Sàn: {SelectedFloor.Count}\n" +
                          $"- Cột: {SelectedColumn.Count}";

        TaskDialog.Show("Revit", thongBao);

        return Result.Succeeded;
    }
}
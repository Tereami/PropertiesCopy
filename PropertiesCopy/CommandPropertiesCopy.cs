using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace PropertiesCopy
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CommandPropertiesCopy : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            Element firstElem;

            List<ElementId> selIds = sel.GetElementIds().ToList();
            if (selIds.Count > 0)
            {
                firstElem = doc.GetElement(selIds.First());
            }
            else
            {
                Reference r1;
                try
                {
                    r1 = sel.PickObject(ObjectType.Element, "Выберите элемент, с которого нужно скопировать свойства");
                }
                catch
                {
                    return Result.Cancelled;
                }

                firstElem = doc.GetElement(r1.ElementId);
            }

            if (firstElem == null)
            {
                message += "Что-то не получилось. Сохраняйте спокойствие и порядок!";
                return Result.Failed;
            }

            ParameterMap parameters = firstElem.ParametersMap;

            while (true)
            {
                try
                {
                    SelectionFilter selFilter = new SelectionFilter(firstElem);
                    Reference r = sel.PickObject(ObjectType.Element, selFilter, "Выберите элементы для копирования свойств");
                    if (r == null) continue;
                    ElementId curId = r.ElementId;
                    if (curId == null || curId == ElementId.InvalidElementId) continue;
                    Element curElem = doc.GetElement(curId);
                    if (curElem == null) continue;

                    try
                    {
                        ElementId firstTypeId = firstElem.GetTypeId();
                        ElementId curTypeId = curElem.GetTypeId();
                        if (firstTypeId != curTypeId)
                        {
                            using (Transaction t1 = new Transaction(doc))
                            {
                                t1.Start("Назначение типа");

                                curElem.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).Set(firstTypeId);

                                t1.Commit();
                            }
                        }
                    }
                    catch { }


                    using (Transaction t = new Transaction(doc))
                    {
                        t.Start("Копирование свойств");

                        foreach (Parameter param in parameters)
                        {
                            try
                            {
                                Parameter curParam = curElem.get_Parameter(param.Definition);

                                switch (param.StorageType)
                                {
                                    case StorageType.None:
                                        break;
                                    case StorageType.Integer:
                                        curParam.Set(param.AsInteger());
                                        break;
                                    case StorageType.Double:
                                        curParam.Set(param.AsDouble());
                                        break;
                                    case StorageType.String:
                                        curParam.Set(param.AsString());
                                        break;
                                    case StorageType.ElementId:
                                        curParam.Set(param.AsElementId());
                                        break;
                                    default:
                                        break;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        t.Commit();
                    }
                }
                catch
                {
                    return Result.Succeeded;
                }
            }
        }
    }
}

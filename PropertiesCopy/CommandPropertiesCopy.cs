#region License
/*Данный код опубликован под лицензией Creative Commons Attribution-NonСommercial-ShareAlike.
Разрешено использовать, распространять, изменять и брать данный код за основу для производных 
в некоммерческих целях, при условии указания авторства и если производные лицензируются на тех же условиях.
Код поставляется "как есть". Автор не несет ответственности за возможные последствия использования.
Зуев Александр, 2021, все права защищены.
This code is listed under the Creative Commons Attribution-NonСommercial-ShareAlike license.
You may use, redistribute, remix, tweak, and build upon this work non-commercially,
as long as you credit the author by linking back and license your new creations under the same terms.
This code is provided 'as is'. Author disclaims any implied warranty.
Zuev Aleksandr, 2021, all rigths reserved.*/
#endregion
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace PropertiesCopy
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CommandPropertiesCopy : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new RbsLogger.Logger("PropertiesCopy"));

            Document doc = commandData.Application.ActiveUIDocument.Document;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            Element firstElem;

            List<ElementId> selIds = sel.GetElementIds().ToList();
            Debug.WriteLine("Selected elements: " + selIds.Count.ToString());
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
                Debug.WriteLine($"First elem id: {firstElem.Id}");
            }

            if (firstElem == null)
            {
                message += "Что-то не получилось. Сохраняйте спокойствие и порядок!";
                Debug.WriteLine("First elem is null");
                return Result.Failed;
            }

            ParameterMap parameters = firstElem.ParametersMap;
            Debug.WriteLine("Parameters found: " + parameters.Size.ToString());

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
                    Debug.WriteLine($"Cur element id: {curId}");

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
                                Debug.WriteLine("Type of element is changed");
                            }
                        }
                    }
                    catch { }


                    using (Transaction t = new Transaction(doc))
                    {
                        t.Start("Копирование свойств");

                        foreach (Parameter param in parameters)
                        {
                            if (param == null) continue;
                            if (!param.HasValue) continue;
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
                                Debug.WriteLine("Param value is written: " + curParam.Definition.Name + " = " + curParam.AsValueString());
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                continue;
                            }
                        }
                        t.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return Result.Succeeded;
                }
            }
        }
    }
}

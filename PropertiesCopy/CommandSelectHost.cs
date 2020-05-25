using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Structure;

namespace PropertiesCopy
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CommandSelectHost : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            List<ElementId> selIds = sel.GetElementIds().ToList();
            if(selIds.Count != 1)
            {
                message = "Выберите элемент, для которого нужно найти основу.";
                return Result.Failed;
            }
            Element selElem = doc.GetElement(selIds.First());
            ElementId hostId = null;


            if(selElem is AreaReinforcement)
            {
                AreaReinforcement el = selElem as AreaReinforcement;
                hostId = el.GetHostId();
            }
            if (selElem is PathReinforcement)
            {
                PathReinforcement el = selElem as PathReinforcement;
                hostId = el.GetHostId();
            }
            if (selElem is Rebar)
            {
                Rebar el = selElem as Rebar;
                hostId = el.GetHostId();
            }
            if (selElem is RebarInSystem)
            {
                RebarInSystem el = selElem as RebarInSystem;
                hostId = el.SystemId;
            }

            if (selElem is FamilyInstance)
            {
                FamilyInstance el = selElem as FamilyInstance;
                Element host = el.Host;
                if (host != null)
                {
                    hostId = host.Id;
                }
                else
                {
                    Element parentFamily = el.SuperComponent;
                    if (parentFamily != null)
                    {
                        hostId = parentFamily.Id;
                    }
                }
            }


            if (hostId == null)
            {
                message = "Не удалось получить родительский элемент.";
                return Result.Failed;
            }
            else
            {
                sel.SetElementIds(new List<ElementId> { hostId });
                return Result.Succeeded;
            }
        }
    }
}

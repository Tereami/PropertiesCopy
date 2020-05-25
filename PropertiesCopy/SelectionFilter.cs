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
    public class SelectionFilter : ISelectionFilter
    {
        Element firstElem;

        public SelectionFilter(Element elem)
        {
            firstElem = elem;
        }

        bool ISelectionFilter.AllowElement(Element elem)
        {
            Type firstType = firstElem.GetType();
            Type curType = elem.GetType();
            bool check = firstType.Equals(curType);
            return check;
        }

        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}

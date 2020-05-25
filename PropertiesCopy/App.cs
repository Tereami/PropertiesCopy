using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace PropertiesCopy
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class App : IExternalApplication
    {
        public static string assemblyPath = "";

        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            string tabName = "Weandrevit";
            try { application.CreateRibbonTab(tabName); } catch { }

            RibbonPanel panel1 = application.CreateRibbonPanel(tabName, "Параметры");
            PushButton btn = panel1.AddItem(new PushButtonData(
                "btnCopyProperties",
                "Копирование\nсвойств",
                assemblyPath,
                "PropertiesCopy.CommandPropertiesCopy")
                ) as PushButton;
            //btn.LargeImage = PngImageSource("SchedulesTable.Resources.Schedules.png");
            //btn.Image = PngImageSource("SchedulesTable.Resources.SchedulesSmall.png");
            btn.ToolTip = "Супер-копирование свойств! Копирует все параметры, в том числе марку, высоту стен, параметры армирования и так далее";
            return Result.Succeeded;
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

    }
}

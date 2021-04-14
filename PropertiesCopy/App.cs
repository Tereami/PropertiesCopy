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

using Autodesk.Revit.UI;

namespace PropertiesCopy
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class App : IExternalApplication
    {
        public static string assemblyPath = "";

        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            string tabName = "BIM-STARTER";
            try { application.CreateRibbonTab(tabName); } catch { }

            RibbonPanel panel1 = application.CreateRibbonPanel(tabName, "Параметры");
            PushButton btn = panel1.AddItem(new PushButtonData(
                "btnCopyProperties",
                "Копирование\nсвойств",
                assemblyPath,
                "PropertiesCopy.CommandPropertiesCopy")
                ) as PushButton;
            btn.ToolTip = "Супер-копирование свойств! Копирует все параметры, в том числе марку, высоту стен, параметры армирования и так далее";
            return Result.Succeeded;
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}

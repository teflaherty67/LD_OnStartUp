using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using LD_OnStartUp.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace LD_OnStartUp
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentOpened += OnDocumentOpened;

            return Result.Succeeded;
        }       

        public Result OnShutdown(UIControlledApplication a)
        {
            a.ControlledApplication.DocumentOpened -= OnDocumentOpened;
            return Result.Succeeded;
        }

        private void OnDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            Document curDoc = e.Document;

            // skip if document is a fmialy document
            if (curDoc.IsFamilyDocument)
                return;

            // check all standards
            AppUtils.CheckAllStandards(curDoc);
        }
    }
}
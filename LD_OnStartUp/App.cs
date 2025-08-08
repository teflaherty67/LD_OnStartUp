using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
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

            // check view templates
            CheckViewTemplates(curDoc);
        }

        private void CheckViewTemplates(Document curDoc)
        {
            // define required view templates
            List<string> curViewTemplates = new List<string>()
            {
                "-Frame Schedule-",
                "-Schedule-",
                "01-Enlarged Form Plans",
                "01-Form Plans",
                "02-Enlarged Plans",
                "02-Floor Annotations",
                "02-Floor Dimensions",
                "02-Key Plans",
                "03-Exterior Elevations",
                "03-Key Elevations",
                "03-Porch Elevations",
                "04-Roof Plans",
                "05-Sections",
                "05-Sections_3/8\"",
                "06-Cabinet Layout Plans",
                "06-Interior Elevations",
                "07-Electrical Plans",
                "08-Frame_Ceiling/Floor",
                "09-Frame_Roof",
                "10-Floor Area",
                "11-Frame Area",
                "12-Roof Ventilation",
                "13-Elevation Presentation",
                "13-Floor Presentation",
                "14-Ceiling",
                "14-Soffit",
                "15-Roof",
                "16-3D",
                "16-3D Frame",
                "17-Details",
                "18-Framing Elevation"
            };

            List<string> missingTemplates = FindMissingViewTemplates(curDoc, curViewTemplates);

            if (missingTemplates.Count > 0)
            {
                // Create compliance report
                CreateComplianceReport(curDoc, missingTemplates);

                // Show warning dialog
                ShowWarningDialog(curDoc.Title);
            }
        }

        private List<string> FindMissingViewTemplates(Document curDoc, List<string> curTemplates)
        {
            List<string> missingTemplates = new List<string>();

            // Get all view templates in the document
            FilteredElementCollector collector = new FilteredElementCollector(curDoc)
                .OfClass(typeof(View))
                .WhereElementIsNotElementType();

            HashSet<string> existingTemplates = new HashSet<string>();

            foreach (View view in collector)
            {
                if (view.IsTemplate)
                {
                    existingTemplates.Add(view.Name);
                }
            }

            // Check which required templates are missing
            foreach (string requiredTemplate in curTemplates)
            {
                if (!existingTemplates.Contains(requiredTemplate))
                {
                    missingTemplates.Add(requiredTemplate);
                }
            }

            return missingTemplates;
        }

        private void CreateComplianceReport(Document curDoc, List<string> missingTemplates)
        {
            try
            {
                // Get the project file path
                string projectPath = curDoc.PathName;

                if (string.IsNullOrEmpty(projectPath))
                {
                    // If document hasn't been saved yet, save to desktop
                    projectPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }
                else
                {
                    projectPath = Path.GetDirectoryName(projectPath);
                }

                // Create report file name
                string projectName = string.IsNullOrEmpty(curDoc.Title) ? "Untitled" : curDoc.Title;
                string reportFileName = $"{projectName} Compliance Report.txt";
                string reportPath = Path.Combine(projectPath, reportFileName);

                // Create report content
                List<string> reportLines = new List<string>();
                reportLines.Add("LIFESTYLE DESIGN STANDARDS COMPLIANCE REPORT");
                reportLines.Add("==========================================");
                reportLines.Add($"Project: {projectName}");
                reportLines.Add($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                reportLines.Add("");
                reportLines.Add("VIEW TEMPLATE COMPLIANCE:");
                reportLines.Add("");
                reportLines.Add($"Missing View Templates ({missingTemplates.Count}):");

                for (int i = 0; i < missingTemplates.Count; i++)
                {
                    reportLines.Add($"  {i + 1}. {missingTemplates[i]}");
                }

                // Write to file
                File.WriteAllLines(reportPath, reportLines);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"Could not create compliance report: {ex.Message}");
            }
        }

        private void ShowWarningDialog(string projectName)
        {
            var warningWindow = new frmComplianceWarning(projectName);
            warningWindow.ShowDialog();
        }
    }
}
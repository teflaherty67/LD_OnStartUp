using Autodesk.Revit.DB.Electrical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD_OnStartUp
{
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class StandardsCheckerApplication : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            // Subscribe to the DocumentOpened event
            application.ControlledApplication.DocumentOpened += OnDocumentOpened;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // Unsubscribe from the event
            application.ControlledApplication.DocumentOpened -= OnDocumentOpened;
            return Result.Succeeded;
        }

        private void OnDocumentOpened(object sender, Autodesk.Revit.DB.Events.DocumentOpenedEventArgs e)
        {
            Document doc = e.Document;

            // Skip if it's a family document
            if (doc.IsFamilyDocument)
                return;

            // Check view templates
            CheckViewTemplates(doc);
        }

        private void CheckViewTemplates(Document doc)
        {
            // Define required view templates
            List<string> requiredViewTemplates = new List<string>
            {
                "Frame Schedule-",
                "Schedule-",
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

            List<string> missingTemplates = FindMissingViewTemplates(doc, requiredViewTemplates);

            if (missingTemplates.Count > 0)
            {
                // Create compliance report
                CreateComplianceReport(doc, missingTemplates);

                // Show warning dialog
                ShowWarningDialog(doc.Title);
            }
        }

        private List<string> FindMissingViewTemplates(Document doc, List<string> requiredTemplates)
        {
            List<string> missingTemplates = new List<string>();

            // Get all view templates in the document
            FilteredElementCollector collector = new FilteredElementCollector(doc)
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
            foreach (string requiredTemplate in requiredTemplates)
            {
                if (!existingTemplates.Contains(requiredTemplate))
                {
                    missingTemplates.Add(requiredTemplate);
                }
            }

            return missingTemplates;
        }

        private void CreateComplianceReport(Document doc, List<string> missingTemplates)
        {
            try
            {
                // Get the project file path
                string projectPath = doc.PathName;

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
                string projectName = string.IsNullOrEmpty(doc.Title) ? "Untitled" : doc.Title;
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
            // Show final report
            TaskDialog tdFinalReport = new TaskDialog("Warning");
            tdFinalReport.TitleAutoPrefix = false;                      
            tdFinalReport.Title = "Lifestyle Design Standards";
            tdFinalReport.MainIcon = Icon.TaskDialogIconWarning;
            tdFinalReport.MainInstruction = "Warning: This file does not comply with Lifestyle Design standards.";
            tdFinalReport.MainContent = $"Please refer to the {projectName} Compliance Report.txt file located in the project folder.";
            tdFinalReport.CommonButtons = TaskDialogCommonButtons.Close;

            TaskDialogResult tdSchedSuccessRes = tdFinalReport.Show();
        }
    }
}
// -------------------------------------------------------------------------------------------------
// <copyright file="ReportGenerator.cs" company="Starion Group S.A.">
// 
//   Copyright 2022-2024 Starion Group S.A.
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace VCD.Generator.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    using ClosedXML.Excel;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using Spectre.Console;

    /// <summary>
    /// The purpose of the <see cref="ReportGenerator"/> is to generate a verification control document
    /// report
    /// </summary>
    public class ReportGenerator : IReportGenerator
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<ReportGenerator> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultReader"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        public ReportGenerator(ILoggerFactory loggerFactory = null)
        {
            this.logger = loggerFactory == null ? NullLogger<ReportGenerator>.Instance : loggerFactory.CreateLogger<ReportGenerator>();
        }

        /// <summary>
        /// Generates the VCD report in the specified location
        /// </summary>
        /// <param name="requirements">
        /// The <see cref="Requirement"/> objects on the basis of which the report will be generated
        /// </param>
        /// <param name="filePath">
        /// the file path (including file-name) where the report will be generated
        /// </param>
        /// <param name="reportKind">
        /// The kind of report that is generated
        /// </param>
        /// <param name="addStatusColumn">
        /// When <c>true</c>, a <c>STATUS</c> column with an aggregated, colour-coded outcome per
        /// requirement is appended to the report. See <see cref="DeriveStatus"/> for the rules.
        /// </param>
        public void Generate(IEnumerable<Requirement> requirements, string filePath, ReportKind reportKind, bool addStatusColumn = false)
        {
            switch (reportKind)
            {
                case ReportKind.SpreadSheet:
                    this.GenerateSpreadsheetReport(requirements, filePath, addStatusColumn);
                    break;
                case ReportKind.Html:
                    this.GeneratedHtmlReport(requirements, filePath);
                    break;
            }
        }

        /// <summary>
        /// Generates a Spreadsheet Report
        /// </summary>
        /// <param name="requirements">
        /// The <see cref="Requirement"/> objects on the basis of which the report will be generated
        /// </param>
        /// <param name="filePath">
        /// the file path (including file-name) where the report will be generated
        /// </param>
        /// <param name="addStatusColumn">
        /// When <c>true</c>, appends a <c>STATUS</c> column with the colour-coded per-requirement
        /// outcome; see <see cref="DeriveStatus"/>.
        /// </param>
        private void GenerateSpreadsheetReport(IEnumerable<Requirement> requirements, string filePath, bool addStatusColumn)
        {
            this.logger.LogInformation("Creating target workbook");

            var wb = new XLWorkbook();

            var now = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var worksheet = wb.Worksheets.Add($"VCD-{now}");

            var dataTable = new DataTable();
            dataTable.Columns.Add("REQUIREMENT-ID", typeof(string));
            dataTable.Columns.Add("REQUIREMENT-TEXT", typeof(string));
            dataTable.Columns.Add("TESTCASES", typeof(string));
            if (addStatusColumn)
            {
                dataTable.Columns.Add("STATUS", typeof(string));
            }

            var materialisedRequirements = requirements as IList<Requirement> ?? requirements.ToList();

            foreach (var requirement in materialisedRequirements)
            {
                var dataRow = dataTable.NewRow();
                dataRow["REQUIREMENT-ID"] = requirement.Identifier;
                dataRow["REQUIREMENT-TEXT"] = requirement.Text;

                var sb = new StringBuilder();

                foreach (var tc in requirement.TestCases)
                {
                    sb.AppendLine($"{tc.FullName} - {tc.Result}");
                }

                dataRow["TESTCASES"] = sb.ToString();

                if (addStatusColumn)
                {
                    dataRow["STATUS"] = DeriveStatus(requirement);
                }

                dataTable.Rows.Add(dataRow);
            }

            worksheet.Cell(1, 1).InsertTable(dataTable, "VCD", true);

            worksheet.Column("A").Style.Alignment.WrapText = false;
            worksheet.Column("B").Style.Alignment.WrapText = true;
            worksheet.Column("C").Style.Alignment.WrapText = false;

            worksheet.Column("A").Width = 25;
            worksheet.Column("B").Width = 80;

            if (addStatusColumn)
            {
                const int statusColumnIndex = 4;
                worksheet.Column(statusColumnIndex).Style.Alignment.WrapText = false;
                worksheet.Column(statusColumnIndex).Width = 15;

                for (var i = 0; i < materialisedRequirements.Count; i++)
                {
                    var status = (string)dataTable.Rows[i]["STATUS"];
                    ApplyStatusFill(worksheet.Cell(i + 2, statusColumnIndex), status);
                }
            }

            try
            {
                worksheet.Rows().AdjustToContents();
                worksheet.Columns().AdjustToContents();
            }
            catch (Exception e)
            {
                AnsiConsole.Markup($"[blue]Problem loading fonts[/]: {e.Message}");
            }

            wb.SaveAs(filePath);
            this.logger.LogInformation("Target workbook saved to: {filePath}", filePath);
        }

        /// <summary>
        /// Aggregates the outcome of every <see cref="TestCase"/> linked to a requirement into a
        /// single status value suitable for filtering.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="Requirement"/> whose test cases are aggregated.
        /// </param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><c>""</c> — no test cases (the requirement is uncovered)</description></item>
        ///   <item><description><c>"Failed"</c> — any test case has <c>Result == "Failed"</c></description></item>
        ///   <item><description><c>"Passed"</c> — every test case has <c>Result == "Passed"</c></description></item>
        ///   <item><description><c>"Mixed"</c> — at least one Passed and at least one non-Passed non-Failed</description></item>
        ///   <item><description><c>"Inconclusive"</c> — no Passed and no Failed (e.g. only Skipped/Inconclusive)</description></item>
        /// </list>
        /// </returns>
        public static string DeriveStatus(Requirement requirement)
        {
            if (requirement?.TestCases == null || requirement.TestCases.Count == 0)
            {
                return string.Empty;
            }

            if (requirement.TestCases.Any(tc => tc.Result == "Failed"))
            {
                return "Failed";
            }

            if (requirement.TestCases.All(tc => tc.Result == "Passed"))
            {
                return "Passed";
            }

            if (requirement.TestCases.Any(tc => tc.Result == "Passed"))
            {
                return "Mixed";
            }

            return "Inconclusive";
        }

        /// <summary>
        /// Applies the conventional Excel "Good/Bad/Neutral" fill palette to a status cell so the
        /// report is scannable at a glance.
        /// </summary>
        /// <param name="cell">
        /// The <see cref="IXLCell"/> to colour.
        /// </param>
        /// <param name="status">
        /// The aggregated status value produced by <see cref="DeriveStatus"/>.
        /// </param>
        private static void ApplyStatusFill(IXLCell cell, string status)
        {
            switch (status)
            {
                case "Passed":
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#C6EFCE"); // light green
                    break;
                case "Failed":
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFC7CE"); // light red
                    break;
                case "Mixed":
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFEB9C"); // light orange
                    break;
                case "Inconclusive":
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9"); // light grey
                    break;
            }
        }

        /// <summary>
        /// Generates a HTML Report
        /// </summary>
        /// <param name="requirements">
        /// The <see cref="Requirement"/> objects on the basis of which the report will be generated
        /// </param>
        /// <param name="filePath">
        /// the file path (including file-name) where the report will be generated 
        /// </param>
        private void GeneratedHtmlReport(IEnumerable<Requirement> requirements, string filePath)
        {
            throw new NotImplementedException("the HTML report generation is not yet supported");
        }
    }
}

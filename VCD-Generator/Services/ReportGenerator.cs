// -------------------------------------------------------------------------------------------------
// <copyright file="ReportGenerator.cs" company="RHEA System S.A.">
// 
//   Copyright 2022-2023 RHEA System S.A.
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
        public void Generate(IEnumerable<Requirement> requirements, string filePath, ReportKind reportKind)
        {
            switch (reportKind)
            {
                case ReportKind.SpreadSheet:
                    this.GenerateSpreadsheetReport(requirements,filePath);
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
        private void GenerateSpreadsheetReport(IEnumerable<Requirement> requirements, string filePath)
        {
            this.logger.LogInformation("Creating target workbook");

            var wb = new XLWorkbook();

            var now = DateTime.UtcNow.ToString("yyyy-MM-dd");
            
            var worksheet = wb.Worksheets.Add($"VCD-{now}");

            var dataTable = new DataTable();
            dataTable.Columns.Add("REQUIREMENT-ID", typeof(string));
            dataTable.Columns.Add("REQUIREMENT-TEXT", typeof(string));
            dataTable.Columns.Add("TESTCASES", typeof(string));
            
            foreach (var requirement in requirements)
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

                dataTable.Rows.Add(dataRow);
            }

            worksheet.Cell(1, 1).InsertTable(dataTable, "VCD", true);

            worksheet.Column("A").Style.Alignment.WrapText = false;
            worksheet.Column("B").Style.Alignment.WrapText = true;
            worksheet.Column("C").Style.Alignment.WrapText = false;

            worksheet.Column("A").Width = 25;
            worksheet.Column("B").Width = 80;

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

// -------------------------------------------------------------------------------------------------
// <copyright file="ReportGenerator.cs" company="RHEA System S.A.">
// 
//   Copyright 2022 RHEA System S.A.
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
    using System.Text;

    using ClosedXML.Excel;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

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
            var wb = new XLWorkbook();

            var now = DateTime.UtcNow.ToString("yyyy-mm-dd");
            
            var worksheet = wb.Worksheets.Add($"VCD-{now}");

            worksheet.Cell(1, 1).Value = "REQUIREMENT-ID";
            worksheet.Cell(1, 2).Value = "REQUIREMENT-TEXT";
            worksheet.Cell(1, 3).Value = "TESTCASE";

            var i = 2;

            foreach (var requirement in requirements)
            {
                worksheet.Cell(i, 1).Value = requirement.Identifier;
                worksheet.Cell(i, 2).Value = requirement.Text;
                
                var sb = new StringBuilder();

                foreach (var tc in requirement.TestCases)
                {
                    sb.AppendLine($"{tc.FullName} - {tc.Result}");
                }
                
                worksheet.Cell(i, 3).Value = sb.ToString();
                
                i++;
            }
            
            wb.SaveAs(filePath);
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

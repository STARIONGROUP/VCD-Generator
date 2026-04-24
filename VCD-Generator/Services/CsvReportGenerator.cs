// -------------------------------------------------------------------------------------------------
// <copyright file="CsvReportGenerator.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using nietras.SeparatedValues;

    /// <summary>
    /// The purpose of the <see cref="CsvReportGenerator"/> is to generate a verification control document
    /// report in CSV format. This is a helper class used by <see cref="ReportGenerator"/> and is not
    /// intended to be an <see cref="IReportGenerator"/> implementation on its own.
    /// </summary>
    public class CsvReportGenerator
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<CsvReportGenerator> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReportGenerator"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        public CsvReportGenerator(ILoggerFactory loggerFactory = null)
        {
            this.logger = loggerFactory == null ? NullLogger<CsvReportGenerator>.Instance : loggerFactory.CreateLogger<CsvReportGenerator>();
        }

        /// <summary>
        /// Generates a CSV VCD report at the specified location.
        /// </summary>
        /// <param name="requirements">
        /// The <see cref="Requirement"/> objects on the basis of which the report will be generated
        /// </param>
        /// <param name="filePath">
        /// the file path (including file-name) where the report will be generated
        /// </param>
        public void Generate(IEnumerable<Requirement> requirements, string filePath)
        {
            this.logger.LogInformation("Creating CSV report at {filePath}", filePath);

            using var writer = Sep.Writer(o => o with { Sep = new Sep(','), Escape = true }).ToFile(filePath);

            foreach (var requirement in requirements)
            {
                var testCases = string.Join("\n", requirement.TestCases.Select(tc => $"{tc.FullName} - {tc.Result}"));

                using var row = writer.NewRow();
                row["REQUIREMENT-ID"].Set(requirement.Identifier);
                row["REQUIREMENT-TEXT"].Set(requirement.Text);
                row["TESTCASES"].Set(testCases);
            }

            this.logger.LogInformation("CSV report saved to: {filePath}", filePath);
        }
    }
}

// -------------------------------------------------------------------------------------------------
// <copyright file="CsvRequirementsReader.cs" company="Starion Group S.A.">
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
    using System.IO;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using nietras.SeparatedValues;

    /// <summary>
    /// The purpose of the <see cref="CsvRequirementsReader"/> is to read the list of requirements from a CSV file
    /// </summary>
    /// <remarks>
    /// The input is a CSV file where the column that holds the requirement identifier (and optionally the
    /// requirement text) is specified by header name. The separator is auto-detected by the Sep library.
    /// </remarks>
    public class CsvRequirementsReader : IRequirementsReader
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<CsvRequirementsReader> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRequirementsReader"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        public CsvRequirementsReader(ILoggerFactory loggerFactory = null)
        {
            this.logger = loggerFactory == null ? NullLogger<CsvRequirementsReader>.Instance : loggerFactory.CreateLogger<CsvRequirementsReader>();
        }

        /// <summary>
        /// Reads the <see cref="Requirement"/>s from the specified CSV file
        /// </summary>
        /// <param name="fileInfo">
        /// The <see cref="FileInfo"/> to the requirements input file
        /// </param>
        /// <param name="sheetName">
        /// Ignored for CSV input. When a non-empty value is supplied a debug log entry is emitted and the value is discarded.
        /// </param>
        /// <param name="identifierColumnName">
        /// The name of the header column where the unique identifier of the requirements is located. When this is null
        /// or empty, the first column is used.
        /// </param>
        /// <param name="textColumnName">
        /// The name of the header column where the requirement text is located. When this is null the requirement text
        /// is ignored and remains an empty string.
        /// </param>
        /// <returns>
        /// A fully materialised <see cref="IEnumerable{Requirement}"/>
        /// </returns>
        public IEnumerable<Requirement> Read(FileInfo fileInfo, string sheetName = null, string identifierColumnName = null, string textColumnName = null)
        {
            if (!string.IsNullOrEmpty(sheetName))
            {
                this.logger.LogDebug("sheetName '{sheetName}' ignored for CSV input", sheetName);
            }

            var results = new List<Requirement>();

            using var reader = Sep.Reader().FromFile(fileInfo.FullName);

            int requirementIdColumnIndex;
            if (string.IsNullOrEmpty(identifierColumnName))
            {
                requirementIdColumnIndex = 0;
            }
            else
            {
                if (!reader.Header.TryIndexOf(identifierColumnName, out requirementIdColumnIndex))
                {
                    throw new InvalidRequirementsFormatException($"The identifier column with name \"{identifierColumnName}\" could not be found");
                }
            }

            var requirementTextColumnIndex = -1;
            if (textColumnName != null)
            {
                if (!reader.Header.TryIndexOf(textColumnName, out requirementTextColumnIndex))
                {
                    throw new InvalidRequirementsFormatException($"The text column with name \"{textColumnName}\" could not be found");
                }
            }

            foreach (var row in reader)
            {
                var identifier = row[requirementIdColumnIndex].ToString();

                if (string.IsNullOrWhiteSpace(identifier))
                {
                    continue;
                }

                var text = string.Empty;
                if (textColumnName != null)
                {
                    text = row[requirementTextColumnIndex].ToString();
                }

                this.logger.LogDebug("requirement found: {identifier}", identifier);

                var requirement = new Requirement
                {
                    Identifier = identifier.Trim(),
                    Text = text
                };

                results.Add(requirement);
            }

            return results;
        }
    }
}

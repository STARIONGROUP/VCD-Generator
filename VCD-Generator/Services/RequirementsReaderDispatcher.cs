// -------------------------------------------------------------------------------------------------
// <copyright file="RequirementsReaderDispatcher.cs" company="Starion Group S.A.">
//
//   Copyright 2022-2026 Starion Group S.A.
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
    using System.IO;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The purpose of the <see cref="RequirementsReaderDispatcher"/> is to delegate the reading of
    /// requirements to the appropriate concrete <see cref="IRequirementsReader"/> implementation
    /// based on the file extension of the provided input file.
    /// </summary>
    /// <remarks>
    /// Files with a <c>.csv</c> extension are routed to the <see cref="CsvRequirementsReader"/>.
    /// Any other extension (including <c>.xlsx</c>, empty or unknown extensions) is routed to the
    /// spreadsheet-based <see cref="RequirementsReader"/>.
    /// </remarks>
    public class RequirementsReaderDispatcher : IRequirementsReader
    {
        /// <summary>
        /// The (injected) <see cref="RequirementsReader"/> used for spreadsheet (xlsx) input
        /// </summary>
        private readonly RequirementsReader xlsxReader;

        /// <summary>
        /// The (injected) <see cref="CsvRequirementsReader"/> used for CSV input
        /// </summary>
        private readonly CsvRequirementsReader csvReader;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<RequirementsReaderDispatcher> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementsReaderDispatcher"/> class.
        /// </summary>
        /// <param name="xlsxReader">
        /// The (injected) <see cref="RequirementsReader"/> used when the input file is an xlsx spreadsheet
        /// </param>
        /// <param name="csvReader">
        /// The (injected) <see cref="CsvRequirementsReader"/> used when the input file is a CSV file
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="xlsxReader"/> or <paramref name="csvReader"/> is <c>null</c>.
        /// </exception>
        public RequirementsReaderDispatcher(RequirementsReader xlsxReader, CsvRequirementsReader csvReader, ILoggerFactory loggerFactory = null)
        {
            this.xlsxReader = xlsxReader
                ?? throw new ArgumentNullException(nameof(xlsxReader));
            this.csvReader = csvReader
                ?? throw new ArgumentNullException(nameof(csvReader));
            this.logger = loggerFactory == null ? NullLogger<RequirementsReaderDispatcher>.Instance : loggerFactory.CreateLogger<RequirementsReaderDispatcher>();
        }

        /// <summary>
        /// Reads the <see cref="Requirement"/>s from the specified file by dispatching to the
        /// concrete <see cref="IRequirementsReader"/> that matches the file extension.
        /// </summary>
        /// <param name="fileInfo">
        /// The <see cref="FileInfo"/> to the requirements input file
        /// </param>
        /// <param name="sheetName">
        /// The name of the sheet where the requirements are located, in case this is null the first sheet in the
        /// workbook is used. Ignored when the input is a CSV file.
        /// </param>
        /// <param name="identifierColumnName">
        /// The name of the column where the unique identifier of the requirements is located, in case
        /// this is null, the first used column is used
        /// </param>
        /// <param name="textColumnName">
        /// The name of the column where the requirement text is located. In case this is null the requirement text is ignored
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{Requirement}"/>
        /// </returns>
        public IEnumerable<Requirement> Read(FileInfo fileInfo, string sheetName = null, string identifierColumnName = null, string textColumnName = null)
        {
            var extension = fileInfo.Extension.ToLowerInvariant();

            if (extension == ".csv")
            {
                this.logger.LogDebug("dispatching {fileName} to {reader}", fileInfo.Name, "CSV");
                return this.csvReader.Read(fileInfo, sheetName, identifierColumnName, textColumnName);
            }

            this.logger.LogDebug("dispatching {fileName} to {reader}", fileInfo.Name, "xlsx");
            return this.xlsxReader.Read(fileInfo, sheetName, identifierColumnName, textColumnName);
        }
    }
}

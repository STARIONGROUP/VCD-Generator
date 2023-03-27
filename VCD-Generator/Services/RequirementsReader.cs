// -------------------------------------------------------------------------------------------------
// <copyright file="RequirementsReader.cs" company="RHEA System S.A.">
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
    using System.IO;
    
    using ClosedXML.Excel;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The purpose of the <see cref="IRequirementsReader"/> is to read the list of requirements
    /// </summary>
    /// <remarks>
    /// the input is an excel spreadsheet where the sheet-name and column ID need to be specified where the
    /// requirement human readable unique identifier are located
    /// </remarks>
    public class RequirementsReader : IRequirementsReader
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<RequirementsReader> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultReader"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        public RequirementsReader(ILoggerFactory loggerFactory = null)
        {
            this.logger = loggerFactory == null ? NullLogger<RequirementsReader>.Instance : loggerFactory.CreateLogger<RequirementsReader>();
        }

        /// <summary>
        /// Reads the <see cref="Requirement"/>s from the specified file fileName
        /// </summary>
        /// <param name="fileInfo">
        /// The <see cref="FileInfo"/> to the requirements input file
        /// </param>
        /// <param name="sheetName">
        /// The name of the sheet where the requirements are located, in case this is null the first sheet in the
        /// workbook is used
        /// </param>
        /// <param name="identifierColumnName">
        /// The name of the column where the unique identifier of the requirements is located on the sheet, in case
        /// this is null, the first used column in the sheet is used
        /// </param>
        /// <param name="textColumnName">
        /// The name of the column where the requirement text is located. In case this is null the requirement text is ignored
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{Requirement}"/>
        /// </returns>
        public IEnumerable<Requirement> Read(FileInfo fileInfo, string sheetName = null, string identifierColumnName = null, string textColumnName = null)
        {
            using var fs = fileInfo.OpenRead();
            var wb = new XLWorkbook(fs);

            IXLWorksheet requirementsSheet;

            try
            {
                requirementsSheet = string.IsNullOrEmpty(sheetName) ? wb.Worksheet(1) : wb.Worksheet(sheetName);
            }
            catch (ArgumentException)
            {
                throw new SheetNotFoundException(sheetName);
            }
            
            var results = new List<Requirement>();
            
            var firstRowUsed = requirementsSheet.FirstRowUsed();
            var lastRowUsed = requirementsSheet.LastRowUsed();
            var headerRow = firstRowUsed.RowUsed();

            var firstCell = headerRow.FirstCellUsed();
            var firstCellColumnNumber = firstCell.Address.ColumnNumber;
            
            var lastCell = headerRow.LastCellUsed();
            var lastCellColumnNumber = lastCell.Address.ColumnNumber;
            
            var requirementIdColumnNumber = identifierColumnName != null ? this.QueryColumnNumber(identifierColumnName, requirementsSheet, headerRow.RowNumber(), firstCellColumnNumber, lastCellColumnNumber) : firstCellColumnNumber;
            if (requirementIdColumnNumber == -1)
            {
                throw new InvalidRequirementsFormatException($"The identifier column with name \"{identifierColumnName}\" could not be found");
            }

            var requirementTextColumnNumber = -1;
            if (textColumnName != null)
            {
                requirementTextColumnNumber = this.QueryColumnNumber(textColumnName, requirementsSheet, headerRow.RowNumber(), firstCellColumnNumber, lastCellColumnNumber);
            }

            if (textColumnName != null && requirementTextColumnNumber == -1)
            {
                throw new InvalidRequirementsFormatException($"The text column with name \"{textColumnName}\" could not be found");
            }

            for (int i = headerRow.RowNumber() + 1; i < lastRowUsed.RowNumber() + 1; i++)
            {
                var identifier = requirementsSheet.Cell(i, requirementIdColumnNumber).Value.ToString();
                string text = string.Empty;

                if (textColumnName != null)
                {
                    text = requirementsSheet.Cell(i, requirementTextColumnNumber).Value.ToString();
                }
                
                if (!string.IsNullOrEmpty(identifier))
                {
                    this.logger.LogDebug("requirement found: {identifier}", identifier);

                    var requirement = new Requirement
                    {
                        Identifier = identifier.Trim(),
                        Text = text
                    };

                    results.Add(requirement);
                }
            }
            
            return results;
        }

        /// <summary>
        /// Queries the column number from the cell with the specified value in the provided row
        /// </summary>
        /// <param name="columnName">
        /// the name of the column that is being queried
        /// </param>
        /// <param name="requirementsSheet">
        /// The <see cref="IXLWorksheet"/> from which the data is queried
        /// </param>
        /// <param name="row">
        /// the row in which the data is queried
        /// </param>
        /// <param name="start">
        /// the starting cell in the row
        /// </param>
        /// <param name="end">
        /// the ending cell in the row
        /// </param>
        /// <returns>
        /// returns the column number in which the <paramref name="columnName"/> is found. -1 if not found
        /// </returns>
        private int QueryColumnNumber(string columnName, IXLWorksheet requirementsSheet, int row, int start, int end)
        {
            if (columnName != null)
            {
                for (int i = start; i < end; i++)
                {
                    var activeCell = requirementsSheet.Cell(row, i);
                    
                    var activeCellValue = activeCell.Value.ToString();

                    if (activeCellValue ==  columnName.Trim())
                    {
                        this.logger.LogDebug("{columnName}: {i}", columnName, i);
                        return i;
                    }
                }
            }
            
            return -1;
        }
    }
}

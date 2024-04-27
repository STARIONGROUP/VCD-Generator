// -------------------------------------------------------------------------------------------------
// <copyright file="IRequirementsReader.cs" company="Starion Group S.A.">
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

    /// <summary>
    /// The purpose of the <see cref="IRequirementsReader"/> is to read the list of requirements
    /// </summary>
    /// <remarks>
    /// the input is an excel spreadsheet where the sheet-name and column ID need to be specified where the
    /// requirement human readable unique identifier are located
    /// </remarks>
    public interface IRequirementsReader
    {
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
        IEnumerable<Requirement> Read(FileInfo fileInfo, string sheetName = null, string identifierColumnName = null, string textColumnName = null);
    }
}

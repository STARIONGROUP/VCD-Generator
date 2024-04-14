// -------------------------------------------------------------------------------------------------
// <copyright file="IReportGenerator.cs" company="RHEA System S.A.">
// 
//   Copyright 2022-2024 RHEA System S.A.
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

    /// <summary>
    /// The purpose of the <see cref="IReportGenerator"/> is to generate a verification control document
    /// report
    /// </summary>
    public interface IReportGenerator
    {
        /// <summary>
        /// Generates the VCD report in the specified location
        /// </summary>
        /// <param name="requirements">
        /// The <see cref="Requirement"/> objects on the basis of which the report will be generated
        /// </param>
        /// <param name="filePath">
        /// the file path(including file-name) where the report will be generated 
        /// </param>
        /// <param name="reportKind">
        /// The kind of report that is generated
        /// </param>
        void Generate(IEnumerable<Requirement> requirements, string filePath, ReportKind reportKind);
    }
}

// -------------------------------------------------------------------------------------------------
// <copyright file="ReportKind.cs" company="Starion Group S.A.">
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
    /// <summary>
    /// enumeration data-type that defines the possible kinds of reports that can be generated
    /// </summary>
    public enum ReportKind
    {
        /// <summary>
        /// Assertion that the report kind is a spreadsheet
        /// </summary>
        SpreadSheet,

        /// <summary>
        /// Assertion that the report kind is an HTML
        /// </summary>
        Html
    }
}

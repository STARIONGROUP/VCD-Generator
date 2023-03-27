// -------------------------------------------------------------------------------------------------
// <copyright file="ITestResultReader.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The purpose of the <see cref="ITestResultReader"/> is to read all the test results that
    /// are located in a folder (and sub folders). A test file ends with .Result.xml
    /// </summary>
    public interface ITestResultReader
    {
        /// <summary>
        /// Reads the <see cref="TestCase"/>s from the results file in
        /// the specified Directory and sub Directories
        /// </summary>
        /// <param name="directoryInfo">
        /// The <see cref="DirectoryInfo"/> from where to recursively read the the test results filr
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{TestCase}"/>
        /// </returns>
        IEnumerable<TestCase> Read(DirectoryInfo directoryInfo);
    }
}

﻿// -------------------------------------------------------------------------------------------------
// <copyright file="TestResultReader.cs" company="RHEA System S.A.">
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
    using System.Threading.Tasks;

    /// <summary>
    /// The purpose of the <see cref="TestResultReader"/> is to read all the test results that
    /// are located in a folder (and sub folders). A test file ends with .Result.xml
    /// </summary>
    public class TestResultReader : ITestResultReader
    {

        /// <summary>
        /// Asynchronously reads the <see cref="Test"/>s from the results file in
        /// the specified Directory and sub Directories
        /// </summary>
        /// <param name="directoryInfo">
        /// The <see cref="DirectoryInfo"/> from where to start the read
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{Test}"/>
        /// </returns>
        public Task<IEnumerable<Test>> Read(DirectoryInfo directoryInfo)
        {
            throw new NotImplementedException();
        }
    }
}

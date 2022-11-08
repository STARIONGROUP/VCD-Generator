// -------------------------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The purpose of the <see cref="TestResultReader"/> is to read all the test results that
    /// are located in a folder (and sub folders). A test file ends with .Result.xml
    /// </summary>
    public class TestResultReader : ITestResultReader
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<TestResultReader> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultReader"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        public TestResultReader(ILoggerFactory loggerFactory = null)
        {
            this.logger = loggerFactory == null ? NullLogger<TestResultReader>.Instance : loggerFactory.CreateLogger<TestResultReader>();
        }

        /// <summary>
        /// Asynchronously reads the <see cref="TestCase"/>s from the results file in
        /// the specified Directory and sub Directories
        /// </summary>
        /// <param name="directoryInfo">
        /// The <see cref="DirectoryInfo"/> from where to recursively read the the test results filr
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{TestCase}"/>
        /// </returns>
        public IEnumerable<TestCase> Read(DirectoryInfo directoryInfo)
        {
            var fileInfos = directoryInfo.GetFiles("*.Result.xml", SearchOption.AllDirectories);

            this.logger.LogDebug("{fileLength} Result files found", fileInfos.Length);

            var result = new List<TestCase>();

            foreach (var fileInfo in fileInfos)
            {
                this.logger.LogDebug("processing: {file}", fileInfo);

                var testCases = this.ReadXml(fileInfo.FullName);

                result.AddRange(testCases);
            }

            return result;
        }

        /// <summary>
        /// Reads the <see cref="TestCase"/> objects from the specified results file
        /// </summary>
        /// <param name="fileName">
        /// path to the test result file
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{TestCase}"/>
        /// </returns>
        private IEnumerable<TestCase> ReadXml(string fileName)
        {
            var testCases = new List<TestCase>();

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            
            var testCaseNodes = xmlDocument.GetElementsByTagName("test-case");

            this.logger.LogDebug("found a total of {testCaseNodesCount} testcases in {fileName}", testCaseNodes.Count, fileName);

            foreach (XmlNode testCaseNode in testCaseNodes)
            {
                var testCase = new TestCase();
                
                testCase.Name = testCaseNode.Attributes["name"]?.Value;
                testCase.FullName = testCaseNode.Attributes["fullname"]?.Value;
                testCase.Result = testCaseNode.Attributes["result"]?.Value;

                var propertyNodes = testCaseNode.SelectNodes("properties//property");

                foreach (XmlNode propertyNode in propertyNodes)
                {
                    var propertyName = propertyNode.Attributes["name"]?.Value;

                    switch (propertyName)
                    {
                        case "Description":
                            testCase.Description = propertyNode.Attributes["value"]?.Value;
                            break;
                        case "REQUIREMENT-ID":
                            var requirementId = propertyNode.Attributes["value"]?.Value;
                            if (!string.IsNullOrEmpty(requirementId))
                            {
                                testCase.RequirementId.Add(requirementId);
                            }
                            break;
                    }
                }

                testCases.Add(testCase);
            }

            this.logger.LogDebug("created a total of {testCasesCount} TestCase objects from {fileName}", testCases.Count, fileName);

            return testCases;
        }
    }
}

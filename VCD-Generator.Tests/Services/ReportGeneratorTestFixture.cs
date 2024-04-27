// -------------------------------------------------------------------------------------------------
// <copyright file="ReportGeneratorTestFixture.cs" company="Starion Group S.A.">
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

namespace VCD.Generator.Tests.Services
{
    using System;
    using System.IO;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using VCD.Generator.Services;
    using System.Collections.Generic;

    /// <summary>
    /// Suite of tests for the <see cref="ReportGenerator"/> class
    /// </summary>
    [TestFixture]
    public class ReportGeneratorTestFixture
    {
        private ReportGenerator reportGenerator;

        private List<Requirement> requirements;

        private string spreadsheetReportPath;

        private ILoggerFactory loggerFactory;

        [SetUp]
        public void SetUp()
        {
            this.loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

            this.spreadsheetReportPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "VCD-report.xlsx");

            this.reportGenerator = new ReportGenerator(this.loggerFactory);

            this.CreateTestData();
        }

        private void CreateTestData()
        {
            this.requirements = new List<Requirement>();

            var requirement_1 = new Requirement
            {
                Identifier = "REQ-01",
                Text = "The VCD Generator shall read the test results created by the NunitXml.TestLogger in the Nunit3 output format"
            };
            this.requirements.Add(requirement_1);

            var requirement_2 = new Requirement
            {
                Identifier = "REQ-02",
                Text = "The VCD Generator shall read the requirements from an excel spreadsheet"
            };
            this.requirements.Add(requirement_2);
            
            var testCase_1_1 = new TestCase
            {
                Name = "Verify_that_Read_throws_exception",
                FullName = "VCD.Generator.Tests.Services.TestResultReaderTestFixture.Verify_that_Read_throws_exception",
                Description = "Verifies that the TestResultReader.Read methods trows an exception",
                RequirementId = new List<string> { "REQ-01" },
                Result = "Passed"
            };
            requirement_1.TestCases.Add(testCase_1_1);

            var testCase_1_2 = new TestCase
            {
                Name = "Verify_that_Read_does_not_throw_exception",
                FullName = "VCD.Generator.Tests.Services.TestResultReaderTestFixture.Verify_that_Read_does_not_throw_exception",
                Description = "Verifies that the TestResultReader.Read methods does not throw an exception",
                RequirementId = new List<string> { "REQ-01" },
                Result = "Passed"
            };
            requirement_1.TestCases.Add(testCase_1_2);

            var testCase_2 = new TestCase
            {
                Name = "Verify_that_Read_throws_exception",
                FullName = "VCD.Generator.Tests.Services.RequirementsReaderTestFixture.Verify_that_Read_throws_exception",
                Description = "Verifies that the TestResultReader.Read methods trows an exception",
                RequirementId = new List<string> { "REQ-02" },
                Result = "Failed"
            };
            requirement_2.TestCases.Add(testCase_2);
        }

        [Test]
        public void Verify_that_spreadsheet_report_is_generated()
        {
            Assert.That(() => this.reportGenerator.Generate(this.requirements, this.spreadsheetReportPath, ReportKind.SpreadSheet),
                Throws.Nothing);
        }

        [Test]
        public void Verify_that_html_report_is_not_generated_and_exception_is_thrown()
        {
            Assert.That(() => this.reportGenerator.Generate(this.requirements, this.spreadsheetReportPath, ReportKind.Html),
                Throws.TypeOf<NotImplementedException>());
        }
    }
}

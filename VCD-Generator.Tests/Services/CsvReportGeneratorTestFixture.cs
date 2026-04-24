// -------------------------------------------------------------------------------------------------
// <copyright file="CsvReportGeneratorTestFixture.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using nietras.SeparatedValues;

    using NUnit.Framework;

    using VCD.Generator.Services;

    /// <summary>
    /// Suite of tests for the <see cref="CsvReportGenerator"/> class
    /// </summary>
    [TestFixture]
    public class CsvReportGeneratorTestFixture
    {
        private CsvReportGenerator csvReportGenerator;

        private List<Requirement> requirements;

        private string csvReportPath;

        private ILoggerFactory loggerFactory;

        [SetUp]
        public void SetUp()
        {
            this.loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

            this.csvReportPath = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString("N") + ".csv");

            this.csvReportGenerator = new CsvReportGenerator(this.loggerFactory);

            this.CreateTestData();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(this.csvReportPath))
            {
                File.Delete(this.csvReportPath);
            }
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
        public void Verify_that_Generate_writes_header_row_and_rows()
        {
            this.csvReportGenerator.Generate(this.requirements, this.csvReportPath);

            using var reader = Sep.Reader().FromFile(this.csvReportPath);

            var header = reader.Header.ColNames.ToArray();
            Assert.That(header, Is.EqualTo(new[] { "REQUIREMENT-ID", "REQUIREMENT-TEXT", "TESTCASES" }));

            var rows = new List<string[]>();
            foreach (var row in reader)
            {
                rows.Add(new[] { row[0].ToString(), row[1].ToString(), row[2].ToString() });
            }

            Assert.That(rows.Count, Is.EqualTo(this.requirements.Count));

            for (var i = 0; i < this.requirements.Count; i++)
            {
                Assert.That(rows[i][0], Is.EqualTo(this.requirements[i].Identifier));
            }
        }

        [Test]
        public void Verify_that_TESTCASES_cell_contains_embedded_newlines()
        {
            var requirement = this.requirements[0];
            var singleRequirement = new List<Requirement> { requirement };

            this.csvReportGenerator.Generate(singleRequirement, this.csvReportPath);

            using var reader = Sep.Reader(o => o with { Unescape = true }).FromFile(this.csvReportPath);

            var testCasesCells = new List<string>();
            foreach (var row in reader)
            {
                testCasesCells.Add(row["TESTCASES"].ToString());
            }

            Assert.That(testCasesCells.Count, Is.EqualTo(1));

            var tc1 = requirement.TestCases[0];
            var tc2 = requirement.TestCases[1];
            var expected = $"{tc1.FullName} - {tc1.Result}\n{tc2.FullName} - {tc2.Result}";

            Assert.That(testCasesCells[0], Is.EqualTo(expected));
        }

        [Test]
        public void Verify_that_Generate_overwrites_existing_file()
        {
            this.csvReportGenerator.Generate(this.requirements, this.csvReportPath);

            var secondBatch = new List<Requirement>
            {
                new Requirement
                {
                    Identifier = "REQ-99",
                    Text = "Overwrite requirement"
                }
            };

            this.csvReportGenerator.Generate(secondBatch, this.csvReportPath);

            using var reader = Sep.Reader().FromFile(this.csvReportPath);

            var ids = new List<string>();
            var texts = new List<string>();
            foreach (var row in reader)
            {
                ids.Add(row["REQUIREMENT-ID"].ToString());
                texts.Add(row["REQUIREMENT-TEXT"].ToString());
            }

            Assert.That(ids.Count, Is.EqualTo(1));
            Assert.That(ids[0], Is.EqualTo("REQ-99"));
            Assert.That(texts[0], Is.EqualTo("Overwrite requirement"));
        }
    }
}

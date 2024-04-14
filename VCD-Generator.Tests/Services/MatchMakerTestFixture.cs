// -------------------------------------------------------------------------------------------------
// <copyright file="MatchMakerTestFixture.cs" company="RHEA System S.A.">
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
// ------------------------------------------------------------------------------------------------;

namespace VCD.Generator.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using VCD.Generator.Services;

    /// <summary>
    /// Suite of tests for the <see cref="MatchMaker"/> class
    /// </summary>
    public class MatchMakerTestFixture
    {
        private MatchMaker matchMaker;
        
        private ILoggerFactory loggerFactory;

        private List<Requirement> requirements;

        private List<TestCase> testCases;
 
        [SetUp]
        public void SetUp()
        {
            this.loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

            this.CreateTestData();

            this.matchMaker = new MatchMaker(this.loggerFactory);
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
            
            this.testCases = new List<TestCase>();

            var testCase_1 = new TestCase
            {
                Name = "Verify_that_Read_throws_exception",
                FullName = "VCD.Generator.Tests.Services.TestResultReaderTestFixture.Verify_that_Read_throws_exception",
                Description = "Verifies that the TestResultReader.Read methods trows an exception",
                RequirementId = new List<string> { "REQ-01" },
            };
            this.testCases.Add(testCase_1);

            var testCase_2 = new TestCase
            {
                Name = "Verify_that_Read_throws_exception",
                FullName = "VCD.Generator.Tests.Services.RequirementsReaderTestFixture.Verify_that_Read_throws_exception",
                Description = "Verifies that the TestResultReader.Read methods trows an exception",
                RequirementId = new List<string> { "REQ-02" },
            };
            this.testCases.Add(testCase_2);
        }

        [Test]
        public void Verify_that_requirements_and_testcases_are_matched()
        {
            this.matchMaker.Match(this.requirements, this.testCases);

            var requirement_1 = this.requirements.Single(x => x.Identifier == "REQ-01");
            Assert.That(requirement_1.TestCases.Count, Is.EqualTo(1));

            var testcase_1 = requirement_1.TestCases.Single();
            Assert.That(testcase_1.RequirementId, Is.EqualTo(new List<string> { "REQ-01"} ));
            
            var requirement_2 = this.requirements.Single(x => x.Identifier == "REQ-02");
            Assert.That(requirement_2.TestCases.Count, Is.EqualTo(1));

            var testcase_2 = requirement_2.TestCases.Single();
            Assert.That(testcase_2.RequirementId, Is.EqualTo(new List<string> { "REQ-02" }));
        }
    }
}

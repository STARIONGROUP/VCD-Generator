// -------------------------------------------------------------------------------------------------
// <copyright file="RequirementsReaderTestFixture.cs" company="RHEA System S.A.">
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

namespace VCD.Generator.Tests.Services
{
    using System.Linq;
    using System.IO;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using VCD.Generator.Services;

    /// <summary>
    /// Suite of tests for the <see cref="RequirementsReader"/> class
    /// </summary>
    [TestFixture]
    public class RequirementsReaderTestFixture
    {
        private RequirementsReader requirementsReader;

        private FileInfo requirementsDocumentFileInfo_01;

        private FileInfo requirementsDocumentFileInfo_02;

        private ILoggerFactory loggerFactory;

        [SetUp]
        public void SetUp()
        {
            this.loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

            this.requirementsDocumentFileInfo_01 = new FileInfo(Path.Combine(TestContext.CurrentContext.WorkDirectory,
                "Data", "Requirements-01.xlsx"));

            this.requirementsDocumentFileInfo_02 = new FileInfo(Path.Combine(TestContext.CurrentContext.WorkDirectory,
                "Data", "Requirements-02.xlsx"));

            this.requirementsReader = new RequirementsReader(this.loggerFactory);
        }

        [Test(Description = "Verifies that the RequirementsReader.Read methods returns the expected requirements"),
         Property("REQUIREMENT-ID", "REQ-02")]
        public void Verify_that_Read_requirementsDocumentFileInfo_01_returns_the_expected_requirements()
        {
            var requirements = this.requirementsReader.Read(this.requirementsDocumentFileInfo_01).ToList();

            Assert.That(requirements.Count, Is.EqualTo(2));
        }

        [Test]
        public void Verify_that_Read_requirementsDocumentFileInfo_01_with_idcolumn_returns_the_expected_requirements()
        {
            var requirements = this.requirementsReader.Read(this.requirementsDocumentFileInfo_01, null, "Identifier")
                .ToList();

            Assert.That(requirements.Count, Is.EqualTo(2));

            var requirement = requirements.First();

            Assert.That(requirement.Identifier, Is.EqualTo("REQ-01"));
            Assert.That(requirement.Text, Is.EqualTo(""));
        }

        [Test]
        public void Verify_that_Read_requirementsDocumentFileInfo_01_with_idcolumn_and_textcolumn_returns_the_expected_requirements()
        {
            var requirements = this.requirementsReader
                .Read(this.requirementsDocumentFileInfo_01, null, "Identifier", "Requirement Text").ToList();

            Assert.That(requirements.Count, Is.EqualTo(2));

            var requirement = requirements.First();

            Assert.That(requirement.Identifier, Is.EqualTo("REQ-01"));
            Assert.That(requirement.Text,
                Is.EqualTo(
                    "The VCD Generator shall read the test results created by the NunitXml.TestLogger in the Nunit3 output format"));
        }

        [Test]
        public void Verify_that_Read_requirementsDocumentFileInfo_02_with_idcolumn_and_textcolumn_returns_the_expected_requirements()
        {
            var requirements = this.requirementsReader
                .Read(this.requirementsDocumentFileInfo_02, null, "Identifier", "Requirement Text").ToList();

            Assert.That(requirements.Count, Is.EqualTo(2));
        }
    }
}

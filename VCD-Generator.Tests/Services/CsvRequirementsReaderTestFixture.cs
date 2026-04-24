// -------------------------------------------------------------------------------------------------
// <copyright file="CsvRequirementsReaderTestFixture.cs" company="Starion Group S.A.">
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
    using System.Linq;
    using System.IO;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using VCD.Generator.Services;

    /// <summary>
    /// Suite of tests for the <see cref="CsvRequirementsReader"/> class
    /// </summary>
    [TestFixture]
    public class CsvRequirementsReaderTestFixture
    {
        private CsvRequirementsReader requirementsReader;

        private FileInfo requirementsDocumentFileInfo;

        private ILoggerFactory loggerFactory;

        [SetUp]
        public void SetUp()
        {
            this.loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

            this.requirementsDocumentFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.WorkDirectory,
                "Data", "requirements.csv"));

            this.requirementsReader = new CsvRequirementsReader(this.loggerFactory);
        }

        [Test(Description = "Verifies that the CsvRequirementsReader.Read method returns the expected requirements"),
         Property("REQUIREMENT-ID", "REQ-02")]
        public void Verify_that_Read_with_default_first_column_returns_expected_requirements()
        {
            var requirements = this.requirementsReader.Read(this.requirementsDocumentFileInfo).ToList();

            Assert.That(requirements, Is.Not.Empty);
            Assert.That(requirements.Select(x => x.Identifier).ToList(),
                Is.EqualTo(new[] { "REQ-01", "REQ-02", "REQ-03" }));
        }

        [Test]
        public void Verify_that_Read_with_named_identifier_column_returns_expected_requirements()
        {
            var requirements = this.requirementsReader
                .Read(this.requirementsDocumentFileInfo, null, "Identifier").ToList();

            Assert.That(requirements.Select(x => x.Identifier).ToList(),
                Is.EqualTo(new[] { "REQ-01", "REQ-02", "REQ-03" }));
        }

        [Test]
        public void Verify_that_Read_with_named_text_column_populates_Text()
        {
            var requirements = this.requirementsReader
                .Read(this.requirementsDocumentFileInfo, null, "Identifier", "Requirement Text").ToList();

            Assert.That(requirements.Any(r => !string.IsNullOrEmpty(r.Text)), Is.True);

            var multiline = requirements.Single(r => r.Identifier == "REQ-03");
            Assert.That(multiline.Text, Does.Contain("\n"));
            Assert.That(multiline.Text, Does.Contain("Line one of requirement"));
            Assert.That(multiline.Text, Does.Contain("Line two continues here"));
        }

        [Test]
        public void Verify_that_Read_with_unknown_identifier_column_throws_InvalidRequirementsFormatException()
        {
            Assert.That(
                () => this.requirementsReader
                    .Read(this.requirementsDocumentFileInfo, null, "NotAColumn").ToList(),
                Throws.TypeOf<InvalidRequirementsFormatException>());
        }

        [Test]
        public void Verify_that_Read_with_unknown_text_column_throws_InvalidRequirementsFormatException()
        {
            Assert.That(
                () => this.requirementsReader
                    .Read(this.requirementsDocumentFileInfo, null, "Identifier", "NotAColumn").ToList(),
                Throws.TypeOf<InvalidRequirementsFormatException>());
        }

        [Test]
        public void Verify_that_empty_identifier_rows_are_skipped()
        {
            var requirements = this.requirementsReader.Read(this.requirementsDocumentFileInfo).ToList();

            Assert.That(requirements.Count, Is.EqualTo(3));
        }

        [Test]
        public void Verify_that_identifier_is_trimmed()
        {
            var requirements = this.requirementsReader.Read(this.requirementsDocumentFileInfo).ToList();

            foreach (var requirement in requirements)
            {
                Assert.That(requirement.Identifier, Is.EqualTo(requirement.Identifier.Trim()));
            }
        }

        [Test]
        public void Verify_that_sheetName_is_ignored_for_csv_input()
        {
            var requirements = this.requirementsReader
                .Read(this.requirementsDocumentFileInfo, "AnySheet", "Identifier", "Requirement Text").ToList();

            Assert.That(requirements.Count, Is.EqualTo(3));
        }
    }
}

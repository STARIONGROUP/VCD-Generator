// -------------------------------------------------------------------------------------------------
// <copyright file="RequirementsReaderDispatcherTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2022-2026 Starion Group S.A.
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
    using System.IO;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using VCD.Generator.Services;

    /// <summary>
    /// Suite of tests for the <see cref="RequirementsReaderDispatcher"/> class.
    /// </summary>
    /// <remarks>
    /// Because the concrete <see cref="RequirementsReader.Read"/> and
    /// <see cref="CsvRequirementsReader.Read"/> methods are not virtual, the dispatcher is
    /// verified against real input files. Each routing assertion confirms that the expected
    /// concrete reader executed by inspecting the shape of the returned requirements, which
    /// differs between the xlsx and csv fixtures.
    /// </remarks>
    [TestFixture]
    public class RequirementsReaderDispatcherTestFixture
    {
        private RequirementsReaderDispatcher dispatcher;

        private FileInfo xlsxFileInfo;

        private FileInfo csvFileInfo;

        private ILoggerFactory loggerFactory;

        [SetUp]
        public void SetUp()
        {
            this.loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

            this.xlsxFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.WorkDirectory,
                "Data", "Requirements-01.xlsx"));

            this.csvFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.WorkDirectory,
                "Data", "requirements.csv"));

            var xlsxReader = new RequirementsReader(this.loggerFactory);
            var csvReader = new CsvRequirementsReader(this.loggerFactory);

            this.dispatcher = new RequirementsReaderDispatcher(xlsxReader, csvReader, this.loggerFactory);
        }

        [Test]
        public void Verify_that_csv_extension_routes_to_CsvRequirementsReader()
        {
            var requirements = this.dispatcher.Read(this.csvFileInfo).ToList();

            // The csv fixture contains three requirements (REQ-01, REQ-02, REQ-03)
            Assert.That(requirements.Count, Is.EqualTo(3));
            Assert.That(requirements.Select(r => r.Identifier).ToList(),
                Is.EqualTo(new[] { "REQ-01", "REQ-02", "REQ-03" }));
        }

        [Test]
        public void Verify_that_xlsx_extension_routes_to_RequirementsReader()
        {
            var requirements = this.dispatcher.Read(this.xlsxFileInfo).ToList();

            // The xlsx fixture contains two requirements
            Assert.That(requirements.Count, Is.EqualTo(2));
        }

        [Test]
        public void Verify_that_uppercase_extension_is_handled()
        {
            var uppercaseCopyPath = Path.Combine(TestContext.CurrentContext.WorkDirectory,
                "Data", "requirements-uppercase.CSV");

            File.Copy(this.csvFileInfo.FullName, uppercaseCopyPath, true);

            try
            {
                var uppercaseFileInfo = new FileInfo(uppercaseCopyPath);

                var requirements = this.dispatcher.Read(uppercaseFileInfo).ToList();

                // Confirms csv routing: shape matches the csv fixture, not the xlsx fixture
                Assert.That(requirements.Count, Is.EqualTo(3));
                Assert.That(requirements.Select(r => r.Identifier).ToList(),
                    Is.EqualTo(new[] { "REQ-01", "REQ-02", "REQ-03" }));
            }
            finally
            {
                if (File.Exists(uppercaseCopyPath))
                {
                    File.Delete(uppercaseCopyPath);
                }
            }
        }
    }
}

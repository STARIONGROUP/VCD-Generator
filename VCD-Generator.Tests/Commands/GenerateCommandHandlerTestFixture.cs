// -------------------------------------------------------------------------------------------------
// <copyright file="GenerateCommandHandlerTestFixture.cs" company="RHEA System S.A.">
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

namespace VCD.Generator.Tests.Commands
{
    using System.Collections.Generic;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    
    using Moq;
    
    using NUnit.Framework;

    using VCD.Generator.Commands;
    using VCD.Generator.Services;

    /// <summary>
    /// Suite of tests for the <see cref="GenerateCommand"/> class
    /// </summary>
    [TestFixture]
    public class GenerateCommandHandlerTestFixture
    {
        private Mock<IRequirementsReader> requirementsReader;

        private Mock<ITestResultReader> resultReader;

        private Mock<IMatchMaker> matchMaker;

        private Mock<IReportGenerator> reportGenerator;

        private Mock<ILogger<GenerateCommand>> logger;

        private GenerateCommand.Handler handler;

        [SetUp]
        public void SetUp()
        {
            this.requirementsReader = new Mock<IRequirementsReader>();
            this.requirementsReader.Setup(x => x.Read(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Enumerable.Empty<Requirement>());

            this.resultReader = new Mock<ITestResultReader>();
            this.resultReader.Setup(x => x.Read(It.IsAny<string>()))
                .Returns(Enumerable.Empty<TestCase>());

            this.matchMaker = new Mock<IMatchMaker>();
            this.reportGenerator = new Mock<IReportGenerator>();
            this.logger = new Mock<ILogger<GenerateCommand>>();

            this.handler = new GenerateCommand.Handler(
                this.requirementsReader.Object,
                this.resultReader.Object,
                this.matchMaker.Object,
                this.reportGenerator.Object,
                this.logger.Object);

            this.handler.RequirementsFile = new FileInfo(TestContext.CurrentContext.WorkDirectory);
            this.handler.SourceDirectory = new DirectoryInfo(TestContext.CurrentContext.WorkDirectory);
            this.handler.OutputReport = new FileInfo(TestContext.CurrentContext.WorkDirectory);
        }

        [Test]
        public async Task Verify_that_InvokeAsync_returns_0()
        {
            var invocationContext = new InvocationContext(null);

            var result = await this.handler.InvokeAsync(invocationContext);

            this.requirementsReader.Verify(x =>
                x.Read(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);

            this.resultReader.Verify(x => x.Read(It.IsAny<string>()),
                Times.Once);

            this.matchMaker.Verify(
                x => x.Match(It.IsAny<IEnumerable<Requirement>>(), It.IsAny<IEnumerable<TestCase>>()),
                Times.Once);

            this.reportGenerator.Verify(x => x.Generate(It.IsAny<IEnumerable<Requirement>>(), It.IsAny<string>(), ReportKind.SpreadSheet),
                Times.Once);


            Assert.That(result, Is.EqualTo(0));
        }
    }
}

// -------------------------------------------------------------------------------------------------
// <copyright file="GenerateCommand.cs" company="RHEA System S.A.">
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

namespace VCD.Generator.Commands
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Spectre.Console;

    using VCD.Generator.Resources;
    using VCD.Generator.Services;

    /// <summary>
    /// The <see cref="RootCommand"/> that generates the VCD
    /// </summary>
    public class GenerateCommand : RootCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateCommand"/>
        /// </summary>
        public GenerateCommand() : base("VCD Generator")
        {
            var noLogoOption = new Option<bool>(
                name: "--no-logo",
                description: "Suppress the logo",
                getDefaultValue: () => false);
            this.AddOption(noLogoOption);

            var requirementsFileOption = new Option<FileInfo>(
                name: "--requirements-file",
                description: "The spreadsheet file that contains the requirements that need to be verified",
                getDefaultValue: () => new FileInfo("requirements.xlsx"));
            requirementsFileOption.AddAlias("-rf");
            requirementsFileOption.IsRequired = true;
            this.AddOption(requirementsFileOption);
            
            var testResultsFileOption = new Option<DirectoryInfo>(
                name: "--source-directory",
                description: "The directory that contains the test result files, this directory is process recursively",
                getDefaultValue: () =>
                {
                    var strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    return new FileInfo(strExeFilePath).Directory;
                });
            testResultsFileOption.AddAlias("-sd");
            testResultsFileOption.IsRequired = true;
            this.AddOption(testResultsFileOption);

            var reportFileOption = new Option<FileInfo>(
                name: "--output-report",
                description: "The path to the report file",
                getDefaultValue: () => new FileInfo("VCD-report.xlsx"));
            reportFileOption.AddAlias("-o");
            reportFileOption.IsRequired = true;
            this.AddOption(reportFileOption);
        }

        /// <summary>
        /// The Command Handler of the <see cref="GenerateCommand"/>
        /// </summary>
        public new class Handler : ICommandHandler
        {
            /// <summary>
            /// The (injected) <see cref="IRequirementsReader"/> that is used to read a set of requirements
            /// </summary>
            private readonly IRequirementsReader requirementsReader;

            /// <summary>
            /// The (injected) <see cref="ITestResultReader"/> that is used to read the NUnit test results
            /// </summary>
            private readonly ITestResultReader resultReader;

            /// <summary>
            /// The (injected) <see cref="IMatchMaker"/> that is used to match test cases to requirements
            /// </summary>
            private readonly IMatchMaker matchMaker;

            /// <summary>
            /// The (injected) <see cref="IReportGenerator"/> that is used to generate the VCD report
            /// </summary>
            private readonly IReportGenerator reportGenerator;

            /// <summary>
            /// The (injected) <see cref="ILogger"/>
            /// </summary>
            private readonly ILogger<GenerateCommand> logger;

            /// <summary>
            /// Initializes a nwe instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="requirementsReader">
            /// The (injected) <see cref="IRequirementsReader"/> that is used to read a set of requirements
            /// </param>
            /// <param name="resultReader">
            /// The (injected) <see cref="ITestResultReader"/> that is used to read the NUnit test results
            /// </param>
            /// <param name="matchMaker">
            /// The (injected) <see cref="IMatchMaker"/> that is used to match test cases to requirements
            /// </param>
            /// <param name="reportGenerator">
            /// The (injected) <see cref="IReportGenerator"/> that is used to generate the VCD report
            /// </param>
            /// <param name="logger">
            /// The (injected) <see cref="ILogger{Handler}"/>
            /// </param>
            public Handler(IRequirementsReader requirementsReader, ITestResultReader resultReader, IMatchMaker matchMaker, IReportGenerator reportGenerator, ILogger<GenerateCommand>  logger)
            {
                this.requirementsReader = requirementsReader 
                    ?? throw new ArgumentNullException(nameof(requirementsReader));
                this.resultReader = resultReader
                    ?? throw new ArgumentNullException(nameof(resultReader));
                this.reportGenerator = reportGenerator
                    ?? throw new ArgumentNullException(nameof(reportGenerator));
                this.matchMaker = matchMaker 
                    ?? throw new ArgumentNullException(nameof(matchMaker));
                this.logger = logger 
                    ?? throw new ArgumentNullException(nameof(logger));
            }

            /// <summary>
            /// Gets or sets the value indicating whether the logo should be shown or not
            /// </summary>
            public bool NoLogo { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="FileInfo"/> that points to the Requirement file
            /// </summary>
            public FileInfo RequirementsFile { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="DirectoryInfo"/> (and subfolders) in which the NUnit test results are located
            /// </summary>
            public DirectoryInfo SourceDirectory { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="FileInfo"/> where the output report is to be generated
            /// </summary>
            public FileInfo OutputReport { get; set; }

            /// <summary>
            /// Invokes the <see cref="ICommandHandler"/>
            /// </summary>
            /// <param name="context">
            /// The <see cref="InvocationContext"/> 
            /// </param>
            /// <returns>
            /// 0 when successful, another if not
            /// </returns>
            public int Invoke(InvocationContext context)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Asynchronously invokes the <see cref="ICommandHandler"/>
            /// </summary>
            /// <param name="context">
            /// The <see cref="InvocationContext"/> 
            /// </param>
            /// <returns>
            /// 0 when successful, another if not
            /// </returns>
            public async Task<int> InvokeAsync(InvocationContext context)
            {
                if (!this.NoLogo)
                {
                    AnsiConsole.Markup($"[blue]{ResourceLoader.QueryLogo()}[/]");
                }

                if (!this.RequirementsFile.Exists)
                {
                    AnsiConsole.MarkupLine($"[red]The specified requirements file does not exist[/]");
                    AnsiConsole.MarkupLine($"[red]{this.RequirementsFile.FullName}[/]");
                    return -1;
                }

                if (!this.SourceDirectory.Exists)
                {
                    AnsiConsole.MarkupLine($"[red]The specified test case source directory does not exist[/]");
                    AnsiConsole.MarkupLine($"[red]{this.SourceDirectory.FullName}[/]");
                    return -1;
                }

                try
                {
                    await AnsiConsole.Status()
                        .AutoRefresh(true)
                        .SpinnerStyle(Style.Parse("green bold"))
                        .Start("Getting ready for takeoff...", ctx =>
                        {
                            ctx.Status("Reading Requirements...");
                            var requirements = this.requirementsReader.Read(this.RequirementsFile);
                            AnsiConsole.MarkupLine($"[grey]LOG:[/] A total of [bold]{requirements.Count()}[/] requirements were read");

                            ctx.Status("Reading NUnit Test Results...");
                            var testCases = this.resultReader.Read(this.SourceDirectory);
                            AnsiConsole.MarkupLine($"[grey]LOG:[/] A total of [bold]{testCases.Count()} [/] test cases were read");

                            ctx.Status($"Matching [green]{requirements.Count()}[/] requirements to [red]{testCases.Count()}[/] test cases...");
                            this.matchMaker.Match(requirements, testCases);

                            ctx.Status($"Generating report");
                            this.reportGenerator.Generate(requirements, this.OutputReport.FullName, ReportKind.SpreadSheet);
                            AnsiConsole.MarkupLine($"[grey]LOG:[/] VCD report generated at [bold]{this.OutputReport.FullName}[/]");
                            
                            return Task.FromResult(0);
                        });
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[red]An exception occurred, please report an issue at[/]"); 
                    AnsiConsole.MarkupLine("[link] https://github.com/RHEAGROUP/VCD-Generator/issues [/]");
                    AnsiConsole.WriteLine();
                    AnsiConsole.WriteException(ex);

                    this.logger.LogError(ex, "`VCD Generator Failed");
                    return -1;
                }

                return 0;
            }
        }
    }
}

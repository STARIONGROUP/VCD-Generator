// -------------------------------------------------------------------------------------------------
// <copyright file="GenerateCommand.cs" company="RHEA System S.A.">
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
// ------------------------------------------------------------------------------------------------

namespace VCD.Generator.Commands
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Linq;
    using System.Threading;
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

            var requirementsSheetNameOption = new Option<string>(
                name: "--requirements-sheet-name",
                description: "The name of the requirements sheet in the spreadsheet file that is to be processed. If left empty then the first sheet in the workbook will be used.");
            requirementsSheetNameOption.AddAlias("-sn");
            requirementsSheetNameOption.IsRequired = false;
            this.AddOption(requirementsSheetNameOption);

            var identifierColumnNameOption = new Option<string>(
                name:"--requirements-id-column",
                description: "The name of the table-column that contains the identifier of the requirements. If left empty then the first column with content is used.");
            identifierColumnNameOption.IsRequired = false;
            identifierColumnNameOption.AddAlias("-id");
            this.AddOption(identifierColumnNameOption);

            var textColumnNameOption = new Option<string>(
                name: "--requirements-text-column",
                description: "The name of the table-column that contains the text of the requirements. If left empty then the requirement text is ignored.");
            textColumnNameOption.IsRequired = false;
            textColumnNameOption.AddAlias("-txt");
            this.AddOption(textColumnNameOption);

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
            /// Gets or sets the name of the sheet in the excel workbook that contains the requirements
            /// </summary>
            public string RequirementsSheetName { get; set; }

            /// <summary>
            /// Gets or sets the name of the requirements ID column in the requirements spreadsheet
            /// </summary>
            public string RequirementsIdColumn { get; set; }

            /// <summary>
            /// Gets or sets the name of the requirements text column in the requirements spreadsheet
            /// </summary>
            public string RequirementsTextColumn { get; set; }

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
                    AnsiConsole.MarkupLine($"[purple]{this.RequirementsFile.FullName}[/]");
                    return -1;
                }

                if (!this.SourceDirectory.Exists)
                {
                    AnsiConsole.MarkupLine($"[red]The specified test case source directory does not exist[/]");
                    AnsiConsole.MarkupLine($"[purple]{this.SourceDirectory.FullName}[/]");
                    return -1;
                }

                try
                {
                    await AnsiConsole.Status()
                        .AutoRefresh(true)
                        .SpinnerStyle(Style.Parse("green bold"))
                        .Start("Preparing Warp Engines...", ctx =>
                        {
                            Thread.Sleep(1500);

                            ctx.Status("Reading Requirements at Warp 2...");
                            Thread.Sleep(1500);
                            
                            IEnumerable<Requirement> requirements;

                            try
                            {
                                requirements = this.requirementsReader.Read(
                                    this.RequirementsFile,
                                    this.RequirementsSheetName,
                                    this.RequirementsIdColumn, 
                                    this.RequirementsTextColumn);
                                AnsiConsole.MarkupLine($"[grey]LOG:[/] A total of [bold]{requirements.Count()}[/] requirements were read");
                            }
                            catch (SheetNotFoundException)
                            {
                                AnsiConsole.MarkupLine($"[red]The specified sheet name does not exist[/]");
                                AnsiConsole.MarkupLine($"[purple]{this.RequirementsSheetName}[/]");
                                AnsiConsole.MarkupLine($"[blue]Back to Impulse speed[/]");

                                ctx.Status("Dropping to Impulse speed!");
                                Thread.Sleep(1500);

                                return Task.FromResult(-1);
                            }
                            catch (InvalidRequirementsFormatException e)
                            {
                                AnsiConsole.MarkupLine($"[red]{e.Message}[/]");

                                ctx.Status("Dropping to Impulse speed!");
                                Thread.Sleep(1500);

                                return Task.FromResult(-1);
                            }

                            ctx.Status("Reading NUnit Test Results at Warp 7...");
                            Thread.Sleep(1500);

                            var testCases = this.resultReader.Read(this.SourceDirectory);
                            AnsiConsole.MarkupLine($"[grey]LOG:[/] A total of [bold]{testCases.Count()} [/] test cases were read");

                            ctx.Status($"Matching [green]{requirements.Count()}[/] requirements to [red]{testCases.Count()}[/] test cases at warp 9...");
                            Thread.Sleep(1500);

                            this.matchMaker.Match(requirements, testCases);

                            ctx.Status($"Generating report at Warp 11, Captain..., SLOW DOWN!");
                            Thread.Sleep(1500);

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

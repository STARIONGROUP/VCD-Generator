// -------------------------------------------------------------------------------------------------
// <copyright file="GenerateCommand.cs" company="Starion Group S.A.">
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

namespace VCD.Generator.Commands
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
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
        /// The <see cref="Option{T}"/> to suppress the logo
        /// </summary>
        public Option<bool> NoLogoOption { get; }

        /// <summary>
        /// The <see cref="Option{T}"/> for the requirements spreadsheet file
        /// </summary>
        public Option<FileInfo> RequirementsFileOption { get; }

        /// <summary>
        /// The <see cref="Option{T}"/> for the name of the requirements sheet
        /// </summary>
        public Option<string> RequirementsSheetNameOption { get; }

        /// <summary>
        /// The <see cref="Option{T}"/> for the name of the requirements id column
        /// </summary>
        public Option<string> RequirementsIdColumnOption { get; }

        /// <summary>
        /// The <see cref="Option{T}"/> for the name of the requirements text column
        /// </summary>
        public Option<string> RequirementsTextColumnOption { get; }

        /// <summary>
        /// The <see cref="Option{T}"/> for the directory that contains the test result files
        /// </summary>
        public Option<DirectoryInfo> SourceDirectoryOption { get; }

        /// <summary>
        /// The <see cref="Option{T}"/> for the path of the generated report file
        /// </summary>
        public Option<FileInfo> OutputReportOption { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateCommand"/>
        /// </summary>
        public GenerateCommand() : base("VCD Generator")
        {
            this.NoLogoOption = new Option<bool>("--no-logo")
            {
                Description = "Suppress the logo",
            };
            this.Options.Add(this.NoLogoOption);

            this.RequirementsFileOption = new Option<FileInfo>("--requirements-file", "-rf")
            {
                Description = "The spreadsheet file that contains the requirements that need to be verified",
                DefaultValueFactory = _ => new FileInfo("requirements.xlsx"),
                Required = true,
            };
            this.Options.Add(this.RequirementsFileOption);

            this.RequirementsSheetNameOption = new Option<string>("--requirements-sheet-name", "-sn")
            {
                Description = "The name of the requirements sheet in the spreadsheet file that is to be processed. If left empty then the first sheet in the workbook will be used.",
            };
            this.Options.Add(this.RequirementsSheetNameOption);

            this.RequirementsIdColumnOption = new Option<string>("--requirements-id-column", "-id")
            {
                Description = "The name of the table-column that contains the identifier of the requirements. If left empty then the first column with content is used.",
            };
            this.Options.Add(this.RequirementsIdColumnOption);

            this.RequirementsTextColumnOption = new Option<string>("--requirements-text-column", "-txt")
            {
                Description = "The name of the table-column that contains the text of the requirements. If left empty then the requirement text is ignored.",
            };
            this.Options.Add(this.RequirementsTextColumnOption);

            this.SourceDirectoryOption = new Option<DirectoryInfo>("--source-directory", "-sd")
            {
                Description = "The directory that contains the test result files, this directory is process recursively",
                DefaultValueFactory = _ => new DirectoryInfo(AppContext.BaseDirectory),
                Required = true,
            };
            this.Options.Add(this.SourceDirectoryOption);

            this.OutputReportOption = new Option<FileInfo>("--output-report", "-o")
            {
                Description = "The path to the report file",
                DefaultValueFactory = _ => new FileInfo("VCD-report.xlsx"),
                Required = true,
            };
            this.Options.Add(this.OutputReportOption);
        }

        /// <summary>
        /// Binds the values parsed from the command line to the <see cref="Handler"/>
        /// </summary>
        /// <param name="handler">
        /// The <see cref="Handler"/> that will execute the command
        /// </param>
        /// <param name="parseResult">
        /// The <see cref="ParseResult"/> produced by parsing the command line arguments
        /// </param>
        public void BindTo(Handler handler, ParseResult parseResult)
        {
            handler.NoLogo = parseResult.GetValue(this.NoLogoOption);
            handler.RequirementsFile = parseResult.GetValue(this.RequirementsFileOption);
            handler.RequirementsSheetName = parseResult.GetValue(this.RequirementsSheetNameOption);
            handler.RequirementsIdColumn = parseResult.GetValue(this.RequirementsIdColumnOption);
            handler.RequirementsTextColumn = parseResult.GetValue(this.RequirementsTextColumnOption);
            handler.SourceDirectory = parseResult.GetValue(this.SourceDirectoryOption);
            handler.OutputReport = parseResult.GetValue(this.OutputReportOption);
        }

        /// <summary>
        /// The Command Handler of the <see cref="GenerateCommand"/>
        /// </summary>
        public class Handler
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
            /// Asynchronously executes the command
            /// </summary>
            /// <returns>
            /// 0 when successful, another if not
            /// </returns>
            public async Task<int> InvokeAsync()
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

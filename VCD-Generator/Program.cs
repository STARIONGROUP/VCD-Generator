// -------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Starion Group S.A.">
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

namespace VCD.Generator
{
    using System.CommandLine.Builder;
    using System.CommandLine.Help;
    using System.CommandLine.Hosting;
    using System.CommandLine.Parsing;

    using System.Linq;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
 
    using Serilog;

    using Spectre.Console;

    using VCD.Generator.Commands;
    using VCD.Generator.Resources;
    using VCD.Generator.Services;

    /// <summary>
    /// VCD Generator Command line app
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point of the command line application
        /// </summary>
        /// <param name="args">
        /// the command line arguments
        /// </param>
        /// <returns>
        /// the return code, 0 denotes success
        /// </returns>
        public static int Main(string[] args)
        {
            var commandLineBuilder = BuildCommandLine()
                .UseHost(_ => Host.CreateDefaultBuilder(args)
                    .UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                        .ReadFrom.Configuration(context.Configuration)
                    ), builder => builder
                    .ConfigureServices((hostContext, services) => 
                    {
                        services.AddSingleton<IRequirementsReader, RequirementsReader>();
                        services.AddSingleton<ITestResultReader, TestResultReader>();
                        services.AddSingleton<IMatchMaker, MatchMaker>();
                        services.AddSingleton<IReportGenerator, ReportGenerator>();
                    })
                    .UseCommandHandler<GenerateCommand, GenerateCommand.Handler>())
                .UseDefaults()
                .Build();

            return commandLineBuilder.Invoke(args);
        }

        /// <summary>
        /// builds the root command
        /// </summary>
        /// <returns>
        /// The <see cref="CommandLineBuilder"/> with the root command set
        /// </returns>
        private static CommandLineBuilder BuildCommandLine()
        {
            var root = new GenerateCommand();
            
            return new CommandLineBuilder(root)
                .UseHelp(ctx =>
                {
                    ctx.HelpBuilder.CustomizeLayout(_ =>
                        HelpBuilder.Default
                            .GetLayout()
                            .Skip(1) // Skip the default command description section.
                            .Prepend(
                                _ =>
                                {
                                    AnsiConsole.Markup($"[blue]{ResourceLoader.QueryLogo()}[/]");
                                }
                            ));
                });
        }
    }
}

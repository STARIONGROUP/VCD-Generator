// -------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Starion Group S.A.">
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

namespace VCD.Generator
{
    using System.CommandLine;
    using System.CommandLine.Help;
    using System.CommandLine.Invocation;
    using System.Linq;
    using System.Threading.Tasks;

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
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new GenerateCommand();

            rootCommand.SetAction(async (parseResult, cancellationToken) =>
            {
                using var host = Host.CreateDefaultBuilder(args)
                    .UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                        .ReadFrom.Configuration(context.Configuration))
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddSingleton<IRequirementsReader, RequirementsReader>();
                        services.AddSingleton<ITestResultReader, TestResultReader>();
                        services.AddSingleton<IMatchMaker, MatchMaker>();
                        services.AddSingleton<IReportGenerator, ReportGenerator>();
                        services.AddSingleton<GenerateCommand.Handler>();
                    })
                    .Build();

                var handler = host.Services.GetRequiredService<GenerateCommand.Handler>();
                rootCommand.BindTo(handler, parseResult);
                return await handler.InvokeAsync();
            });

            PrependLogoToHelp(rootCommand);

            return await rootCommand.Parse(args).InvokeAsync();
        }

        /// <summary>
        /// Replaces the default help action with one that prepends the ASCII logo.
        /// </summary>
        private static void PrependLogoToHelp(Command command)
        {
            var helpOption = command.Options.OfType<HelpOption>().FirstOrDefault();

            if (helpOption?.Action is SynchronousCommandLineAction defaultHelp)
            {
                helpOption.Action = new LogoHelpAction(defaultHelp);
            }
        }

        /// <summary>
        /// A <see cref="SynchronousCommandLineAction"/> that writes the ASCII logo before
        /// delegating to the default help action.
        /// </summary>
        private sealed class LogoHelpAction : SynchronousCommandLineAction
        {
            private readonly SynchronousCommandLineAction inner;

            public LogoHelpAction(SynchronousCommandLineAction inner)
            {
                this.inner = inner;
            }

            public override int Invoke(ParseResult parseResult)
            {
                AnsiConsole.Markup($"[blue]{ResourceLoader.QueryLogo()}[/]");
                return this.inner.Invoke(parseResult);
            }
        }
    }
}

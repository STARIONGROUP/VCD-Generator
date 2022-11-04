// -------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="RHEA System S.A.">
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

namespace VCD.Generator
{
    using System.CommandLine.Builder;
    using System.CommandLine.Hosting;
    using System.CommandLine.Parsing;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
 
    using Serilog;
    
    using VCD.Generator.Commands;
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
                .UseHost(_ => CreateHostBuilder(args), (builder) => builder
                    .UseSerilog()
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddTransient<IRequirementsReader, RequirementsReader>();
                        services.AddTransient<ITestResultReader, TestResultReader>();
                        services.AddTransient<IMatchMaker, MatchMaker>();
                        services.AddTransient<IReportGenerator, ReportGenerator>();
                    })
                    .UseDefaultServiceProvider((context, options) =>
                    {
                        options.ValidateScopes = true;
                    })
                    .UseCommandHandler<GenerateCommand, GenerateCommand.Handler>())
                .UseDefaults().Build();

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
            
            return new CommandLineBuilder(root);
        }

        /// <summary>
        /// Creates the default builder
        /// </summary>
        /// <param name="args">
        /// the commandline arguments
        /// </param>
        /// <returns>
        /// The <see cref="IHostBuilder"/>
        /// </returns>
        private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args);
    }
}

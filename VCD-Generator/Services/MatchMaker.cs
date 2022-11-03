// -------------------------------------------------------------------------------------------------
// <copyright file="MatchMaker.cs" company="RHEA System S.A.">
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

namespace VCD.Generator.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The purpose of the <see cref="MatchMaker"/> is to match the provided <see cref="TestCase"/> and
    /// <see cref="Requirement"/> objects
    /// </summary>
    public class MatchMaker : IMatchMaker
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<MatchMaker> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchMaker"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        public MatchMaker(ILoggerFactory loggerFactory = null)
        {
            this.logger = loggerFactory == null ? NullLogger<MatchMaker>.Instance : loggerFactory.CreateLogger<MatchMaker>();
        }

        /// <summary>
        /// Matches <see cref="TestCase"/> and <see cref="Requirement"/> objects.
        /// </summary>
        /// <param name="requirements">
        /// The subject <see cref="Requirement"/> objects that need to be matched
        /// </param>
        /// <param name="testCases">
        /// The subject <see cref="TestCase"/> objects that need to be matched
        /// </param>
        /// <remarks>
        /// The <see cref="Requirement.TestCases"/> property is updated with matched <see cref="TestCase"/> objects
        /// </remarks>
        public void Match(IEnumerable<Requirement> requirements, IEnumerable<TestCase> testCases)
        {
            foreach (var requirement in requirements)
            {
                foreach (var testCase in testCases)
                {
                    if (testCase.RequirementId.Contains(requirement.Identifier))
                    {
                        this.logger.LogDebug($"TestCase {testCase.FullName} matched to requirement {requirement.Identifier}");

                        requirement.TestCases.Add(testCase);
                    }
                }
            }
        }
    }
}

// -------------------------------------------------------------------------------------------------
// <copyright file="IMatchMaker.cs" company="Starion Group S.A.">
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

namespace VCD.Generator.Services
{
    using System.Collections.Generic;

    /// <summary>
    /// The purpose of the <see cref="IMatchMaker"/> is to match the provided <see cref="TestCase"/> and
    /// <see cref="Requirement"/> objects
    /// </summary>
    public interface IMatchMaker
    {
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
        void Match(IEnumerable<Requirement> requirements, IEnumerable<TestCase> testCases);
    }
}

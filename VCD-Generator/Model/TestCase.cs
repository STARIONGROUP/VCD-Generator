// -------------------------------------------------------------------------------------------------
// <copyright file="TestCase.cs" company="RHEA System S.A.">
// 
//   Copyright 2022-2023 RHEA System S.A.
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
    using System.Collections.Generic;

    /// <summary>
    /// Represents a <see cref="TestCase"/> in a test suite that is used to verify a <see cref="Requirement"/>
    /// </summary>
    public class TestCase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCase"/> class.
        /// </summary>
        public TestCase()
        {
            this.RequirementId = new List<string>();
        }

        /// <summary>
        /// Gets or sets the name of the test method (includes the 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the qualified name of the test method
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the human readable description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the test result (pass, fail, inconclusive)
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Gets or sets the list of requirement identifiers that this <see cref="TestCase"/> verifies
        /// </summary>
        public List<string> RequirementId { get; set; }
    }
}

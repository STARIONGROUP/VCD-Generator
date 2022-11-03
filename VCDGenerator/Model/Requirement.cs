// -------------------------------------------------------------------------------------------------
// <copyright file="Requirement.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;

    /// <summary>
    /// Represents a <see cref="Requirement"/> that is verified
    /// </summary>
    public class Requirement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Requirement"/> class.
        /// </summary>
        public Requirement()
        {
            this.Tests = new List<string>();
        }

        /// <summary>
        /// Gets or sets the human readable unique identifier
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the requirement text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the names of the tests
        /// </summary>
        public List<string> Tests { get; set; }
    }
}

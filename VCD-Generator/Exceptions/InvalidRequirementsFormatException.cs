﻿// -------------------------------------------------------------------------------------------------
// <copyright file="InvalidRequirementsFormatException.cs" company="RHEA System S.A.">
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
    using System;

    /// <summary>
    /// The <see cref="InvalidRequirementsFormatException"/> is thrown when the requirements data is in invalid format
    /// </summary>
    public class InvalidRequirementsFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequirementsFormatException"/> class.
        /// </summary>
        public InvalidRequirementsFormatException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequirementsFormatException"/> class with a specified
        /// error message and inner exception.
        /// </summary>
        /// <param name="message">
        /// The provided error message.
        /// </param>
        public InvalidRequirementsFormatException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequirementsFormatException"/> class with a specified
        /// error message and inner exception.
        /// </summary>
        /// <param name="message">
        /// The provided error message.
        /// </param>
        /// <param name="inner">
        /// The provided inner <see cref="Exception"/>
        /// </param>
        public InvalidRequirementsFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

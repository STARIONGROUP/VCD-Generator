// -------------------------------------------------------------------------------------------------
// <copyright file="TestResultReaderTestFixture.cs" company="RHEA System S.A.">
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

namespace VCD.Generator.Tests.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NUnit.Framework;

    using VCD.Generator.Services;

    /// <summary>
    /// Suite of tests for the <see cref="TestResultReader"/> class
    /// </summary>
    [TestFixture]
    public class TestResultReaderTestFixture
    {
        private TestResultReader testResultReader;

        private string path;

        [SetUp]
        public void SetUp()
        {
            this.path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Data");

            this.testResultReader = new TestResultReader();
        }

        [Test(Description = "Verifies that the TestResultReader.Read methods trows an exception"), Property("REQUIREMENT-ID", "REQ-01")]
        public void Verify_that_Read_throws_exception()
        {
            var testCases = this.testResultReader.Read(this.path).ToList();

            Assert.That(testCases.Count, Is.EqualTo(2));

            var testCase = testCases.Single(x => x.FullName== "VCD.Generator.Tests.Services.TestResultReaderTestFixture.Verify_that_Read_throws_exception");

            Assert.That(testCase.Name, Is.EqualTo("Verify_that_Read_throws_exception"));
            Assert.That(testCase.Description, Is.EqualTo("Verifies that the TestResultReader.Read methods trows an exception"));
            Assert.That(testCase.Result, Is.EqualTo("Passed"));
            Assert.That(testCase.RequirementId, Is.EquivalentTo(new List<string> { "REQ-01" }));
        }
    }
}

# VCD-Generator
The purpose of the VCD Generator is to create Verification Control Documents (reports) that make record of which tests contribute to the verification of a set of Requirements.

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_VCD-Generator&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_VCD-Generator)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_VCD-Generator&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_VCD-Generator)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_VCD-Generator&metric=coverage)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_VCD-Generator)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_VCD-Generator&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_VCD-Generator)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_VCD-Generator&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_VCD-Generator)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_VCD-Generator&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_VCD-Generator)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_VCD-Generator&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_VCD-Generator)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_VCD-Generator&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_VCD-Generator)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_VCD-Generator&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_VCD-Generator)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_VCD-Generator&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_VCD-Generator)

The VCD Generator works in combination with NUnit and currently generates reports in 2 forms:
  - Excel Spreadsheet
  - HTML Table

The VCD Generator needs 2 inputs:
  - Requirements: in tabular form (excel spreadsheet), where one of the columns needs to contain the human readable unique identifier of the requirements.
  - Nunit results: in xml form generated using the `NunitXml.TestLogger` logger

> **Note**
> The following test command needs to be run to generate the required output

```
dotnet test VCD-Generator.sln --logger:"nunit;LogFilePath=TestResults/{assembly}.Result.xml" -- NUnit.ShowInternalProperties=true
```

The NUnit tests that need to be part of the VCD process need to be annotated with a C# attribute that is part of the NUnit framework. Have a look at the following example:

```
[TestFixture]
public class ACoolTestFixture()
{
  [Test(Description = "a very fancy test"), Property("REQUIREMENT-ID", "REQ-01")]
  public void Some_fance_test()
  {
    // ... test test test
  }
} 
```

> **Note**
> Make use of the `Property` attribute which provides a key-value pair. For the VCD-Generator to work provide at least the following `Property("REQUIREMENT-ID", "your-requirement-id")`

When the `REQUIREMENT-ID` is matched to a requirement in the input requirements spreadsheet the report will inlcude the testcase results in the report.

## Build Status

GitHub actions are used to build and test the library

Branch | Build Status
------- | :------------
Master | ![Build Status](https://github.com/RHEAGROUP/VCD-Generator/actions/workflows/CodeQuality.yml/badge.svg?branch=master)
Development | ![Build Status](https://github.com/RHEAGROUP/VCD-Generator/actions/workflows/CodeQuality.yml/badge.svg?branch=development)

# License

The VCD-Generator libraries are provided to the community under the Apache License 2.0.

# Contributions

Contributions to the code-base are welcome. However, before we can accept your contributions we ask any contributor to sign the Contributor License Agreement (CLA) and send this digitaly signed to s.gerene@rheagroup.com. You can find the CLA's in the CLA folder.

# VCD-Generator

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

The purpose of the VCD Generator is to create Verification Control Documents (reports) that make record of which tests contribute to the verification of a set of Requirements. The VCD Generator is available on [Nuget](https://www.nuget.org/packages/vcdg) as a dotnet tool called `vcdg`.

The VCD Generator works in combination with NUnit and currently generates reports in 2 forms:
  - Excel Spreadsheet
  - HTML Table

The VCD Generator needs 2 inputs:
  - Requirements: in tabular form (excel spreadsheet), where one of the columns needs to contain the human readable unique identifier of the requirements.
  - Nunit results: in xml form generated using the `NunitXml.TestLogger` logger

Please read the [Quick Start](https://github.com/RHEAGROUP/VCD-Generator/wiki/Quick-Start) to find out how to use the `vcdg` dotnet tool

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
# VCD-Generator
The purpose of the VCD Generator is to create Verification Control Documents (reports) that make record of which tests contribute to the verification of a set of Requirements.

The VCD Generator works in combination with NUnit and currently generates reports in 2 forms:
  - Excel Spreadsheet
  - HTML Table

The VCD Generator needs 2 inputs:
  - Requirements: in tabular form (excel spreadsheet), where one of the columns needs to contain the human readable unique identifier of the requirements.
  - Nunit results: in xml form generated using the `NunitXml.TestLogger` logger

NOTE > The following test command needs to be run to generate the required output

'''
dotnet test VCD-Generator.sln --logger:"nunit;LogFilePath=TestResults/{assembly}.Result.xml" -- NUnit.ShowInternalProperties=true
'''
# CallDetailRecordPlatform
A small REST API for operating data about customer calls.

# Technologies
I've decide to use SQLite with EF Core for the persistence layer. 
The API is built with ASP.NET Core with the minimal API pattern.
Both of these decisions were based on the fact, that I already once used this stack.
Testing is done with xUnit alone, without any mocking framework.

# ToDo list:
- editor config
- semantic versioning
- Migration schema fro DB
- Extract DTOs to separate project and publish it
- Extract abstractions to separate project
- Introduce service layer for handling data - removes tight coupling between endpoints and DB (also enables better testing)
- Figure out deployment - possible in docker and straight to K8
- Add input validation logic to all endpoints and data loading
- Describe how faulty data is handled and actually implement how the app behaves by it
- Solve better logging, maybe do connection to Elastic or similar for gathering data
- Tracing would be nice as well
- Have some logical parts configurable via appsettings.json and have them deployed to ProgramData (for example)
- Prepare the app for OpenAPI so the .NET upgrade is smoother

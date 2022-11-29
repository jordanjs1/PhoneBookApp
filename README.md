# The Phone Book Application
The Phone Book Application is a simple phone book app written in C# .NET 6 for self-education on microservices architectures purposes. Heavily influenced on Microsoft's eShop example.

## Setting the Development Environment on Windows
To run The Phone Book, you need the following technologies to be available to the computer you will develop and run The Phone Book:
- .NET 6 SDK (to compile and run the app)
- PostgreSQL (used as the database)
- RabbitMQ (used as the message broker)

### Installing Dependencies
If you are setting a local-only development environment, do the following:
1. Install the latest version of .NET 6 SDK.
1. Optionally install a version of Visual Studio 2022 that can run the latest .NET 6 SDK.
1. Install PostgreSQL.
1. Install RabbitMQ. Note that you may also need to install Chocolatey based on your choice of RabbitMQ installation style.

### Configuring the Development Environment
The Phone Book is preconfigured for local-only development environment, which means that PostgreSQL and RabbitMQ both look for their servers in `localhost`. However, with proper networking setup, the app can use non-local database and RabbitMQ servers.

If you are setting a local-only development environment, do the following:
1. Create a PostgreSQL server in your computer and start the server. Make sure that the server runs on `localhost`, port `5432`.
1. Create two users with login and create database permissions. Refer to the APIs' `appsettings.json` files for users' login information.
1. To create the databases and tables, apply the preconfigured migrations by either uncommenting the migration code in APIs' `Program.cs` files or use the following commands in order:
    - For `Contact.Api`, use `Update-Database -Context BookContext`.
    - For `Reporting.Api`, use `Update-Database -Context ReportingContext`.

After these steps, you should be ready to write code.

## Running The Phone Book
Plase note that `Reporting.Api` makes calls to `Contact.Api` by sending HTTP requests to `http://localhost:14560`. Therefore, make sure that `Contact.Api` is reachable from that address.

There are two common ways to run the APIs:
- If Visual Studio is present in your development environment, you can use Visual Studio.
- You can use `dotnet run --project ./Src/Services/Contact/Contact.Api` and `dotnet run --project ./Src/Services/Reporting/Reporting.Api` commands on a shell.

Before running the projects, make sure that your PostgreSQL and RabbitMQ servers are running and reachable.

## Notes
- The app is currently missing a lot of documentation. The documentation will be filled in later.
- The app currently doesn't have unit tests. They are planned to be made later.
- The app doesn't have logging apart from Microsoft's own logging systems in the libraries the app depends on. A proper logging with Serilog is planned.
- Some of the app's code and configuration files (looking at you, `appsettings.json` files!) may need readability and/or performance optimizations. Some work will be done on this later.
- More advanced technologies (like gRPC) and patterns (like CQRS) are planned to be implemented if the app grows up.
- The API projects were created with Docker support, although they have no configuration to allow them to work in a Docker container.
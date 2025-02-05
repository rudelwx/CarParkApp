# Car Park App

## Database Setup

1. Using `carParkAppDB.sql` to restore the database to MySQL 9.2.0.

## Application Setup

1. Update the `DefaultConnection` in `appsettings.json` with your MySQL connection details.

## Publishing the Project

To publish the project, run:

```sh
 dotnet publish
```

## Unit Testing

1. Update the database connection string on **line 18** in `carParkApp.Tests/ApiControllerTests`.
2. To run unit tests, execute:

```sh
 dotnet test
```

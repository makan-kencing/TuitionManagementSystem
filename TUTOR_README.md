Tutor README
============

TLDR
----

- Download .NET 9.0 with ASP.NET Core framework and Docker
- Run
  ```bash
  docker compose up -f docker-compose.yaml -f docker-compose.dev.yaml -d
  dotnet ef database update --project src/TuitionManagementSystem.Web/TuitionManagementSystem.Web.csproj
  dotnet run --project  src/TuitionManagementSystem.Web/TuitionManagementSystem.Web.csproj --launch-profile prod
  ```
- Go to http://localhost:8080.
- Login with `student1` to `student20` with password `123456` \
  or `admin1` to `admin3` with password `123456` \
  or `teacher1` to `teacher4` with password `123456` \
  or `parent1` to `parent8` with password `123456`.
  (minus the backticks)









Requirements
------------

The project utilizes two tools to run the application

- [.NET 9.0](https://dotnet.microsoft.com/en-us/download) with [ASP.NET Core framework](https://dotnet.microsoft.com/en-us/apps/aspnet)
- [Docker](https://www.docker.com/)

Before running
--------------

### Database

The database and other services are deployed using docker.

Run the following command to run the services

```cmd
docker compose up -f docker-compose.yaml -f docker-compose.dev.yaml -d
```

### Seeding database

To seed the database with sample data, run the dotnet-ef migrations to update the database.

```cmd
dotnet ef database update --project src/TuitionManagementSystem.Web/TuitionManagementSystem.Web.csproj
```

Running the project
-------------------

After setting up the project, the project can be run with the following command

```cmd
dotnet run --project  src/TuitionManagementSystem.Web/TuitionManagementSystem.Web.csproj --launch-profile prod
```

Using the website
-----------------

By default the webpage will appear at http://localhost:8080.

<small>https://localhost may also work.</small>

### Sample login credentials

The sample data contains login credentials for 33 users
- 3 admins
- 4 teachers
- 20 students
- 8 parents

The given login credentials follows the format of `{role}{n}` with the password of `123456`

For example,

> Username: student1
> 
> Password: 123456

> Username: admin3
> 
> Password: 123456


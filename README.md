## Instructions
Create appsettings.json in the Server project with the following info:
```
{
  "ConnectionStrings": {
    "PostgresConnection": "Host=####;Port=####;Username=####;Password=####;Database=####;"
  }
}
```

## Server Dependencies
`dotnet add Server package Npgsql`

`dotnet add Server package Microsoft.Extensions.Configuration`

`dotnet add Server package Microsoft.Extensions.Configuration.Json`

`dotnet add Server package Microsoft.Extensions.Configuration.EnvironmentVariables`

`dotnet add Server package Microsoft.Extensions.Configuration.Binder`

`dotnet add Modules package Newtonsoft.Json`
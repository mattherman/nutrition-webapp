# Nutrition Calculator

This is a simple nutrition calculator application. It is built in F# using the [SAFE Stack](https://safe-stack.github.io/) (Saturn/Fable/Elmish) and uses the [Nutritionix](https://www.nutritionix.com/) API for nutrition information.

## Install pre-requisites

You'll need to install the following pre-requisites in order to build the application.

* [.NET SDK](https://www.microsoft.com/net/download) 8.0 or higher
* [Node 18](https://nodejs.org/en/download/) or higher
* [NPM 9](https://www.npmjs.com/package/npm) or higher

## Starting the application

Before you run the project **for the first time only** you must install dotnet "local tools" with this command:

```bash
dotnet tool restore
```

To concurrently run the server and the client components in watch mode use the following command:

```bash
dotnet run
```

Then open `http://localhost:8080` in your browser.

The build project in root directory contains a couple of different build targets. You can specify them after `--` (target name is case-insensitive).

To run concurrently server and client tests in watch mode (you can run this command in parallel to the previous one in new terminal):

```bash
dotnet run -- RunTests
```

Client tests are available under `http://localhost:8081` in your browser and server tests are running in watch mode in console.

Finally, there are `Bundle` and `Azure` targets that you can use to package your app and deploy to Azure, respectively:

```bash
dotnet run -- Bundle
dotnet run -- Azure
```

## Secrets Configuration

The application expects two secrets to be configured at `NutritionApi:AppId` and `NutritionApi:AppKey`. There are a couple different ways to do so for local development.

### User Secrets

The simplest is to use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=linux).

Using the .NET SDK you can set the user secrets like this:
```
dotnet user-secrets set "NutritionApi:AppId" "<secret>"
dotnet user-secrets set "NutritionApi:AppKey" "<secret>"
```

### Environment Variables

You can also specify the secrets using environment variables, which is how the application is configured when it is deployed. The environment variables will be in the format `NutritionApi__AppId` and `NutritionApi__AppKey`.

### `appsettings.json` / `launch.json`

**Danger** - If you need a quick way to configure the secrets you can add them directly to the `appsettings.json` file or as environment variables in `.vscode/launch.json`, but take extra care to **remove them before committing** since these files are included in the repository.

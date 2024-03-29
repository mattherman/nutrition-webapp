module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open Microsoft.Extensions.DependencyInjection

open Shared
open Microsoft.AspNetCore.Http
open System
open System.Net.Http
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting

let searchFoods (nutritionApi: NutritionApi.Api) query = async {
    let! searchResponse = nutritionApi.SearchFood query

    let commonFoods =
        Array.map
            (fun (f: NutritionApi.CommonFoodResponse) -> {
                Description = f.FoodName
                Photo = f.Photo.Thumb
            })
            searchResponse.Common

    let brandedFoods =
        Array.map
            (fun (f: NutritionApi.BrandedFoodResponse) -> {
                Description = f.FoodName
                Photo = f.Photo.Thumb
            })
            searchResponse.Branded

    return Array.concat [ commonFoods; brandedFoods ] |> Array.toList
}

let createFoodApi (context: HttpContext) =
    let nutritionApi = context.GetService<NutritionApi.Api>()

    {
        searchFoods = searchFoods nutritionApi
    }

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromContext createFoodApi
    |> Remoting.buildHttpHandler

let configureHost (hostBuilder: IHostBuilder) =
    hostBuilder.ConfigureAppConfiguration(fun _ configBuilder -> configBuilder.AddUserSecrets() |> ignore)

let configureServices (services: IServiceCollection) =
    let nutritionApiConfig =
        (Config.getConfiguration services).GetSection("NutritionApi")

    let nutritionApiBaseAddress = nutritionApiConfig["BaseUrl"]
    let nutritionApiAppKey = nutritionApiConfig["AppKey"]
    let nutritionApiAppId = nutritionApiConfig["AppId"]

    services.AddTransient<NutritionApi.Api>() |> ignore

    services.AddHttpClient<NutritionApi.Api>(
        Action<HttpClient>(fun client ->
            client.BaseAddress <- Uri(nutritionApiBaseAddress)
            client.DefaultRequestHeaders.Add("x-app-id", nutritionApiAppId)
            client.DefaultRequestHeaders.Add("x-app-key", nutritionApiAppKey))
    )
    |> ignore

    services


let app = application {
    use_router webApp
    memory_cache
    use_static "public"
    use_gzip
    host_config configureHost
    service_config configureServices
}

[<EntryPoint>]
let main _ =
    run app
    0
module NutritionApi

open System.Net.Http
open System.Net.Http.Json
open System.Text.Json.Serialization
open System

type Photo = { Thumb: Uri }

type CommonFoodResponse = {
    [<JsonPropertyName("food_name")>]
    FoodName: string
    Photo: Photo
}

type BrandedFoodResponse = {
    [<JsonPropertyName("food_name")>]
    FoodName: string
    Photo: Photo
}

type SearchResponse = {
    Common: CommonFoodResponse[]
    Branded: BrandedFoodResponse[]
}

type Api(httpClient: HttpClient) =
    member _.SearchFood(query: string) =
        task {
            let! result = httpClient.GetFromJsonAsync<SearchResponse>($"v2/search/instant/?query={query}")
            return result
        }
        |> Async.AwaitTask
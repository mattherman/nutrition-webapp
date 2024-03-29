module Index

open Elmish
open Fable.Remoting.Client
open Shared

type Page =
    | Calculator
    | Search
    | Add

type Model = {
    CurrentPage: Page
    Food: Food list
    SearchResults: Food list
    SearchInput: string
    SearchLoading: bool
}

type Msg =
    | GoToPage of Page
    | StartSearch
    | SetSearchInput of string
    | SearchFood
    | GotSearchResults of Food list
    | AddFood of Food
    | RemoveFood of Food

let foodApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IFoodApi>

let init () =
    let model = {
        CurrentPage = Calculator
        Food = []
        SearchResults = []
        SearchInput = ""
        SearchLoading = false
    }

    model, Cmd.none

let update msg model =
    match msg with
    | GoToPage page -> { model with CurrentPage = page }, Cmd.none
    | StartSearch ->
        {
            model with
                SearchInput = ""
                SearchResults = []
                SearchLoading = false
        },
        Cmd.ofMsg (GoToPage Search)
    | SetSearchInput value -> { model with SearchInput = value }, Cmd.none
    | SearchFood ->
        let query = model.SearchInput
        let cmd = Cmd.OfAsync.perform foodApi.searchFoods query GotSearchResults

        {
            model with
                SearchInput = ""
                SearchLoading = true
        },
        cmd
    | GotSearchResults food ->
        {
            model with
                SearchResults = food
                SearchLoading = false
        },
        Cmd.none
    | AddFood food ->
        {
            model with
                Food = model.Food @ [ food ]
        },
        Cmd.ofMsg (GoToPage Calculator)
    | RemoveFood food ->
        let updated = model.Food |> List.filter (fun f -> not (f = food))
        { model with Food = updated }, Cmd.none

open Feliz

let calculatorFoodListEmpty () =
    Html.div [
        prop.className "w-full h-full flex items-center justify-center text-xl text-slate-800 py-12"
        prop.children [ Html.text "Add food to get started..." ]
    ]

let calculatorFoodList (model: Model) dispatch =
    Html.div [
        prop.className "flex flex-col"
        prop.children [
            for food in model.Food do
                Html.div [
                    prop.className "flex flex-row items-center mt-4 gap-4 h-16 px-2 shadow border rounded"
                    prop.children [
                        Html.div [
                            prop.className "w-12 h-12 flex items-center"
                            prop.children [ Html.img [ prop.className "max-h-12"; prop.src (food.Photo.ToString()) ] ]
                        ]
                        Html.span [ prop.className "text-xl flex items-center"; prop.text food.Description ]
                        Html.button [
                            prop.className "flex items-center pr-2 ml-auto text-red-800"
                            prop.text "Remove"
                            prop.onClick (fun _ -> dispatch (RemoveFood food))
                        ]
                    ]
                ]
        ]
    ]

let calculator (model: Model) dispatch =
    Html.div [
        Html.button [
            prop.className "w-full md:w-24 h-10 mt-4 shadow border rounded bg-cyan-300 text-slate-800"
            prop.text "Add Food"
            prop.onClick (fun _ -> dispatch StartSearch)
        ]
        match model.Food with
        | [] -> calculatorFoodListEmpty ()
        | _ -> calculatorFoodList model dispatch
    ]

let searchResults (model: Model) dispatch =
    if model.SearchLoading then
        Html.div [
            prop.className "mt-4 text-xl text-slate-600"
            prop.children [ Html.text "Loading..." ]
        ]
    else
        Html.div [
            for food in model.SearchResults do
                Html.div [
                    prop.className
                        "flex flex-row items-center mt-4 gap-4 h-16 px-2 shadow border rounded cursor-pointer"
                    prop.onClick (fun _ -> dispatch (AddFood food))
                    prop.children [
                        Html.div [
                            prop.className "w-12 h-12 flex items-center"
                            prop.children [ Html.img [ prop.className "max-h-12"; prop.src (food.Photo.ToString()) ] ]
                        ]
                        Html.span [ prop.className "text-xl flex items-center"; prop.text food.Description ]
                    ]
                ]
        ]

let searchFood (model: Model) dispatch =
    Html.div [
        prop.className "flex flex-col"
        prop.children [
            Html.button [
                prop.type' "button"
                prop.className "shadow border rounded w-24 h-10 mt-4 px-4 bg-gray-300 text-slate-800"
                prop.text "< Back"
                prop.onClick (fun _ -> dispatch (GoToPage Calculator))
            ]
            Html.h1 [ prop.className "text-2xl text-slate-800 my-3"; prop.text "Search Food" ]
            Html.div [
                prop.className "flex flex-row gap-2"
                prop.children [
                    Html.input [
                        prop.className
                            "shadow appearance-none border rounded w-full py-2 px-3 outline-none focus:ring-2 ring-cyan-300 text-grey-darker"
                        prop.value model.SearchInput
                        prop.placeholder "ex. hamburger"
                        prop.autoFocus true
                        prop.onChange (SetSearchInput >> dispatch)
                        prop.onKeyPress (fun ev ->
                            if ev.key = "Enter" then
                                dispatch SearchFood)
                    ]
                    Html.button [
                        prop.type' "button"
                        prop.className "shadow border rounded h-10 w-24 px-4 bg-cyan-300 text-slate-800"
                        prop.text "Search"
                        prop.onClick (fun _ -> dispatch SearchFood)
                    ]
                ]
            ]
            searchResults model dispatch
        ]
    ]

let view (model: Model) dispatch =
    Html.section [
        prop.className "container mx-auto md:w-2/3 w-full px-4"
        prop.children [
            Html.h1 [ prop.className "text-4xl mt-4"; prop.text "Nutrition Calculator" ]
            match model.CurrentPage with
            | Search -> searchFood model dispatch
            | _ -> calculator model dispatch
        ]
    ]
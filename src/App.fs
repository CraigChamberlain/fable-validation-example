module App.View

open Elmish
open Fable.React
open Fable.React.Props
open Fulma
open Fable.FontAwesome
open Fable.Validation.Core

type Model =
    { 
      Value : string 
      ValueValidationErrors : string List
    }

type Msg =
    | ChangeValue of string
    | UpdateValidationErrors

let init _ = { Value = "" ; ValueValidationErrors = []}, Cmd.none

let validateValue value = 
      all <| fun t ->
          t.Test "Value" value // call `t.Test fieldName value` to initialize field state
                |> t.Trim // pipe the field state to rules
                |> t.IsValid (Seq.head >> System.Char.IsUpper) "A name should start with a captial" // custom validation
                |> t.Match (System.Text.RegularExpressions.Regex(".[a-z]")) "A should have some lowercase charactors" // regex validation
                |> t.NotBlank "name cannot be blank" // rules can contain params and a generic error message
                |> t.MaxLen 20 "maxlen is {len}"
                |> t.MinLen 3 "minlen is {len}"
                |> t.End // call `t.End` to unwrap the validated
                         // and transformed value,
                         // you can use the transformed values to create a new model


let private update msg model =
    match msg with
    | ChangeValue newValue ->
        { model with Value = newValue }, Cmd.ofMsg UpdateValidationErrors
    | UpdateValidationErrors -> 
        let result = validateValue model.Value
        match result with 
        | Ok _ -> 
          { model with ValueValidationErrors = [] }, Cmd.none
        | Error (result:Map<string,string list>) ->  
          { model with ValueValidationErrors = result.["Value"] }, Cmd.none


let private nameInput model dispatch=

       Field.div [ ]
            [ Label.label [ ]
                [ str "Username" ]
              Control.div [ Control.HasIconLeft
                            Control.HasIconRight ]
                [ Input.text [ Input.OnChange (fun ev -> dispatch (ChangeValue ev.Value))
                               Input.Value model.Value
                               Input.Props [ AutoFocus true ]
                               match model.ValueValidationErrors with
                               | [] -> Input.Color IsSuccess
                               | _ -> Input.Color IsDanger
                              ]
                  Icon.icon [ Icon.Size IsSmall; Icon.IsLeft ]
                    [ Fa.i [ Fa.Solid.User ]
                        [ ] ]
                  Icon.icon [ Icon.Size IsSmall; Icon.IsRight ]
                    [ Fa.i [ 
                              match model.ValueValidationErrors with
                               | [] -> Fa.Solid.Check
                               | _ -> Fa.Solid.ExclamationTriangle
                         ]
                        [ ] ] ]
              
              match model.ValueValidationErrors with
              | [] -> 
                  Help.help  [ Help.Color IsSuccess ] [ str "This username is valid" ] 
              | errorList -> 
                  let errorListHtml = ul [] (errorList |> List.map (fun e -> li [ ] [ str e ]) )
                  Help.help  [ Help.Color IsDanger ] [errorListHtml]
            ]

let private view model dispatch =
    Hero.hero [ Hero.IsFullHeight ]
        [ Hero.body [ ]
            [ Container.container [ ]
                [ Columns.columns [ Columns.CustomClass "has-text-centered" ]
                    [ Column.column [ Column.Width(Screen.All, Column.IsOneThird)
                                      Column.Offset(Screen.All, Column.IsOneThird) ]
                        [ Image.image [ Image.Is128x128
                                        Image.Props [ Style [ Margin "auto"] ] ]
                            [ img [ Src "assets/fulma_logo.svg" ] ]
                          nameInput model dispatch                  
                          Content.content [ ]
                            [ str "Hello, "
                              str model.Value
                              str " "
                              Icon.icon [ ]
                                [ Fa.i [ Fa.Regular.Smile ]
                                    [ ] ] ] ] ] ] ] ]

open Elmish.Debug
open Elmish.HMR

Program.mkProgram init update view
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run

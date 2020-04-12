module App.View

open Elmish
open Fable.React
open Fable.React.Props
open Fulma
open Fable.FontAwesome
open Fable.Validation.Core


type ValidModel = {
        email : string
        password : string
      } with
      static member Email = "Email"
      static member Password = "Password"

type Model =
    {
      Email : string
      Password : string
      ValidationErrors : Map<string, string List>
      ModalState: bool
}

type Msg =
    | ChangeEmail of string
    | ChangePassword of string
    | UpdateValidationErrors
    | Submit
    | Clear
    | ToggleModal

let init _ = { Email = ""; Password = "" ; ValidationErrors = Map.empty; ModalState = false}, Cmd.ofMsg UpdateValidationErrors

let validateModel model =
      fast <| fun t ->
        {
          email =     t.Test ValidModel.Email model.Email
                      |> t.Trim
                      |> t.IsMail "Should be a valid email"
                      |> t.End

          password =  t.Test ValidModel.Password model.Password // call `t.Test fieldName value` to initialize field state
                      |> t.Trim // pipe the field state to rules
                      |> t.IsValid (Seq.head >> System.Char.IsUpper) "A Password should start with a captial" // custom validation
                      |> t.Match (System.Text.RegularExpressions.Regex(".[a-z]")) "A should have some lowercase charactors" // regex validation
                      |> t.NotBlank "Password cannot be blank" // rules can contain params and a generic error message
                      |> t.MaxLen 20 "Max lenght is {len}"
                      |> t.MinLen 8 "Min length is {len}"
                      |> t.End // call `t.End` to unwrap the validated
                               // and transformed value,
                               // you can use the transformed values to create a new model
        }

let private update msg model =
    match msg with
    | ChangePassword newPassword ->
        { model with Password = newPassword }, Cmd.ofMsg UpdateValidationErrors
    | ChangeEmail newEmail ->
        { model with Email = newEmail }, Cmd.ofMsg UpdateValidationErrors
    | UpdateValidationErrors ->
        let result = validateModel model
        match result with
        | Ok _ ->
          { model with ValidationErrors = Map.empty }, Cmd.none
        | Error result ->
          { model with ValidationErrors = result}, Cmd.none
    | Submit ->
        { model with ModalState = not model.ModalState }, Cmd.none
    | Clear ->
        init()
    | ToggleModal ->
        { model with ModalState = not model.ModalState }, Cmd.none

// Exceptions are burried inside the view and so this should empty map should be guarded against.
let guardEmptyMap key unsafeMap =
    if Map.isEmpty unsafeMap || not (Map.containsKey key unsafeMap)
    then []
    else unsafeMap.[key]

let private emailInput model dispatch =

  let errors = guardEmptyMap ValidModel.Email model.ValidationErrors

  Field.div [ ]
            [ Label.label [ ]
                [ str ValidModel.Email ]
              Control.div [ Control.HasIconLeft
                            Control.HasIconRight ]
                [ Input.email [ Input.OnChange (fun ev -> dispatch (ChangeEmail ev.Value))
                                Input.Value model.Email
                                match errors with
                                | [] -> Input.Color IsSuccess
                                | _ -> Input.Color IsDanger
                               ]
                  Icon.icon [ Icon.Size IsSmall; Icon.IsLeft ]
                    [ Fa.i [ Fa.Solid.Envelope ]
                        [ ] ]
                  Icon.icon [ Icon.Size IsSmall; Icon.IsRight ]
                    [ Fa.i [ match errors with
                               | [] -> Fa.Solid.Check
                               | _ -> Fa.Solid.ExclamationTriangle ]
                        [ ] ] ]
              match errors with
              | [] ->
                  Help.help  [ Help.Color IsSuccess ] [ str "This Email is valid" ]
              | errorList ->
                  let errorListHtml = ul [] (errorList |> List.map (fun e -> li [ ] [ str e ]) )
                  Help.help  [ Help.Color IsDanger ] [errorListHtml]
            ]

let private passwordInput model dispatch =
       let errors = guardEmptyMap ValidModel.Password model.ValidationErrors
       Field.div [ ]
            [ Label.label [ ]
                [ str ValidModel.Password ]
              Control.div [ Control.HasIconLeft
                            Control.HasIconRight ]
                [ Input.password
                          [
                             Input.OnChange (fun ev -> dispatch (ChangePassword ev.Value))
                             Input.Value model.Password
                             Input.Props [ AutoFocus true ]
                             match errors with
                             | [] -> Input.Color IsSuccess
                             | _ -> Input.Color IsDanger
                          ]
                  Icon.icon [ Icon.Size IsSmall; Icon.IsLeft ]
                    [ Fa.i [ Fa.Solid.User ]
                        [ ] ]
                  Icon.icon [ Icon.Size IsSmall; Icon.IsRight ]
                    [ Fa.i [
                              match errors with
                               | [] -> Fa.Solid.Check
                               | _ -> Fa.Solid.ExclamationTriangle
                         ]
                        [ ] ] ]


              match errors with
              | [] ->
                  Help.help  [ Help.Color IsSuccess ] [ str "This Password is valid" ]
              | errorList ->
                  let errorListHtml = ul [] (errorList |> List.map (fun e -> li [ ] [ str e ]) )
                  Help.help  [ Help.Color IsDanger ] [errorListHtml]
            ]


// Render the modal
let basicModal  model dispatch =
    Modal.modal [ Modal.IsActive model.ModalState ]
        [ Modal.background [ Props [ OnClick (fun _ -> dispatch ToggleModal) ] ] [ ]
          Modal.content [ ]
            [ Box.box' [ ]
                [
                  Heading.h2 [] [str "Email:"]
                  str model.Email
                  Heading.h2 [] [str "Password:"]
                  str model.Password  ] ]
          Modal.close [ Modal.Close.Size IsLarge
                        Modal.Close.OnClick (fun _ -> dispatch ToggleModal) ] [ ] ]

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
                          form [ ]
                              [ emailInput model dispatch
                                passwordInput model dispatch
                                // Control area (submit, cancel, etc.)
                                Field.div [ Field.IsGrouped ]
                                  [ Control.div [ ]
                                      [ Button.button
                                          [
                                            Button.Color IsPrimary
                                            Button.Props
                                              [ Type "button"
                                                OnClick (fun _ -> dispatch Submit)
                                                Disabled <| not (Map.isEmpty model.ValidationErrors)
                                              ]
                                            ]
                                          [ str "Submit" ]
                                       ]
                                    div [ ]
                                      [ basicModal model dispatch ]
                                    Control.div [ ]
                                      [ Button.button [
                                          Button.IsLink
                                          Button.Props
                                              [ Type "button"
                                                OnClick (fun _ -> dispatch Clear)
                                              ]
                                          ]
                                          [ str "Clear" ] ] ]


        ] ] ] ] ] ]

open Elmish.Debug
open Elmish.HMR

Program.mkProgram init update view
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run

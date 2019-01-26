namespace ModernRonin.LastFmToSpotify.Console

open ModernRonin.LastFmToSpotify

type Message = Start 
                | Exit
                | PickUser
                | UserPicked 
                | PickTag 
                | ConvertTag 
                | AskWhatNext 
                | AuthorizeSpotify
                | ReAuthorizeSpotify


type SharedState= { UserName: string 
                    AvailableTags: TagInfo list 
                    SelectedTag: TagInfo option
                    SpotifyAuthorization: SpotifyAuthData option
                    } with
    static member Initial = {UserName= ""; AvailableTags= []; SelectedTag= None; SpotifyAuthorization= None}


module StateMachine=
    open System
    open Actions
    open ModernRonin.ConsoleUi.Interaction

             
    type private DispatchResult = ExitCode of int | ChangeState of Message*SharedState | Notify of Message


    let private dispatch msg state = 
        match msg with
        | Exit -> ExitCode 0
        | Start -> 
            welcome() 
            Notify PickUser

        | PickUser -> 
            match pickUser() with
            | None -> Notify Exit
            | Some u  -> ChangeState (UserPicked, {state with UserName=u})

        | UserPicked -> 
            match retrieveTags state.UserName with
            | [] -> Notify PickUser
            | _ as ts -> ChangeState (PickTag, {state with AvailableTags=ts})

        | PickTag -> 
            match pickTag state.AvailableTags with
            | None -> Notify PickUser
            | Some _ as t -> ChangeState (ConvertTag, {state with SelectedTag= t})

        | ConvertTag -> 
            match state.SpotifyAuthorization with
            | None -> Notify AuthorizeSpotify
            | Some {AccessToken=_; ExpiryTime=t} when t<=DateTime.Now -> Notify ReAuthorizeSpotify
            | _ -> 
                convertUserTag state.UserName state.SelectedTag.Value state.SpotifyAuthorization.Value
                Notify AskWhatNext

        | AskWhatNext -> 
            match indexOfUserChoice ["Exit"; "Another tag"; "Another user"] with
            | Some 0 -> Notify Exit
            | Some 1 -> Notify PickTag
            | Some 2 -> Notify PickUser
            | Some _
            | None   -> Notify AskWhatNext 

        | AuthorizeSpotify -> ChangeState (ConvertTag, {state with SpotifyAuthorization= authorizeSpotify()})

        | ReAuthorizeSpotify ->
            printfn "My authorization for Spotify has expired. You need to authorize me again, please."
            ChangeState (ConvertTag, {state with SpotifyAuthorization= authorizeSpotify()})


    let rec run (msg, state)=
        match dispatch msg state with
        | ExitCode c -> c
        | ChangeState (m,s) -> run (m,s)
        | Notify m -> run (m, state)

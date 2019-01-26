namespace ModernRonin.LastFmToSpotify

open System


type AuthorizationResult = AuthorizationSuccess of SpotifyAuthData | AuthorizationFailure | UserError
and SpotifyAuthData = {AccessToken: string; ExpiryTime: DateTime}

module SpotifyApi=

    open ModernRonin.FsCore.Strings
    open ModernRonin.FsCore.Parsing

    let mutable private apiKey= ""

    let redirectUri= "http://localhost/sp"

    let private authorizeUrlFormat= sprintf "https://accounts.spotify.com/authorize?client_id=%s&redirect_uri=%s&scope=%s&response_type=token"


    let private urlEncode s = Uri.EscapeDataString s

    let private formatRedirectUri =  redirectUri |> urlEncode

    let private formatScopes (scopes: string list) = String.Join (" ", scopes) |> urlEncode

    let private scopes = ["playlist-modify-private"] |> formatScopes



    let setApiKey key = apiKey <- key

    let authorizationUrl() = authorizeUrlFormat apiKey formatRedirectUri scopes

    let parseAuthorizationReply url= 
        let successPrefix= redirectUri + "#"
        let errorPrefix= redirectUri + "?error="
        match url with 
        | Start errorPrefix _ -> AuthorizationFailure
        | Start successPrefix queryString ->
            let arguments= queryStringToMap queryString
            match (arguments.TryFind "access_token", arguments.TryFind "expires_in") with
            | None, _
            | _, None  -> UserError
            | Some token, Some expiry -> 
                match expiry with
                | Int seconds -> AuthorizationSuccess {AccessToken=token; ExpiryTime= seconds |> float |> DateTime.Now.AddSeconds}
                | _ -> UserError
        | _ -> UserError

        
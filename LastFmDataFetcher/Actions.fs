namespace ModernRonin.LastFmToSpotify.Console

module Actions= 

    open TextCopy
    open ModernRonin.ConsoleUi.Interaction
    open ModernRonin.LastFmToSpotify



    let retrieveTags userName=
        LastFmApi.getUserTags userName

    let pickTag (userTags: TagInfo list)= 
        let maxNameLength = userTags |> Seq.map (fun t -> t.Name.Length) |> Seq.max
        let formatTag (tag: TagInfo) = 
            let padded = tag.Name.PadRight maxNameLength
            sprintf "%s  (%04i)" padded tag.Count
        userChoice formatTag userTags

    let pickUser() = userChoiceOrFreeform "the username" ["azazeldev"; "celuie"]

    let welcome() = 
        printfn "Hello"
        printfn "Finish all your inputs with [Enter]."


    let rec authorizeSpotify() =
        printfn "You need to authorize me to access your Spotify account and create playlists."
        printfn "To that end, I have copied an URL to your clipboard."
        printfn "Now please open the browser of your choice and paste that URL into the address bar."
        printfn "Follow the prompts by Spotify."
        printfn "At the end you will arrive at a page which shows an error or is empty."
        printfn "The address of this page will begin with %s." SpotifyApi.redirectUri
        printfn "Please copy the whole address, come back here, paste it and press <Enter>!"
    
        SpotifyApi.authorizationUrl() |> Clipboard.SetText 

        match readLine() |> SpotifyApi.parseAuthorizationReply with
        | AuthorizationSuccess result -> Some result
        | AuthorizationFailure ->
            printfn "It seems you have decided not to trust me."
            None
        | UserError -> 
            printfn "Error - it seems you have not copied or pasted the whole address."
            if yesOrNo "Do you want to try again" then authorizeSpotify() else None

    let convertUserTag userName (tag: TagInfo) (spotifyAuth: SpotifyAuthData)=
        printfn "Fetching tracks of tag '%s'..." tag.Name
    
        let print track = sprintf "%s by %s" track.Name track.Artist
        let print page = 
            printfn "Page %i/%i:" page.Index page.Total
            page.Elements |> List.map print |> List.iter (printfn "%s")
            if page.Index<page.Total then printfn "---"
        LastFmApi.getTagTracks userName tag |> Seq.iter print
        printfn "----------------------------------------------"




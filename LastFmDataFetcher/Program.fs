namespace ModernRonin.LastFmToSpotify.Console

module Program= 
    open ModernRonin.ConsoleUi
    open ModernRonin.LastFmToSpotify
    
    [<EntryPoint>]
    let main _ = 
        let cfg= Configuration.load()
        LastFmApi.setApiKey cfg.LastFmApiKey
        SpotifyApi.setApiKey cfg.SpotifyClientKey

        // make sure users can input the long response url from the spotify authorization
        Interaction.setupMaximumInputLength 1000
        StateMachine.run (Start, SharedState.Initial)



namespace ModernRonin.LastFmToSpotify.Console

module Program= 
    open ModernRonin.ConsoleUi
    open ModernRonin.LastFmToSpotify
    
    [<EntryPoint>]
    let main _ = 
        let cfg= Configuration.load()
        LastFmApi.setApiKey cfg.LastFmApiKey
        SpotifyApi.setApiKey cfg.SpotifyClientKey

        Interaction.setupConsole()
        StateMachine.run (Start, SharedState.Initial)



namespace ModernRonin.LastFmToSpotify.Console


[<CLIMutable>]
type ApiConfiguration = {SpotifyClientKey: string; LastFmApiKey: string}

module Configuration= 

    open System
    open System.IO
    open Microsoft.Extensions.Configuration
    open Microsoft.Extensions.DependencyInjection;
    open Microsoft.Extensions.Options


    let private isDevelopment = 
        match Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") with 
        | null -> true
        | environment ->
            match environment.Trim().ToLowerInvariant() with 
            | "" 
            | "development" -> true
            | _ -> false

    let addSecrets (c: IConfigurationBuilder) =
        match isDevelopment with
        | true -> c.AddUserSecrets<ApiConfiguration>()
        | false -> c 

    let load()= 
        let builder = (new ConfigurationBuilder())
                        .AddJsonFile("appsettings.json", false, true)
                        .SetBasePath(Directory.GetCurrentDirectory()) 
                        |> addSecrets

        let config= builder.Build();
        let services= (new ServiceCollection())
                        .Configure<ApiConfiguration>(config.GetSection("ApiConfiguration"))
                        .AddOptions()
                        .BuildServiceProvider()

        services.GetService<IOptions<ApiConfiguration>>().Value
    

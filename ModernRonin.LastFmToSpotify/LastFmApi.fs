namespace ModernRonin.LastFmToSpotify

type TagInfo = { Name: string; Count: int}
type TrackInfo = {Name: string; Artist: string; MusicBrainsId: string option; LastFmUrl: string}
type Page<'T> = {Index: int; Total: int; Elements: 'T list}


module LastFmApi= 

    open FSharp.Data
    open System.Threading

    let mutable private apiKey= "";

    type private UserTags= JsonProvider<""" 
    {
      "toptags": {
        "tag": [
          {
            "name": "no-system-to-my-madness",
            "count": "568",
            "url": "https://www.last.fm/tag/no-system-to-my-madness"
          },
          {
            "name": "chilling-and-thinking-baby",
            "count": "536",
            "url": "https://www.last.fm/tag/chilling-and-thinking-baby"
          }
        ],
        "@attr": {
          "user": "azazeldev"
        }
      }
    }
    """>

    type private TagTracks= JsonProvider<"""
    {
      "taggings": {
        "tracks": {
          "track": [
            {
              "name": "Brigas Nunca Mais",
              "duration": "FIXME",
              "mbid": "",
              "url": "https://www.last.fm/music/Fred+Hersch/_/Brigas+Nunca+Mais",
              "streamable": {
                "#text": "0",
                "fulltrack": "0"
              },
              "artist": {
                "name": "Fred Hersch",
                "mbid": "f573100a-b05b-4286-83f0-5620adf73695",
                "url": "https://www.last.fm/music/Fred+Hersch"
              },
              "image": [
                {
                  "#text": "https://lastfm-img2.akamaized.net/i/u/34s/32791d263a044438b274ff08a732ff5a.png",
                  "size": "small"
                },
                {
                  "#text": "https://lastfm-img2.akamaized.net/i/u/64s/32791d263a044438b274ff08a732ff5a.png",
                  "size": "medium"
                },
                {
                  "#text": "https://lastfm-img2.akamaized.net/i/u/174s/32791d263a044438b274ff08a732ff5a.png",
                  "size": "large"
                },
                {
                  "#text": "https://lastfm-img2.akamaized.net/i/u/300x300/32791d263a044438b274ff08a732ff5a.png",
                  "size": "extralarge"
                }
              ]
            },
            {
              "name": "04 Meditacao",
              "duration": "FIXME",
              "mbid": "",
              "url": "https://www.last.fm/music/Fred+Hersch/_/04+Meditacao",
              "streamable": {
                "#text": "0",
                "fulltrack": "0"
              },
              "artist": {
                "name": "Fred Hersch",
                "mbid": "f573100a-b05b-4286-83f0-5620adf73695",
                "url": "https://www.last.fm/music/Fred+Hersch"
              },
              "image": [
                {
                  "#text": "https://lastfm-img2.akamaized.net/i/u/34s/32791d263a044438b274ff08a732ff5a.png",
                  "size": "small"
                },
                {
                  "#text": "https://lastfm-img2.akamaized.net/i/u/64s/32791d263a044438b274ff08a732ff5a.png",
                  "size": "medium"
                },
                {
                  "#text": "https://lastfm-img2.akamaized.net/i/u/174s/32791d263a044438b274ff08a732ff5a.png",
                  "size": "large"
                },
                {
                  "#text": "https://lastfm-img2.akamaized.net/i/u/300x300/32791d263a044438b274ff08a732ff5a.png",
                  "size": "extralarge"
                }
              ]
            },
            {
              "name": "Summertime",
              "duration": "FIXME",
              "mbid": "",
              "url": "https://www.last.fm/music/The+Ultimate+Jazz+Archive+Set+25/_/Summertime",
              "streamable": {
                "#text": "0",
                "fulltrack": "0"
              },
              "artist": {
                "name": "The Ultimate Jazz Archive Set 25",
                "mbid": "",
                "url": "https://www.last.fm/music/The+Ultimate+Jazz+Archive+Set+25"
              },
              "image": [
                {
                  "#text": "",
                  "size": "small"
                },
                {
                  "#text": "",
                  "size": "medium"
                },
                {
                  "#text": "",
                  "size": "large"
                },
                {
                  "#text": "",
                  "size": "extralarge"
                }
              ]
            }
          ]
        },
        "@attr": {
          "user": "azazeldev",
          "tag": "piano to fall on your knees",
          "page": "1",
          "perPage": "50",
          "totalPages": "1",
          "total": "3"
        }
      }
    }""">

    let private userTagsUrlFormat=  sprintf "http://ws.audioscrobbler.com/2.0/?method=user.gettoptags&user=%s&api_key=%s&format=json"
    let private tagTracksUrlFormat= sprintf "http://ws.audioscrobbler.com/2.0/?method=user.getpersonaltags&user=%s&tag=%s&taggingtype=track&page=%i&api_key=%s&format=json"

    let private addApiKey f = f apiKey

    let private extractTags (json: UserTags.Root) =
        json.Toptags.Tag |> Array.map(fun t -> {Name=t.Name; Count= t.Count}) |> Array.toList



    let private extractTracks (json: TagTracks.Root) = 
        let optionize  = function
            | "" -> None
            | x -> Some x

        json.Taggings.Tracks.Track 
            |> Array.map(fun t -> {Name= t.Name; Artist=t.Artist.Name; MusicBrainsId=optionize (t.Mbid.ToString());LastFmUrl= t.Url})
            |> Array.toList

    let private getTagTracksPage userName (tagName:string) pageIndex=
        let urlFormattedTag = tagName.Replace(' ', '+') 
        let format= tagTracksUrlFormat userName urlFormattedTag pageIndex 
        format |> addApiKey |> TagTracks.Load |> extractTracks



    let private pageSize= 50.0

    let setApiKey key = apiKey <- key

    let getUserTags userName = userName |> userTagsUrlFormat |> addApiKey |> UserTags.Load |> extractTags

    let getTagTracks userName tag = 
        let pageCount= float tag.Count / pageSize |> ceil |> int
        let page p = {Index=p; Total=pageCount; Elements= getTagTracksPage userName tag.Name p}
        seq { for n in 1..pageCount do 
                yield page n 
                Thread.Sleep 200            // not worth making this nicer. rate limit is 5 calls/second
            }


    
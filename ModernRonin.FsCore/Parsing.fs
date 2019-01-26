namespace ModernRonin.FsCore

module Parsing=

    open System

    let parseInt (s:string) = 
        match Int32.TryParse(s) with 
        | (true, number) -> Some(number)
        | _ -> None

    let (|Int|_|) = parseInt

    let queryStringToMap (queryString: string) =
        let parseKeyValuePair (kvp:string) = 
            match kvp.Split '=' |> Array.toList with
            | key::value::[] -> Some (key, value) 
            | _ -> None

        queryString.Split '&' |> Array.map parseKeyValuePair |> Array.filter Option.isSome |> Array.map Option.get |> Map.ofArray

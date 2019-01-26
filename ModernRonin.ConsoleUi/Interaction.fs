namespace ModernRonin.ConsoleUi

module Interaction=

    open System
    open ModernRonin.FsCore.Parsing
    
    let setupMaximumInputLength limit= 
        Console.SetIn(new System.IO.StreamReader(Console.OpenStandardInput(limit), Console.InputEncoding, false, limit))

    let readLine() = Console.ReadLine().Trim()

    let indexOfUserChoice labels = 
        let line index text = sprintf "%02i-->%s" index text
        let menu = labels |> List.mapi line
        menu |> List.iter (printfn "%s")
        printfn "Enter the number of your choice or empty to return:"
        printf ">"
        let input = Console.ReadLine().Trim().ToLowerInvariant()
        parseInt input

    let userChoice optionFormatter options =
        let optionForIndex index = List.item index options
        options |> List.map optionFormatter |> indexOfUserChoice |> Option.map optionForIndex
   

    let userEntry title= 
        printfn "Please enter %s (empty returns):" title
        printf ">"
        match Console.ReadLine().Trim() with
        | "" -> None
        | u -> Some(u)


    let userChoiceOrFreeform title options =
        let choice = "Enter manually" :: options |> indexOfUserChoice
        match choice with
        | Some 0 -> userEntry title 
        | Some index -> Some(List.item (index-1) options)
        | None -> None

    let yesOrNo question=
        printfn "%s (yes/no)?" question
        let rec loop() = 
            match Console.ReadLine().Trim().ToLowerInvariant() with 
            | "y"
            | "yes" -> true
            | "n"
            | "no" -> false
            | _ -> loop()
        loop()
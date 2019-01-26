namespace ModernRonin.ConsoleUi

open System
open ModernRonin.FsCore.Parsing

type LineIo = { 
                writer: string->unit
                reader: unit ->string
                prompt: unit->unit
              }

type Terminal(io: LineIo)=
    let writeLine= io.writer
    let prompt= io.prompt

    member this.readLine= io.reader

    member this.indexOfUserChoice labels = 
        let line index text = sprintf "%02i-->%s" index text
        let menu = labels |> List.mapi line
        menu |> List.iter writeLine
        writeLine "Enter the number of your choice or empty to return:"
        prompt()
        let input = this.readLine().ToLowerInvariant()
        parseInt input

    member this.userChoice optionFormatter options =
        let optionForIndex index = List.item index options
        options |> List.map optionFormatter |> this.indexOfUserChoice |> Option.map optionForIndex
   

    member this.userEntry title= 
        sprintf "Please enter %s (empty returns):" title |> writeLine
        prompt()
        match this.readLine() with
        | "" -> None
        | u -> Some(u)


    member this.userChoiceOrFreeform title options =
        let choice = "Enter manually" :: options |> this.indexOfUserChoice
        match choice with
        | Some 0 -> this.userEntry title 
        | Some index -> Some(List.item (index-1) options)
        | None -> None

    member this.yesOrNo question=
        sprintf "%s (yes/no)?" question |> writeLine
        let rec loop() = 
            match this.readLine().ToLowerInvariant() with 
            | "y"
            | "yes" -> true
            | "n"
            | "no" -> false
            | _ -> loop()
        loop()    


module Interaction=
    let setupMaximumInputLength limit= 
        Console.SetIn(new System.IO.StreamReader(Console.OpenStandardInput(limit), Console.InputEncoding, false, limit))

    
    let private terminal= new Terminal({
                                            writer= printfn "%s"
                                            reader= (fun () -> Console.ReadLine().Trim())
                                            prompt= (fun() -> printf ">")
                                      })
    
    let readLine = terminal.readLine
    let indexOfUserChoice= terminal.indexOfUserChoice
    let userChoice = terminal.userChoice
    let userEntry = terminal.userEntry
    let userChoiceOrFreeform= terminal.userChoiceOrFreeform
    let yesOrNo= terminal.yesOrNo

    

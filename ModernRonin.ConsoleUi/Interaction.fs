namespace ModernRonin.ConsoleUi

open System
open ModernRonin.FsCore.Parsing
open ModernRonin.FsCore.ListExtensions

type LineIo = { 
                Writer: string->unit
                Reader: unit ->string
                Prompt: unit->unit
              }

type Terminal(io: LineIo)=
    let writeLine= io.Writer
    let prompt= io.Prompt

    member this.readLine= io.Reader

    member this.indexOfUserChoice labels = 
        let line index text = sprintf "%02i-->%s" index text
        let menu = labels |> List.mapi line
        menu |> List.iter writeLine
        writeLine "Enter the number of your choice or empty to return:"
        prompt()
        let input = this.readLine().ToLowerInvariant()
        parseInt input

    member this.userChoice optionFormatter options=
        options 
        |> List.map optionFormatter 
        |> this.indexOfUserChoice 
        |> List.optionItemOrNone <| options
   

    member this.userEntry title= 
        sprintf "Please enter %s (empty returns):" title |> writeLine
        prompt()
        match this.readLine() with
        | "" -> None
        | u -> Some(u)


    member this.userChoiceOrFreeform title labels =
        let choice = "Enter manually" :: labels |> this.indexOfUserChoice
        match choice with
        | Some 0 -> this.userEntry title 
        | Some index -> List.itemOrNone (index-1) labels
        | None -> None

    member this.yesOrNo question=
        sprintf "%s (yes/no)?" question |> writeLine
        prompt()
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
                                            Writer= printfn "%s"
                                            Reader= (fun () -> Console.ReadLine().Trim())
                                            Prompt= (fun() -> printf ">")
                                      })
    
    let readLine = terminal.readLine
    let indexOfUserChoice= terminal.indexOfUserChoice
    let userChoice = terminal.userChoice
    let userEntry = terminal.userEntry
    let userChoiceOrFreeform= terminal.userChoiceOrFreeform
    let yesOrNo= terminal.yesOrNo

    

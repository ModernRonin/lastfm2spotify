module ``Terminal``

open NUnit.Framework
open FsUnit
open ModernRonin.ConsoleUi

let noop = (fun () -> ())

module ``readLine``=
    [<Test>]
    let ``delegates to io.Reader``()=
        let io= {
                    Writer= ignore
                    Prompt= noop
                    Reader= (fun ()-> "alpha")
                }    

        let underTest= new Terminal(io)

        underTest.readLine() |> should equal "alpha"

module ``indexOfUserChoice``=
    [<Test>]
    let ``writes a formatted list of <labels> with indices, prompts the user to pick an index and returns that index``()=
        let mutable promptWasCalled= false
        let mutable writtenLines= []
        let choice()=
            if (not promptWasCalled) then "13"
            elif (writtenLines.Length<3) then "17"
            else "19"

        let io= {
                    Writer= (fun l -> writtenLines<-l::writtenLines)
                    Prompt= (fun () -> promptWasCalled<-true)
                    Reader= choice
                }    

        let underTest= new Terminal(io)
        let result= underTest.indexOfUserChoice ["alpha"; "bravo"]

        writtenLines.Length |> should equal 3
        writtenLines |> List.head |> should equal "Enter the number of your choice or empty to return:"
        writtenLines |> List.item 1 |> should equal "01-->bravo"
        writtenLines |> List.item 2 |> should equal "00-->alpha"
        promptWasCalled |> should be True
        result |> should equal (Some 19)

module ``userChoice``=
    [<Test>]
    let ``writes the list of <options> formatted using <optionFormatter> with indices, prompts the user to pick an index and returns the option for the index``()=
        let mutable promptWasCalled= false
        let mutable writtenLines= []
        let choice()=
            if (not promptWasCalled) then "13"
            elif (writtenLines.Length<3) then "17"
            else "1"

        let io= {
                    Writer= (fun l -> writtenLines<-l::writtenLines)
                    Prompt= (fun () -> promptWasCalled<-true)
                    Reader= choice
                }    

        let underTest= new Terminal(io)
        let options= ["alpha"; "bravo"]
        let format (s:string) = s.ToUpperInvariant()
        let result= underTest.userChoice format options

        writtenLines.Length |> should equal 3
        writtenLines |> List.head |> should equal "Enter the number of your choice or empty to return:"
        writtenLines |> List.item 1 |> should equal "01-->BRAVO"
        writtenLines |> List.item 2 |> should equal "00-->ALPHA"
        promptWasCalled |> should be True
        result |> should equal (Some "bravo")

    [<Test>]
    let ``returns None if the user picks a non-existing index``()=
        let mutable promptWasCalled= false
        let mutable writtenLines= []
        let choice()= "13"

        let io= {
                    Writer= (fun l -> writtenLines<-l::writtenLines)
                    Prompt= (fun () -> promptWasCalled<-true)
                    Reader= choice
                }    

        let underTest= new Terminal(io)
        let options= ["alpha"; "bravo"]
        let format (s:string) = s.ToUpperInvariant()
        let result= underTest.userChoice format options

        result |> should equal None

    [<Test>]
    let ``returns None if the user picks a negative index``()=
        let mutable promptWasCalled= false
        let mutable writtenLines= []
        let choice()= "-1"

        let io= {
                    Writer= (fun l -> writtenLines<-l::writtenLines)
                    Prompt= (fun () -> promptWasCalled<-true)
                    Reader= choice
                }    

        let underTest= new Terminal(io)
        let options= ["alpha"; "bravo"]
        let format (s:string) = s.ToUpperInvariant()
        let result= underTest.userChoice format options

        result |> should equal None
    
module ``userEntry``=
    [<Test>]
    let ``writes a message using <title>, prompts, reads a line and returns it``()=
        let mutable promptWasCalled= false
        let mutable writtenLines= []
        let line()= "alpha"

        let io= {
                    Writer= (fun l -> writtenLines<-l::writtenLines)
                    Prompt= (fun () -> promptWasCalled<-true)
                    Reader= line
                }    

        let underTest= new Terminal(io)
        let result= underTest.userEntry "bravo"

        writtenLines.Length |> should equal 1
        promptWasCalled |> should be True
        result |> should equal (Some "alpha")

    [<Test>]
    let ``returns None if the entered line is empty``()=
        let mutable promptWasCalled= false
        let mutable writtenLines= []
        let line()= ""

        let io= {
                    Writer= (fun l -> writtenLines<-l::writtenLines)
                    Prompt= (fun () -> promptWasCalled<-true)
                    Reader= line
                }    

        let underTest= new Terminal(io)
        let result= underTest.userEntry "bravo"

        result |> should equal None

module ``userChoiceOrFreeform``=
    [<Test>]
    let ``writes a formatted list of <labels> with indices, with an additional label for free-form entry, prompts the user to pick an index and label for that index``()=
        let mutable promptWasCalled= false
        let mutable writtenLines= []
        let choice()=
            if (not promptWasCalled) then "13"
            elif (writtenLines.Length<3) then "17"
            else "1"

        let io= {
                    Writer= (fun l -> writtenLines<-l::writtenLines)
                    Prompt= (fun () -> promptWasCalled<-true)
                    Reader= choice
                }    

        let underTest= new Terminal(io)
        let result= underTest.userChoiceOrFreeform "zulu" ["alpha"; "bravo"]

        writtenLines.Length |> should equal 4
        writtenLines |> List.head |> should equal "Enter the number of your choice or empty to return:"
        writtenLines |> List.item 1 |> should equal "02-->bravo"
        writtenLines |> List.item 2 |> should equal "01-->alpha"
        writtenLines |> List.item 3 |> should equal "00-->Enter manually"
        promptWasCalled |> should be True
        result |> should equal (Some "alpha")
    
    [<Test>]
    let ``if the user picks free-form, reads a line and returns that``()=
        let mutable promptWasCalled= false
        let mutable writtenLines= []
        let mutable isSecondChoiceCall= false
        let choice()=
            if (not promptWasCalled) then "13"
            elif (writtenLines.Length<3) then "17"
            elif (isSecondChoiceCall) then "xray"
            else    
                isSecondChoiceCall<-true
                "0"

        let io= {
                    Writer= (fun l -> writtenLines<-l::writtenLines)
                    Prompt= (fun () -> promptWasCalled<-true)
                    Reader= choice
                }    

        let underTest= new Terminal(io)
        let result= underTest.userChoiceOrFreeform "zulu" ["alpha"; "bravo"]

        writtenLines.Length |> should equal 5
        writtenLines |> List.head |> should equal "Please enter zulu (empty returns):"
        writtenLines |> List.item 1 |> should equal "Enter the number of your choice or empty to return:"
        writtenLines |> List.item 2 |> should equal "02-->bravo"
        writtenLines |> List.item 3 |> should equal "01-->alpha"
        writtenLines |> List.item 4 |> should equal "00-->Enter manually"
        promptWasCalled |> should be True
        result |> should equal (Some "xray")

    [<Test>]
    let ``returns None if the user picks an index too high``()=
        let mutable promptWasCalled= false
        let mutable writtenLines= []
        let choice()=
            if (not promptWasCalled) then "13"
            elif (writtenLines.Length<3) then "17"
            else "3"

        let io= {
                    Writer= (fun l -> writtenLines<-l::writtenLines)
                    Prompt= (fun () -> promptWasCalled<-true)
                    Reader= choice
                }    

        let underTest= new Terminal(io)
        let result= underTest.userChoiceOrFreeform "zulu" ["alpha"; "bravo"]

        writtenLines.Length |> should equal 4
        writtenLines |> List.head |> should equal "Enter the number of your choice or empty to return:"
        writtenLines |> List.item 1 |> should equal "02-->bravo"
        writtenLines |> List.item 2 |> should equal "01-->alpha"
        writtenLines |> List.item 3 |> should equal "00-->Enter manually"
        promptWasCalled |> should be True
        result |> should equal None
    
    [<Test>]
    let ``returns None if the user picks a negative index``()=
        let mutable promptWasCalled= false
        let mutable writtenLines= []
        let choice()=
            if (not promptWasCalled) then "13"
            elif (writtenLines.Length<3) then "17"
            else "-1"

        let io= {
                    Writer= (fun l -> writtenLines<-l::writtenLines)
                    Prompt= (fun () -> promptWasCalled<-true)
                    Reader= choice
                }    

        let underTest= new Terminal(io)
        let result= underTest.userChoiceOrFreeform "zulu" ["alpha"; "bravo"]

        writtenLines.Length |> should equal 4
        writtenLines |> List.head |> should equal "Enter the number of your choice or empty to return:"
        writtenLines |> List.item 1 |> should equal "02-->bravo"
        writtenLines |> List.item 2 |> should equal "01-->alpha"
        writtenLines |> List.item 3 |> should equal "00-->Enter manually"
        promptWasCalled |> should be True
        result |> should equal None
    
module ``yesOrNo``=
    [<Test>]
    let ``writes a message using questions, prompts, reads input and returns true if input is 'yes'``()=
        let mutable promptWasCalled= false
        let mutable writtenLines= []
        let answer()= "yes"

        let io= {
                    Writer= (fun l -> writtenLines<-l::writtenLines)
                    Prompt= (fun () -> promptWasCalled<-true)
                    Reader= answer
                }    

        let underTest= new Terminal(io)
        let result= underTest.yesOrNo "alpha"

        writtenLines.Length |> should equal 1
        writtenLines |> List.head |> should equal "alpha (yes/no)?"
        promptWasCalled |> should be True
        result |> should be True
    
    [<Test>]
    let ``returns true also for input 'y'``()=
        let answer()= "y"

        let io= {
                    Writer= ignore
                    Prompt= noop
                    Reader= answer
                }    

        let underTest= new Terminal(io)
        let result= underTest.yesOrNo "alpha"

        result |> should be True

    [<Test>]
    let ``returns false for input 'no'``()=
        let answer()= "no"

        let io= {
                    Writer= ignore
                    Prompt= noop
                    Reader= answer
                }    

        let underTest= new Terminal(io)
        let result= underTest.yesOrNo "alpha"

        result |> should be False

    [<Test>]
    let ``returns false also for input 'n'``()=
        let answer()= "n"

        let io= {
                    Writer= ignore
                    Prompt= noop
                    Reader= answer
                }    

        let underTest= new Terminal(io)
        let result= underTest.yesOrNo "alpha"

        result |> should be False
        
    [<Test>]
    let ``keeps reading lines until it gets a valid answer``()=
        let mutable answers= ["bla"; "blu"; "ble"; "yes"]
        let answer()= 
            match answers with 
            | head::tail ->
                answers <- tail
                head
            | [] -> failwith "should not read past 'yes'"

        let io= {
                    Writer= ignore
                    Prompt= noop
                    Reader= answer
                }    

        let underTest= new Terminal(io)
        let result= underTest.yesOrNo "alpha"

        result |> should be True
        answers |> should be Empty
        
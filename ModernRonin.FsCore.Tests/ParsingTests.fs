module ``Parsing``

open NUnit.Framework
open FsUnit
open ModernRonin.FsCore.Parsing

module parseInt=
    [<Test>]
    let ``returns None if string is not an int``()=
        parseInt "bla" |> should equal None

    [<Test>]
    let ``returns Some <number> if the string is an int``()=
        parseInt "13" |> should equal (Some 13)


module ``Active pattern Int``=
    [<Test>]
    let ``does not match if string is not an int``()= 
        match "bla" with
        | Int _ -> false
        | _ -> true
        |> should be True

    [<Test>]
    let ``matches and returns the number if string is an int``()=
        match "13" with
        | Int n -> n
        | _ -> 0
        |> should equal 13

module ``queryStringToMap``=
    [<Test>]
    let ``returns a key-value map of the encoded parameters``()=
        let result= queryStringToMap "alpha=13&bravo=nix&charlie=..."
        result |> should haveCount 3
        result.["alpha"] |> should equal "13"
        result.["bravo"] |> should equal "nix"
        result.["charlie"] |> should equal "..."

    [<Test>]
    let ``deals correctly with empty values``()=
        let result= queryStringToMap "alpha=13&bravo=&charlie=..."
        result |> should haveCount 3
        result.["alpha"] |> should equal "13"
        result.["bravo"] |> should equal ""
        result.["charlie"] |> should equal "..."

    [<Test>]
    let ``filters out incomplete key-value pairs``()=
        let result= queryStringToMap "alpha=13&bravo&charlie=..."
        result |> should haveCount 2
        result.["alpha"] |> should equal "13"
        result.["charlie"] |> should equal "..."
        
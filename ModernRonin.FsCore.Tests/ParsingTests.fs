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

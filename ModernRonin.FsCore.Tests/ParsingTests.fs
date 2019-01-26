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



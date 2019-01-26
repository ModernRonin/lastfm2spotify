module ``Strings``

open NUnit.Framework
open FsUnit
open ModernRonin.FsCore.Strings


module ``Active pattern Start``=
    [<Test>]
    let ``does not match if <whole> does not begin with <part>``()=
        match "alpha centauri" with
        | Start "bravo" _ -> false
        | _ -> true
        |> should be True

    [<Test>]
    let ``matches and returns everything after <part> in <whole>``()=
        match "alpha centauri" with
        | Start "alpha " x -> x
        | _ -> ""
        |> should equal "centauri"
    




module Strings

open NUnit.Framework
open FsUnit
open ModernRonin.FsCore.Strings

[<Test>]
let ``Start returns None if <whole> does not begin with <part>``()=
    match "alpha centauri" with
    | Start "bravo" _ -> false
    | _ -> true
    |> should be True

[<Test>]
let ``Start returns Some with everything after <part> in <whole>``()=
    match "alpha centauri" with
    | Start "alpha " x -> Some x
    | _ -> None
    |> should equal (Some "centauri")
    




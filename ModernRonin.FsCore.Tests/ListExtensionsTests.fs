module ``ListExtensions``

open NUnit.Framework
open FsUnit
open ModernRonin.FsCore.ListExtensions


let list = [11; 13; 17] 

module ``itemOrNone``=
    [<Test>]
    let ``returns None if <index> is negative``()=
       list |> List.itemOrNone -1 |> should equal None

    [<Test>]
    let ``return None if <index> is too big``()=
        list |> List.itemOrNone 3 |> should equal None

    [<Test>]
    let ``returns the item at <index> for valid indices``()=
        list |> List.itemOrNone 0 |> should equal (Some 11)
        list |> List.itemOrNone 1 |> should equal (Some 13)
        list |> List.itemOrNone 2 |> should equal (Some 17)

module ``optionItemOrNone``= 
    [<Test>]
    let ``returns None if <index> is none``()=
       list |> List.optionItemOrNone None |> should equal None

    [<Test>]
    let ``returns None if <index> is negative``()=
       list |> List.optionItemOrNone (Some -1) |> should equal None

    [<Test>]
    let ``return None if <index> is too big``()=
        list |> List.optionItemOrNone (Some 3) |> should equal None

    [<Test>]
    let ``returns the item at <index> for valid indices``()=
        list |> List.optionItemOrNone (Some 0) |> should equal (Some 11)
        list |> List.optionItemOrNone (Some 1) |> should equal (Some 13)
        list |> List.optionItemOrNone (Some 2) |> should equal (Some 17)
    
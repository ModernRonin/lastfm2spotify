namespace ModernRonin.FsCore

module ListExtensions=
    type List<'T> with
        static member itemOrNone index list=
            match index with
            | i when i<0 -> None
            | i when i>= (list |> List.length) -> None
            | i -> Some (list |> List.item i)

        static member optionItemOrNone index list=
            match index with
            | None -> None
            | Some i -> list |> List.itemOrNone i
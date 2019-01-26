namespace ModernRonin.FsCore

module Strings=

    let (|Start|_|) (part:string) (whole:string) =
        if whole.StartsWith part then
            Some (whole.Substring part.Length)
        else
            None

 

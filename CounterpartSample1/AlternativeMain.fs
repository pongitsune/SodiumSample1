open System

[<EntryPoint>]
do
    Seq.initInfinite(fun _ -> Console.ReadKey(true))
    |> Seq.takeWhile(fun (ki: ConsoleKeyInfo) -> ki.Key <> ConsoleKey.Escape)
    |> Seq.choose(fun (ki: ConsoleKeyInfo) ->
                        match ki.KeyChar with
                        | c when Char.IsNumber c ->
                            $" '%c{c}' は数字です" |> Some
                        | c when Char.IsLetter c ->
                            $" '%c{c}' はアルファベットです" |> Some
                        | _ -> None)
    |> Seq.iter(fun (s: string) -> printfn $"%s{s}")
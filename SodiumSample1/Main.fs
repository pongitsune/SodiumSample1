open System
open Sodium.Frp

[<EntryPoint>]
do
    let sKeyInput: StreamSink<ConsoleKeyInfo> = sinkS()
    let sStrOutput: Stream<string> = 
        sKeyInput
        |> mapS(fun (ki: ConsoleKeyInfo) ->
                match ki.KeyChar with
                | c when Char.IsNumber c ->
                    $" '%c{c}' は数字です" |> Some
                | c when Char.IsLetter c ->
                    $" '%c{c}' はアルファベットです" |> Some
                | _ -> None)
        |> filterOptionS
    
    use outputListener: IStrongListener =
        sStrOutput
        |> listenS (fun (s: string) -> printfn $"%s{s}")

    Seq.initInfinite(fun _ -> Console.ReadKey(true))
    |> Seq.takeWhile(fun (ki: ConsoleKeyInfo) -> ki.Key <> ConsoleKey.Escape)
    |> Seq.iter(fun (ki: ConsoleKeyInfo) -> sKeyInput |> sendS ki)

    outputListener |> unlistenL
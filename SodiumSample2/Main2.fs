open System
open Sodium.Frp

[<EntryPoint>]
do
    /// キー入力イベント用ストリーム
    let sKeyInput: StreamSink<ConsoleKeyInfo> = StreamSink.create()

    use lKeyInput: IStrongListener =
        sKeyInput
        |> mapS (fun (ki: ConsoleKeyInfo) ->
            match ki.KeyChar with
            | c when Char.IsNumber c -> Some c 
            | c when Char.IsLetter c -> Some c
            | _ -> None)
        |> filterOptionS
        |> listenS (fun (c: char) -> printfn $" %c{c}")

    /// 数字キーを押した回数を保持するセル
    let cNumberCounter: Cell<int> = 
        loopWithNoCapturesC (fun (lpcCount: LoopedCell<int>) -> 
            sKeyInput
            |> snapshotC lpcCount (fun (ki: ConsoleKeyInfo) (acc: int) -> 
                                                if Char.IsNumber ki.KeyChar then acc + 1 else acc) 
            |> calmS
            |> holdS 0)

    /// cNumberCounter にリスナー関数を登録する
    use lNumberCounter: IStrongListener =
        cNumberCounter
        |> listenC (fun (count: int) -> printfn $" 数字入力回数= %d{count}")

    /// アルファベットキーを押した回数を保持するセル
    let cLetterCounter: Cell<int> =
        sKeyInput
        |> accumS 0 (fun (ki: ConsoleKeyInfo) (acc: int) -> 
            if Char.IsLetter ki.KeyChar then acc + 1 else acc)
        |> calmC

    /// cLetterCounter にリスナー関数を登録する
    use lLetterCounter: IStrongListener =
        cLetterCounter
        |> listenC (fun (count: int) -> printfn $" アルファベット入力回数= %d{count}")
    
    printfn $" Escキーでアプリケーションを終了します{Environment.NewLine}"
    /// キー入力イベントを sKeyInput ストリームへ流し続ける（Escキー入力で終了）
    Seq.initInfinite(fun _ -> Console.ReadKey(true))
    |> Seq.takeWhile(fun (ki: ConsoleKeyInfo) -> ki.Key <> ConsoleKey.Escape)
    |> Seq.iter(fun (ki: ConsoleKeyInfo) -> sKeyInput |> sendS ki)

    /// リスナー登録を解除する
    [ lKeyInput; lNumberCounter; lLetterCounter ]
    |> Listener.fromList
    |> unlistenL

namespace RpnCalculator.Logic

open System
open System.Collections.Generic
open System.Collections.Specialized
open System.ComponentModel
open RpnCalculator.Collections
open System.Linq.Expressions
open System.Linq

type public Operation =
    | Addition = 0
    | Subtraction = 1
    | Multiplication = 2
    | Division = 3
    | Swap = 4
    | Drop = 5

type public Entry (v : decimal) =
    let ev = new Event<_,_>()
    let mutable value = v
    
    member this.Value
        with public get() = value
        and public set v = value <- v

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = ev.Publish

type public Calculator () =
    let stack = new ObservableStack<Entry>()
    let peek n =
        match stack.Count, n with
        | 0, _ -> None
        | x, y when y >= x -> None
        | _, 0 -> Some (stack.Peek())
        | _, _ -> Some (stack |> Seq.nth n)

    member this.Stack = stack
    member this.X = peek 0
    member this.Y = peek 1
    member this.Z = peek 2
    member this.T = peek 3

    member this.Push x = stack.Push x

    member this.Perform op =
        let fn =
            match stack.Count, op with
            | 0, _ -> None
            | 1, _ -> None
            | _, Operation.Addition -> Some(+)
            | _, Operation.Subtraction -> Some(-)
            | _, Operation.Multiplication -> Some(*)
            | _, Operation.Division -> Some(/)
            | _, Operation.Swap ->
                let x = stack.Pop()
                let y = stack.Pop()
                stack.Push x
                stack.Push y
                None
            | _, Operation.Drop ->
                stack.Pop() |> ignore
                None
            | _ -> raise (InvalidOperationException())

        match fn with
        | Some fn ->
            let x = stack.Pop()
            let y = stack.Pop()
            let result = fn y.Value x.Value
            let value = Entry result
            stack.Push value
        | None -> ()
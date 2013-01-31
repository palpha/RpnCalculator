namespace RpnCalculator.Logic

open System
open System.Collections.Generic
open System.Collections.Specialized
open RpnCalculator.Collections
open System.Linq.Expressions
open System.Linq

type public Operation =
    | Addition = 0
    | Subtraction = 1
    | Multiplication = 2
    | Division = 3
    | Swap = 4

type public Calculator() =
    let stack = new ObservableStack<decimal>()
    let peek n =
        match (stack.Count, n) with
        | (0, _) -> None
        | (_, 0) -> Some(stack.Peek())
        | (x, y) when y >= x -> None
        | (_, _) -> Some (stack |> Seq.nth (stack.Count - n))

    member this.Stack = stack
    member this.X = peek 0
    member this.Y = peek 1
    member this.Z = peek 2
    member this.T = peek 3

    member this.Push(x) =
        match x with
        | 0m -> ()
        | _ -> stack.Push(x)

    member this.Perform(op) =
        let fn =
            match (stack.Count, op) with
            | (0, _) -> None
            | (1, _) -> None
            | (_, Operation.Addition) -> Some(+)
            | (_, Operation.Subtraction) -> Some(-)
            | (_, Operation.Multiplication) -> Some(*)
            | (_, Operation.Division) -> Some(/)
            | (_, Operation.Swap) ->
                let x = stack.Pop()
                let y = stack.Pop()
                stack.Push(x)
                stack.Push(y)
                None
            | _ -> raise (InvalidOperationException())

        match fn with
        | Some(fn) ->
            let x = stack.Pop()
            let y = stack.Pop()
            stack.Push(fn y x)
        | None -> ()
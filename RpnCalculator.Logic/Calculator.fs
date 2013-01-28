namespace RpnCalculator.Logic

open System
open System.Collections.Generic

type public Operation =
    | Addition = 0
    | Subtraction = 1
    | Multiplication = 2
    | Division = 3
    | Swap = 4

type public Calculator() = 
    let stack = new Stack<decimal>()
    let rec peek n =
        match stack.Count with
        | 0 -> None
        | _ ->
            match n with
            | 0 -> Some(stack.Peek())
            | _ ->
                let p = stack.Pop()
                let result = peek (n - 1)
                stack.Push(p)
                result

    member this.X() = peek 0
    member this.Y() = peek 1
    member this.Z() = peek 2
    member this.T() = peek 3

    member this.AddToStack(x) =
        stack.Push(x)
    
    member this.Perform(op) =
        let fn =
            match op with
            | Operation.Addition -> Some(+)
            | Operation.Subtraction -> Some(-)
            | Operation.Multiplication -> Some(*)
            | Operation.Division -> Some(/)
            | Operation.Swap ->
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
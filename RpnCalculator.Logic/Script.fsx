// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#I @"bin/Debug"
#r "RpnCalculator.Collections.dll"
#load "Calculator.fs"
open RpnCalculator.Logic

// Define your library scripting code here

let calc = Calculator()
calc.Push(Entry 10m)
calc.Push(Entry 2m)
calc.Push(Entry 5m)
calc.Perform(Operation.Multiplication)
assert (calc.X = Some(Entry 10m))
calc.Perform(Operation.Addition)
assert (calc.X = Some(Entry 20m))
assert (calc.Y = None)
calc.Push(Entry 10m)
calc.Perform(Operation.Swap)
assert (calc.X = Some(Entry 20m))
assert (calc.Y = Some(Entry 10m))
calc.Perform(Operation.Subtraction)
assert (calc.X = Some(Entry -10m))
calc.Push(Entry 10m)
calc.Push(Entry 2m)
calc.Perform(Operation.Multiplication)
calc.Perform(Operation.Addition)
assert (calc.X = Some(Entry 10m))

calc.Stack |> Seq.nth 0
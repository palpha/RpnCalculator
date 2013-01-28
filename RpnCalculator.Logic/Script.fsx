// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Calculator.fs"
open RpnCalculator.Logic

// Define your library scripting code here

let calc = Calculator()
calc.Push(10m)
calc.Push(2m)
calc.Push(5m)
calc.Perform(Operation.Multiplication)
assert (calc.X = Some(10m))
calc.Perform(Operation.Addition)
assert (calc.X = Some(20m))
assert (calc.Y = None)
calc.Push(10m)
calc.Perform(Operation.Swap)
assert (calc.X = Some(20m))
assert (calc.Y = Some(10m))
calc.Perform(Operation.Subtraction)
assert (calc.X = Some(-10m))
calc.Push(10m)
calc.Push(2m)
calc.Perform(Operation.Multiplication)
calc.Perform(Operation.Addition)
assert (calc.X = Some(10m))


calc.Stack
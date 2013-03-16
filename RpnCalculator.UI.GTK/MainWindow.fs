
// NOTE: If warnings appear, you may need to retarget this project to .NET 4.0. Show the Solution
// Pad, right-click on the project node, choose 'Options --> Build --> General' and change the target
// framework to .NET 4.0 or .NET 4.5.

namespace RpnCalculator.UI.GTK

    module MainWindow =
    
        open System
        open Gtk;

        type MyWindow() as this =
            inherit Window("MainWindow")
            
            do this.SetDefaultSize(400,300)
            do this.DeleteEvent.AddHandler(fun o e -> this.OnDeleteEvent(o,e))
            do this.ShowAll()
            
            member this.OnDeleteEvent(o,e:DeleteEventArgs) = 
                Application.Quit ()
                e.RetVal <- true

namespace Feliz.ReactResizeDetector

open Feliz
open Fable.Core
open Fable.Core.JsInterop

[<Erase>]
type Exports =

    [<Hook; Import("useResizeDetector", "react-resize-detector")>]
    static member useResizeDetector<'T> (?props: FunctionProps) : UseResizeDetectorReturn<'T> =
        jsNative

    static member inline reactResizeDetector (properties : #IReactResizeDetectorProperty  list) =
        Interop.reactApi.createElement(import "default" "react-resize-detector", createObj !!properties)

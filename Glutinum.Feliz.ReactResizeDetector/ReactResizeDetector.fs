namespace Feliz.ReactResizeDetector

open Feliz
open Fable.Core
open Fable.Core.JsInterop

#nowarn "3390" // disable warnings for invalid XML comments

[<StringEnum>]
[<RequireQualifiedAccess>]
type PropsRefreshMode =
    | Throttle
    | Debounce

[<StringEnum>]
[<RequireQualifiedAccess>]
type ResizeObserverOptionsBox =
    | [<CompiledName "content-box">] ContentBox
    | [<CompiledName "border-box">] BorderBox
    | [<CompiledName "device-pixel-content-box">] DevicePixelContentBox

[<Erase>]
type reactResizeDetector =
    static member inline reactResizeDetector (properties : #IReactResizeDetectorProperty) =
        Interop.reactApi.createElement(import "default" "react-resize-detector", createObj properties)

    /// Function that will be invoked with observable element's width and height.
    /// Default: undefined
    static member inline onResize (callback : (float option -> float option -> unit)) =
        Interop.mkReactResizeDetectorProperty "onResize" callback

    /// Trigger update on height change.
    /// Default: true
    static member inline handleHeight (value : bool) =
        Interop.mkReactResizeDetectorProperty "handleHeight" value

    /// Trigger onResize on width change.
    /// Default: true
    static member inline handleWidth (value : bool) =
        Interop.mkReactResizeDetectorProperty "handleWidth" value

    /// Do not trigger update when a component mounts.
    /// Default: false
    static member inline skipOnMount (value : bool) =
        Interop.mkReactResizeDetectorProperty "skipOnMount" value

    /// <summary>
    /// Changes the update strategy. Possible values: "throttle" and "debounce".
    /// See <c>lodash</c> docs for more information <see href="https://lodash.com/docs/" />
    /// undefined - callback will be fired for every frame.
    /// Default: undefined
    /// </summary>
    static member inline refreshMode (value : PropsRefreshMode) =
        Interop.mkReactResizeDetectorProperty "refreshMode" value

    /// <summary>
    /// Set the timeout/interval for <c>refreshMode</c> strategy
    /// Default: undefined
    /// </summary>
    static member inline refreshRate (value : float) =
        Interop.mkReactResizeDetectorProperty "refreshRate" value

    /// <summary>
    /// Pass additional params to <c>refreshMode</c> according to lodash docs
    /// Default: undefined
    /// </summary>
    static member inline refreshOptions (?leading : bool, ?trailing : bool) =
        Interop.mkReactResizeDetectorProperty "refreshOptions"
            {|
                leading = leading
                trailing = trailing
            |}

    /// <summary>These options will be used as a second parameter of <c>resizeObserver.observe</c> method</summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserver/observe">Default: undefined</seealso>
    static member inline observerOptions (value : ResizeObserverOptionsBox) =
        Interop.mkReactResizeDetectorProperty "observerOptions" value

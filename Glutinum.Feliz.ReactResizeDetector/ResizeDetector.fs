namespace rec Feliz.ReactResizeDetector

open Feliz
open Fable.Core

#nowarn "3390" // disable warnings for invalid XML comments

[<StringEnum>]
[<RequireQualifiedAccess>]
type RefreshMode =
    | Throttle
    | Debounce

[<StringEnum>]
[<RequireQualifiedAccess>]
type ResizeObserverOptionsBox =
    | [<CompiledName "content-box">] ContentBox
    | [<CompiledName "border-box">] BorderBox
    | [<CompiledName "device-pixel-content-box">] DevicePixelContentBox

type [<AllowNullLiteral>] ReactResizeDetectorDimensions =
    abstract height: float option with get
    abstract width: float option with get

type ChildFunctionProps<'ElementT when 'ElementT :> Browser.Types.HTMLElement> =
    inherit ReactResizeDetectorDimensions
    abstract targetRef: IRefValue<'ElementT> option with get

[<Erase>]
type reactResizeDetector =
    /// Function that will be invoked with observable element's width and height.
    /// Default: undefined
    static member inline onResize (callback : (float option -> float option -> unit)) =
        Interop.mkReactResizeDetectorProperty "onResize" callback

    /// Trigger update on height change.
    /// Default: true
    static member inline handleHeight =
        Interop.mkReactResizeDetectorProperty "handleHeight" true

    /// Trigger onResize on width change.
    /// Default: true
    static member inline handleWidth =
        Interop.mkReactResizeDetectorProperty "handleWidth" true

    /// Do not trigger update when a component mounts.
    /// Default: false
    static member inline skipOnMount =
        Interop.mkReactResizeDetectorProperty "skipOnMount" true

    /// <summary>
    /// Changes the update strategy. Possible values: "throttle" and "debounce".
    /// See <c>lodash</c> docs for more information <see href="https://lodash.com/docs/" />
    /// undefined - callback will be fired for every frame.
    /// Default: undefined
    /// </summary>
    static member inline refreshMode (value : RefreshMode) =
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
    static member inline refreshOptions (refreshOptions : RefreshOptions) =
        Interop.mkReactResizeDetectorProperty "refreshOptions" refreshOptions

    /// <summary>These options will be used as a second parameter of <c>resizeObserver.observe</c> method</summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserver/observe">Default: undefined</seealso>
    static member inline observerOptions (value : ResizeObserverOptionsBox) =
        Interop.mkReactResizeDetectorProperty "observerOptions" value

    static member children (value : ChildFunctionProps<_> -> ReactElement) =
        Interop.mkReactResizeDetectorProperty "children" value

[<Global>]
type ResizeObserverOptions
    [<ParamObject; Emit "$0">]
    (
        ?box : ResizeObserverOptionsBox
    ) =
    /// <summary>
    /// Sets which box model the observer will observe changes to. Possible values
    /// are <c>content-box</c> (the default), and <c>border-box</c>.
    /// </summary>
    /// <default>'content-box'</default>
    member val box: ResizeObserverOptionsBox option = jsNative with get, set

[<Global>]
type RefreshOptions
    [<ParamObject; Emit "$0">]
    (
        ?leading : bool,
        ?trailing : bool
    )
    =
    member val leading : bool option = jsNative with get, set
    member val trailing : bool option = jsNative with get, set

namespace rec Feliz.ReactResizeDetector

open Fable.Core
open Feliz

[<Global>]
type FunctionProps
    [<ParamObject; Emit "$0">]
    (
        ?onResize: (float option -> float option -> unit),
        ?handleHeight: bool,
        ?handleWidth: bool,
        ?skipOnMount: bool,
        ?refreshMode: RefreshMode,
        ?refreshRate: float,
        ?refreshOptions: RefreshOptions,
        ?observerOptions: ResizeObserverOptions,
        ?targetRef: IRefValue<Browser.Types.HTMLElement>
    )
    =
    /// Function that will be invoked with observable element's width and height.
    /// Default: undefined
    member val onResize: (float option -> float option -> unit) option = jsNative with get, set
    /// Trigger update on height change.
    /// Default: true
    member val handleHeight: bool option = jsNative with get, set
    /// Trigger onResize on width change.
    /// Default: true
    member val handleWidth: bool option = jsNative with get, set
    /// Do not trigger update when a component mounts.
    /// Default: false
    member val skipOnMount: bool option = jsNative with get, set
    /// <summary>
    /// Changes the update strategy. Possible values: "throttle" and "debounce".
    /// See <c>lodash</c> docs for more information <see href="https://lodash.com/docs/" />
    /// undefined - callback will be fired for every frame.
    /// Default: undefined
    /// </summary>
    member val refreshMode: RefreshMode option = jsNative with get, set
    /// <summary>
    /// Set the timeout/interval for <c>refreshMode</c> strategy
    /// Default: undefined
    /// </summary>
    member val refreshRate: float option = jsNative with get, set
    /// <summary>
    /// Pass additional params to <c>refreshMode</c> according to lodash docs
    /// Default: undefined
    /// </summary>
    member val refreshOptions: RefreshOptions option = jsNative with get, set
    /// <summary>These options will be used as a second parameter of <c>resizeObserver.observe</c> method</summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserver/observe">Default: undefined</seealso>
    member val observerOptions: ResizeObserverOptions option = jsNative with get, set

type [<AllowNullLiteral>] UseResizeDetectorReturn<'T> =
    inherit ReactResizeDetectorDimensions
    abstract ref: IRefValue<'T option> with get, set

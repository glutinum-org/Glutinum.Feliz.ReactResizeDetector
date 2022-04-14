module Demo.ChildFunction

open Feliz
open Browser.Dom
open Fable.Core.JsInterop
open Feliz.ReactResizeDetector

open type ReactResizeDetector.Exports

// Workaround to have React-refresh working
// I need to open an issue on react-refresh to see if they can improve the detection
emitJsStatement () "import React from \"react\""

importSideEffects "./index.scss"

type Props =
    {|
        height : float option
        width : float option
        toggleSideBar : unit -> unit
    |}

[<ReactComponent>]
let private MainFrame (props : Props) =
    Html.div [
        prop.className "right-panel"

        prop.children [
            Html.div [
                prop.className "actions-area"
                prop.children [
                    Html.button [
                        prop.className "toggle-sidebar"
                        prop.text "Toogle Sidebar"
                        prop.onClick (fun _ ->
                            props.toggleSideBar()
                        )
                    ]

                    Html.span "or resize the window"
                ]
            ]

            Html.div [
                prop.className "size-report"

                prop.text $"Width: {props.width}, Height: {props.height}"
            ]
        ]
    ]

[<ReactComponent>]
let private Component () =
    let isLeftPanelVisible, setLeftPanelVisibility = React.useState true

    Html.div [
        prop.className "wrapper"

        prop.children [
            if isLeftPanelVisible then
                Html.div [
                    prop.className "left-panel"
                    prop.text "Left panel content"
                ]

            reactResizeDetector [
                reactResizeDetector.handleHeight
                reactResizeDetector.handleWidth

                reactResizeDetector.children (
                    fun props ->
                        MainFrame
                            {|
                                height = props.height
                                width = props.width
                                toggleSideBar = fun _ ->
                                    setLeftPanelVisibility (not isLeftPanelVisible)
                            |}
                )
            ]
        ]
    ]

ReactDOM.render(
    Component ()
    ,
    document.getElementById("root")
)

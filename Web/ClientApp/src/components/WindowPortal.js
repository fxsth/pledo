import React from "react";
import * as ReactDOM from "react-dom";

export class WindowPortal extends React.PureComponent {
    constructor(props) {
        super(props);
        this.containerEl = null;
        this.externalWindow = null;
    }

    componentDidMount() {
        // STEP 1: Create a new window, a div, and append it to the window. The div 
        // *MUST** be created by the window it is to be appended to (Edge only)
        this.externalWindow = window.open('', '', 'width=600,height=400,left=200,top=200');
        this.containerEl = this.externalWindow.document.createElement('div');
        this.externalWindow.document.body.appendChild(this.containerEl);
    }

    componentWillUnmount() {
        // STEP 2: This will fire when this.state.showWindowPortal in the parent component
        // becomes false so we tidy up by just closing the window
        this.externalWindow.close();
    }

    render() {
        // STEP 3: The first render occurs before componentDidMount (where we open the
        // new window) so container may be null, in this case render nothing.
        if (!this.containerEl) {
            return null;
        }

        // STEP 4: Append props.children to the container <div> in the new window
        return ReactDOM.createPortal(this.props.children, this.containerEl);
    }
}
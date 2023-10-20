import React from 'react';
import {
    Button,
    DropdownItem,
    DropdownMenu,
    DropdownToggle, UncontrolledDropdown, UncontrolledTooltip
} from "reactstrap";

export default class DownloadButton extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isLoading: false,
            tooltipOpen: false
        };
    }
    handleClick(event) {
        this.setState({
            isLoading: true
        });
        this.SendDownloadRequest();

    }
    
    getDownloadLink(){
        try {
            const host = this.props.server.lastKnownUri
            const token = this.props.server.accessToken
            const path = this.props.mediaFileKey
            const url = new URL(path, host);
            url.searchParams.append("X-Plex-Token", token)
            return url.toString()
        }
        catch(e)
        {
            console.log("An error occured while building download link.")
            return ""
        }
    }

    getFilename(){
        try {
            const mediaFile = this.props.mediaFile
            const fullPath = mediaFile.serverFilePath
            return fullPath.replace(/^.*[\\/]/, '')
        }
        catch(e)
        {
            console.log("An error occured while building download file name.")
            return ""
        }
    }

    async SendDownloadRequest() {
        const settings = {
            method: 'POST',
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            }
        };
        let input = 'api/download/' + this.props.mediaType + '/' + this.props.mediaKey;
        if (typeof this.props.season !== 'undefined') {
            input = input + '?' + new URLSearchParams({
                season: this.props.season
            })
        }
        if (typeof this.props.mediaFileKey !== 'undefined') {
            input = input + '?' + new URLSearchParams({
                mediaFileKey: this.props.mediaFileKey
            })
        }
        return fetch(input, settings)
            .then(response => {
                if (response.status >= 200 && response.status < 300) {
                    console.log(response);
                } else {
                    alert('Could not add to the download queue');
                }
            }).catch(err => console.log(err)).finally(x => {
                this.setState({
                    isLoading: false
                });
            });
    }

    render() {
        if (this.props.downloadBrowserPossible)
            return (
                <span>
                    <UncontrolledDropdown group id={'download_button_'+ this.props.mediaKey}>
                        <Button color={this.props.color} disabled={this.state.isLoading}
                                onClick={this.handleClick.bind(this)}>{this.props.children}</Button>
                        <DropdownToggle caret />
                        <DropdownMenu>
                            <DropdownItem header>
                                To download in browser save link as file:
                            </DropdownItem>
                            <DropdownItem>
                                <a href={this.getDownloadLink()} download>Direct link</a>
                            </DropdownItem>
                        </DropdownMenu>
                    </UncontrolledDropdown>
                    <UncontrolledTooltip
                             target={'download_button_'+ this.props.mediaKey}>
                        Per default file will be downloaded server-sided.
                    </UncontrolledTooltip>
                </span>
            );
        else
            return (
                <Button color={this.props.color} disabled={this.state.isLoading}
                        onClick={this.handleClick.bind(this)}>{this.props.children}</Button>
            );
    }
}


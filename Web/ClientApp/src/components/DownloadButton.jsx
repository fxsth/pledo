import React from 'react';
import {Button} from "reactstrap";

export default class DownloadButton extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            mediaKey: this.props.mediaKey,
            isLoading: false,
            mediatype: this.props.mediaType
        };
    }

    // componentDidMount() {
    //     console.log('DownloadButton mounted with key' + this.state.mediaKey);
    // }


    handleClick(event) {
        console.log('Handle click is called for key: ' + this.state.mediaKey);
        this.setState({
            isLoading: true
        });
        this.SendDownloadRequest();

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
        return (<Button color={this.props.color} disabled={this.state.isLoading} onClick={this.handleClick.bind(this)}>{this.props.children}</Button>);
    }
}


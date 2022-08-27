import React from 'react';

export default class DownloadButton extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            mediaKey: this.props.mediaKey,
            isLoading: false
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

        return fetch('api/download/' + this.props.mediaKey, settings)
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
        return (<button disabled={this.state.isLoading} onClick={this.handleClick.bind(this)}>Download</button>);
    }
}


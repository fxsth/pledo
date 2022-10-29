import React, {Component} from 'react';

export class Overview extends Component {
    static displayName = Overview.name;

    constructor(props) {
        super(props);
        this.state = {
            account: null,
            servers: null,
        };
    }

    componentDidMount() {
        this.populateAccountData();
        this.populateServerData();
    }

    openInNewTab = async (event) => {
        const response = await fetch('api/account/loginuri');
        const data = await response.text();
        console.log('Login uri: ' + data);
        window.open(data, '_blank', 'noopener,noreferrer');
    };

    render() {

        if (this.state.account) {
            return (
                <div>
                    <h1>Hello, {this.state.account ? this.state.account.username : "User"}!</h1>
                    <p>Welcome to pledo, the Plex Downloader</p>
                    <p>You have access to following servers:</p>
                    <ul>
                        {this.state.servers ? this.state.servers.map(server =>
                            <li><strong>{server.sourceTitle == null ? server.name : server.sourceTitle}</strong> {server.name}</li>
                        ) : null}
                    </ul>
                </div>
            );
        } else {
            return (
                <div>
                    <h1>Hello, guest!</h1>
                    <p>Welcome to pledo, the Plex Downloader</p>
                    <p>To access movies and series, you have to log into your plex account.</p>
                    <div>
                        <a href={this.state.loginuri} target="_blank" rel="noopener noreferrer">
                            Login with plex
                        </a>
                        <hr/>
                        <button onClick={this.openInNewTab.bind(this)}>
                            Login with plex
                        </button>
                    </div>
                </div>
            );
        }
    }

    async populateAccountData() {
        const response = await fetch('api/account');
        const data = await response.json();
        this.setState({account: data, loading: false});
    }

    async populateLoginData() {
        const response = await fetch('api/account/loginuri');
        const data = await response.text();
        this.setState({loginuri: data, loading: false});
    }

    async populateServerData() {
        const response = await fetch('api/server');
        const data = await response.json();
        this.setState({servers: data, loading: false});
    }
}

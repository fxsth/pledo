import React, {Component} from 'react';

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = {
            account: null,
            servers: null,
        };
    }

    componentDidMount() {
        this.populateAccountData();
        if (this.state.account)
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
                        <li><a href='https://get.asp.net/'>ASP.NET Core</a> and <a
                            href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx'>C#</a> for cross-platform
                            server-side code
                        </li>
                        <li><a href='https://facebook.github.io/react/'>React</a> for client-side code</li>
                        <li><a href='http://getbootstrap.com/'>Bootstrap</a> for layout and styling</li>
                    </ul>
                    <p>To help you get started, we have also set up:</p>
                    <ul>
                        <li><strong>Client-side navigation</strong>. For example,
                            click <em>Counter</em> then <em>Back</em> to return here.
                        </li>
                        <li><strong>Development server integration</strong>. In development mode, the development server
                            from <code>create-react-app</code> runs in the background automatically, so your client-side
                            resources are dynamically built on demand and the page refreshes when you modify any file.
                        </li>
                        <li><strong>Efficient production builds</strong>. In production mode, development-time features
                            are
                            disabled, and your <code>dotnet publish</code> configuration produces minified, efficiently
                            bundled JavaScript files.
                        </li>
                    </ul>
                    <p>The <code>ClientApp</code> subdirectory is a standard React application based on
                        the <code>create-react-app</code> template. If you open a command prompt in that directory, you
                        can
                        run <code>npm</code> commands such as <code>npm test</code> or <code>npm install</code>.</p>
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

import React, {Component} from 'react';
import {
    Card,
    CardBody,
    CardImg,
    CardSubtitle,
    CardTitle, 
    Container,
    Row
} from "reactstrap";

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
                    <h2>Hello, {this.state.account ? this.state.account.username : "User"}!</h2>
                    <p>You have access to following servers:</p>
                    <Container>
                        <Row>
                            {this.state.servers ? this.state.servers.map(server =>
                                <Card className="my-2"
                                      color="secondary"
                                      inverse
                                      style={{
                                          width: '10rem',
                                          margin: '1rem'
                                      }}
                                >
                                    <CardImg
                                    style={{
                                        height: 20,
                                        background:'#e5a00d'
                                    }}
                                    top
                                    width="100%"
                                />
                                    <CardBody>
                                        <CardTitle tag="h5">
                                            {server.name}
                                        </CardTitle>
                                        <CardSubtitle>
                                            {server.sourceTitle}
                                        </CardSubtitle>
                                    </CardBody>
                                </Card>
                            ) : null}
                        </Row>
                    </Container>
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

    async populateServerData() {
        const response = await fetch('api/server');
        const data = await response.json();
        this.setState({servers: data, loading: false});
    }
}

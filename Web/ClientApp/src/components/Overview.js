import React, {Component} from 'react';
import {
    Card,
    CardBody,
    CardImg,
    CardSubtitle,
    CardTitle,
    Container,
    Row, Spinner
} from "reactstrap";
import {SyncButton} from "./SyncButton";

export class Overview extends Component {
    static displayName = Overview.name;

    constructor(props) {
        super(props);
        this.state = {
            account: null,
            servers: null,
            loginPending: false,
            popup: null,
            syncing: false
        };
    }

    componentDidMount() {
        this.populateAccountData()
        this.populateServerData()
    }

    openInNewTab = async (event) => {
        const response = await fetch('api/account/loginuri');
        const data = await response.text();
        console.log('Login uri: ' + data);
        this.setState({loginPending: true})
        const popupWindow = window.open(data, '_blank');
        this.setState({popup: popupWindow})
        clearInterval(this.timerID);
        this.startAccountPolling()
    };

    startAccountPolling() {
        this.timerID = setInterval(
            () => {
                if (!this.state.account)
                    this.populateAccountData().then(data => {
                        if (data) {
                            this.state.popup?.close()
                        }
                    })
                else if (this.state.loginPending && !this.state.syncing)
                    this.syncConnections()
                else
                    this.populateServerData().then((data) => {
                        if (data) {
                            clearInterval(this.timerID);
                            this.setState({loginPending: false, syncing: false})
                        }
                    })

            },
            3000
        );
    }

    render() {
        if (this.state.account || this.state.loginPending) {
            return (
                <div>
                    <h2>Hello, {this.state.account ? this.state.account.username : "User"}!</h2>
                    {this.state.loginPending || this.state.syncing ? <Spinner size="sm">Loading...</Spinner> :
                        <SyncButton whenSyncFinished={() => {
                            console.log("Sync finished")
                            this.populateServerData()
                        }}/>}
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
                                            background: server.isOnline ? 
                                                '#19d37b' :
                                                '#ff413c'
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
        if (data)
            this.setState({account: data});
        return data
    }

    async populateServerData() {
        const response = await fetch('api/server');
        const data = await response.json();
        if (data)
            this.setState({servers: data});
        return data
    }

    async syncConnections() {
        if (!this.state.syncing) {
            this.setState({syncing: true});
            fetch('api/sync?syncType=1', {method: 'POST'})
                .then(response => {
                    if (response.status >= 200 && response.status < 300) {
                        console.log(response);
                    } else {
                        // alert('There was a problem with syncing. Please try again.');
                    }
                }).catch(err => console.log(err));
            // this.startSyncPolling();
        }
    }
}

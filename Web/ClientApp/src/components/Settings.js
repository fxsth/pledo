import React, {Component} from 'react';
import AccountForm from './AccountForm';

export class Settings extends Component {
    static displayName = Settings.name;

    constructor(props) {
        super(props);
        this.state = {servers: [], loading: true};
    }

    componentDidMount() {
        this.populateData();
    }

    static renderAccountTable(servers) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                <tr>
                    <th>Name</th>
                    <th>AccessToken</th>
                </tr>
                </thead>
                <tbody>
                {servers.map(server =>
                    <tr key={server.id}>
                        <td>{server.name}</td>
                        <td>{server.accessToken}</td>
                    </tr>
                )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Settings.renderAccountTable(this.state.servers);

        return (
            <div>
                <h1 id="tabelLabel">Plex Accounts</h1>
                <p>This component demonstrates fetching data from the server.</p>
                {contents}
                <AccountForm/>
            </div>
        );
    }

    async populateData() {
        const response = await fetch('api/server');
        const data = await response.json();
        this.setState({servers: data, loading: false});
    }

    async startSync() {
        const response = await fetch('api/server');
        const data = await response.json();
        this.setState({servers: data, loading: false});
    }
}

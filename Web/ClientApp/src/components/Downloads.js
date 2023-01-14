import React, {Component} from 'react';
import {Badge, Progress, Table} from "reactstrap";

export class Downloads extends Component {
    static displayName = Downloads.name;

    constructor(props) {
        super(props);
        this.state = {downloads: [], loading: true};
    }

    componentDidMount() {
        this.populateData();
        this.timerID = setInterval(
            () => this.populateData(),
            2000
        );
    }

    componentWillUnmount()
    {
        clearInterval(this.timerID);
    }

    renderDownloadTable(downloads) {
        return (
            <Table>
                <thead>
                <tr>
                    <th>Name</th>
                    <th>Started at</th>
                    <th>Status</th>
                </tr>
                </thead>
                <tbody>
                {downloads.map(download =>
                    <tr key={download.id}>
                        <td>{download.name}</td>
                        <td>{(new Date(download.started)).toLocaleString()}</td>
                        <td>
                            {download.progress != null ?
                                download.progress < 1 ?
                                    (<Progress visible={true} value={download.progress * 100}>
                                            {Math.round(download.progress * 100)}%
                                        </Progress>
                                    ) : (<Badge color="success" pill>Finished</Badge>) : ("Pending")
                            }
                        </td>
                    </tr>
                )}
                </tbody>
            </Table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderDownloadTable(this.state.downloads);

        return (
            <div>
                <h1 id="tabelLabel">Downloads</h1>
                <p>Successful, failed, ongoing and pending downloads:</p>
                {contents}
            </div>
        );
    }

    async populateData() {
        const response = await fetch('api/download');
        const data = await response.json();
        this.setState({downloads: data, loading: false});
    }
}

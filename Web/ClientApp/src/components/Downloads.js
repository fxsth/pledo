import React, {Component} from 'react';
import {Badge, Button, Progress, Table} from "reactstrap";

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
                    <th>Cancel</th>
                </tr>
                </thead>
                <tbody>
                {downloads.map(download =>
                    <tr key={download.id}>
                        <td>{download.name}</td>
                        <td>{download.started!= null ? (new Date(download.started)).toLocaleString() : ""}</td>
                        <td>
                            {download.started != null ?
                                download.progress < 1 ?
                                    download.finished == null ?
                                    (<Progress visible={true} value={download.progress * 100}>
                                            {Math.round(download.progress * 100)}%
                                        </Progress>
                                    ) : <Badge color="danger" pill>Cancelled</Badge> : (<Badge color="success" pill>Finished</Badge>) : (<Badge color="info" pill>Pending</Badge>)
                            }
                        </td>
                        <td>
                            {(download.progress == null || download.progress<1) && download.finished == null ? 
                                <Button color="danger" outline size="sm" onClick={() => this.handleClick(download.mediaKey)}>Cancel</Button>
                                :
                                ""}
                        </td>
                    </tr>
                )}
                </tbody>
            </Table>
        );
    }
    
    handleClick(key){
        const settings = {
            method: 'DELETE'
        };
        fetch('api/download/' + key, settings)
            .then(response => {
                if (response.status >= 200 && response.status < 300) {
                    console.log("Download cancelled.");
                } else {
                    alert('Could not cancel download due to an unknown error.');
                }
            })
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderDownloadTable(this.state.downloads);

        return (
            <div>
                <h1 id="tabelLabel">Downloads</h1>
                <p>Your download history:</p>
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

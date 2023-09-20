import React, {Component} from 'react';
import {Button, Spinner} from "reactstrap";

export class SyncButton extends Component {
    constructor(props) {
        super(props);
        this.state = {tasks: [], loading: true};
    }

    componentDidMount() {
        this.startSyncPolling();
    }

    componentWillUnmount() {
        this.stopSyncPolling()
    }

    handleClick = () => {
        this.startSync();
    }

    startSyncPolling() {
        this.setState({loading: false});
        this.timerID = setInterval(
            () => this.populateTaskData(),
            1000
        );
    }

    stopSyncPolling() {
        clearInterval(this.timerID);
    }

    isSyncOngoing = () => this.state.tasks.some(task => task.type === 0)

    render() {
        if (this.isSyncOngoing()) {
            return <Button color="primary" disabled={this.isSyncOngoing()}>
                <Spinner size="sm">Loading...</Spinner>
                <span>{'  ' + this.state.tasks[0].name} </span>
            </Button>
        } else
            return <Button color="primary" onClick={this.handleClick.bind(this)}>Sync all data now</Button>
    }

    async populateTaskData() {
        if(this.state.loading)
            return
        this.setState({loading: true});
        const response = await fetch('api/task');
        const data = await response.json();
        const isSyncing = data.some(task => task.type === 0)
        if (!isSyncing) {
            this.stopSyncPolling()
            if(this.isSyncOngoing())
                this.whenSyncFinished()
        }
        this.setState({tasks: data, loading: false});
    }
    
    whenSyncFinished()
    {
        if(this.props.whenSyncFinished)
            this.props.whenSyncFinished();
    }

    async startSync() {
        const settings = {
            method: 'POST',
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            }
        };
        fetch('api/sync', settings)
            .then(response => {
                if (response.status >= 200 && response.status < 300 ||response.status === 409) {
                    console.log(response);
                } else {
                    alert('There was a problem with syncing. Please try again.');
                }
            }).catch(err => console.log(err));
        this.startSyncPolling();
    }
}

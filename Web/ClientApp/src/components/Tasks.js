import React, {Component} from 'react';

export class Tasks extends Component {
    static displayName = Tasks.name;

    constructor(props) {
        super(props);
        this.state = {tasks: [], loading: true};
    }

    componentDidMount() {
        this.timerID = setInterval(
            () => this.populateTaskData(),
            2000
        );
    }
    
    handleClick = ()=>{
        this.startSync();
    }
    
    isSyncOngoing = ()=> this.state.tasks.some(task=>task.type === 0)

    static renderTaskTable(tasks) {
        return (
            <table>
                <thead>
                <tr>
                    <th>Id</th>
                    <th>Name</th>
                    <th>Type</th>
                    <th>Progress</th>
                </tr>
                </thead>
                <tbody>
                {tasks.map(task =>
                    <tr key={task.id}>
                        <td>{task.id}</td>
                        <td>{task.name}</td>
                        <td>{task.type}</td>
                        <td>{task.progress == null ? "" : Math.round(task.progress * 100) + "%"}</td>
                    </tr>
                )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Tasks.renderTaskTable(this.state.tasks);

        return (
            <div>
                <h1 id="tabelLabel">Sync and download tasks</h1>
                <p>Here you find information an progress of ongoing tasks.</p>
                <button onClick={this.handleClick.bind(this)}>Sync all data now</button>
                {contents}
            </div>
        );
    }
    
    async populateTaskData() {
        const response = await fetch('api/task');
        const data = await response.json();
        this.setState({ tasks: data, loading: false });
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
                if (response.status >= 200 && response.status < 300) {
                    console.log(response);
                } else {
                    alert('There was a problem with syncing. Please try again.');
                }
            }).catch(err => console.log(err));
    }
}

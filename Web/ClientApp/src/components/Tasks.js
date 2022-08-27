import React, { Component } from 'react';

export class Tasks extends Component {
  static displayName = Tasks.name;

  constructor(props) {
    super(props);
    this.state = { tasks: [], loading: true };
  }

  componentDidMount() {
    this.timerID = setInterval(
        () => this.populateTaskData(),
        2000
    );
  }

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
              <td>{Math.round(task.progress*100)}%</td>
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
        <h1 id="tabelLabel" >Sync and download tasks</h1>
        <p>Here you find information an progress of ongoing tasks.</p>
        {contents}
      </div>
    );
  }

  async populateTaskData() {
    const response = await fetch('api/task');
    const data = await response.json();
    this.setState({ tasks: data, loading: false });
  }
}

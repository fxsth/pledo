import React, { Component } from 'react';
import Dropdown from "./Dropdown";
import {DropdownMenu} from "reactstrap";

export class Movies extends Component {
  static displayName = Movies.name;

  constructor(props) {
    super(props);
    this.state = { servers: [], libraries:[], movies: [], loading: true };
  }

  componentDidMount() {
    this.populateServersData();
  }

  handleServerChange = (event) => {
    this.populateLibrariesData(event.target.value);
  };

  handleLibraryChange = (event) => {
    // setDrink(event.target.value);
  };


  static renderServerDropdown(servers) {
    const list = servers.map((server)=>
        ({ label: server.name, value: server.id })
    )
    
    return (
      <Dropdown name="servers"
                title="Select server"
                list={list}
                onChange={this.onChange}
      />
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Movies.renderServerDropdown(this.state.servers);

    return (
      <div>
        <h1 id="tabelLabel" >Movies</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {/*<Dropdown/>*/}
        {contents}
      </div>
    );
  }

  async populateServersData() {
    const response = await fetch('api/server');
    const data = await response.json();
    this.setState({ servers: data, loading: false });
  }
  async populateLibrariesData(server) {
    const response = await fetch('api/library?server=${encodeURIComponent(server)}');
    const data = await response.json();
    this.setState({ libraries: data, loading: false });
  }
}

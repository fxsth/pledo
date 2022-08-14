import React, {Component} from 'react';
import Dropdown from "./Dropdown";

export class Movies extends Component {
    static displayName = Movies.name;

    constructor(props) {
        super(props);
        this.state = {servers: [], libraries: [], movies: [], serverloading: true, libraryloading: true, movieloading: true};
    }

    componentDidMount() {
        this.populateServersData();
    }

    handleServerChange = (event) => {
        this.setState({libraryloading:true})
        if(event.target.value != null) {
            this.populateLibrariesData(event.target.value);
        }
    };

    handleLibraryChange = (event) => {
        // setDrink(event.target.value);
    };


    renderServerDropdown(servers) {
        const list = servers.map((server) =>
            ({label: server.name, value: server.id})
        )

        return (
            <Dropdown name="servers"
                      title="Select server"
                      list={list}
                      onChange={this.handleServerChange}
            />
        );
    }

    renderLibraryDropdown(libraries) {
        const list = libraries.map((library) =>
            ({
                label: library.name,
                value: library.id
            })
        )

        return (
            <Dropdown name="libraries"
                      title="select libraries"
                      list={list}
                      onChange={this.handleServerChange}
            />
        );
    }

    render() {
        let serverDropdown = this.state.serverloading
            ? <p><em>Loading...</em></p>
            : this.renderServerDropdown(this.state.servers);

        let libraryDropdown = this.state.libraryloading
            ? <p><em>Loading...</em></p>
            : this.renderLibraryDropdown(this.state.libraries);

        return (
            <div>
                <h1 id="tabelLabel">Movies</h1>
                <p>This component demonstrates fetching data from the server.</p>
                {/*<Dropdown/>*/}
                {serverDropdown}
                {libraryDropdown}
            </div>
        );
    }

    async populateServersData() {
        const response = await fetch('api/server');
        const data = await response.json();
        this.setState({servers: data, serverloading: false});
    }

    async populateLibrariesData(server) {
        const uri = 'api/library?' + new URLSearchParams({
            server: server
        });
        const response = await fetch(uri);
        const data = await response.json();
        this.setState({libraries: data, libraryloading: false});
    }
}

import React, {Component} from 'react';
import Dropdown from "./Dropdown";
import DownloadButton from "./DownloadButton";

export class Movies extends Component {
    static displayName = Movies.name;

    constructor(props) {
        super(props);
        this.state = {
            servers: [],
            libraries: [],
            movies: [],
            serverselected: false,
            libraryselected: false,
            serverloading: true,
            libraryloading: true,
            movieloading: true
        };
    }

    componentDidMount() {
        this.populateServersData();
    }

    handleServerChange = (event) => {
        this.setState({serverselected: true, libraryselected: false, libraryloading: true})
        if (event.target.value != null) {
            this.populateLibrariesData(event.target.value);
        } else {
            this.setState({serverselected: false});
        }
    };

    handleLibraryChange = (event) => {
        this.setState({libraryselected: true, movieloading: true})
        if (event.target.value != null) {
            this.populateMoviesData(event.target.value);
        } else {
            this.setState({libraryselected: false});
        }
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
                      onChange={this.handleLibraryChange}
            />
        );
    }

    render() {
        let serverDropdown = this.state.serverloading
            ? <p><em>Loading servers...</em></p>
            : this.renderServerDropdown(this.state.servers);

        let libraryDropdown = this.state.serverselected
            ? this.state.libraryloading
                ? <p><em>Loading libraries...</em></p>
                : this.renderLibraryDropdown(this.state.libraries)
            : <p/>;

        let moviesContent = this.state.libraryselected
            ? this.state.movieloading
                ? <p><em>Loading movies...</em></p>
                : Movies.renderMoviesTable(this.state.movies)
            : <p/>;

        return (
            <div>
                <h1 id="tabelLabel">Movies</h1>
                <p>Select server and library to see a list of all movies.</p>
                <br/>
                {serverDropdown}
                <br/>
                {libraryDropdown}
                <br/>
                {moviesContent}
            </div>
        );
    }

    static renderMoviesTable(movies) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                <tr>
                    <th>Title</th>
                    <th>Key</th>
                    <th>Rating Key</th>
                    <th>Download Url</th>
                </tr>
                </thead>
                <tbody>
                {movies.map(movie =>
                    <tr key={movie.title}>
                        <td>{movie.title}</td>
                        <td>{movie.key}</td>
                        <td>{movie.ratingKey}</td>
                        <td><DownloadButton mediaKey={movie.ratingKey}/></td>
                    </tr>
                )}
                </tbody>
            </table>
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

    async populateMoviesData(libraryId) {
        const uri = 'api/movie?' + new URLSearchParams({
            libraryId: libraryId
        });
        const response = await fetch(uri);
        const data = await response.json();
        this.setState({movies: data, movieloading: false});
    }
}

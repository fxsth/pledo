import React, {Component} from 'react';
import Dropdown from "./Dropdown";
import DownloadButton from "./DownloadButton";

export class TvShows extends Component {
    static displayName = TvShows.name;

    constructor(props) {
        super(props);
        this.state = {
            servers: [],
            libraries: [],
            tvshows: [],
            episodes: [],
            serverselected: false,
            libraryselected: false,
            tvshowselected: false,
            serverloading: true,
            libraryloading: true,
            tvshowsloading: true,
            episodesloading: true,
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
            this.populateTvShowsData(event.target.value);
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

        let tvshowsContent = this.state.libraryselected
            ? this.state.movieloading
                ? <p><em>Loading movies...</em></p>
                : TvShows.renderEpisodesTable(this.state.tvshows)
            : <p/>;

        return (
            <div>
                <h1 id="tabelLabel">TV Shows</h1>
                <p>Select server and library to see a list of all tv shows.</p>
                <br/>
                {serverDropdown}
                <br/>
                {libraryDropdown}
                <br/>
                {tvshowsContent}
            </div>
        );
    }

    static renderEpisodesTable(episodes) {
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
                {episodes.map(episode =>
                    <tr key={episode.title}>
                        <td>{episode.title}</td>
                        <td>{episode.key}</td>
                        <td>{episode.ratingKey}</td>
                        <td><DownloadButton mediaKey={episode.ratingKey}/></td>
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

    async populateTvShowsData(libraryId) {
        const uri = 'api/tvshow?' + new URLSearchParams({
            libraryId: libraryId
        });
        const response = await fetch(uri);
        const data = await response.json();
        this.setState({movies: data, movieloading: false});
    }
}

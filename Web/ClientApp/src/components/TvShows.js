import React, {Component} from 'react';
import Dropdown from "./Dropdown";
import DownloadButton from "./DownloadButton";
import CollapsibleTableRow from "./CollapsibleTableRow";
import {Button, ButtonGroup, ButtonToolbar, Card, CardBody} from "reactstrap";

export class TvShows extends Component {
    static displayName = TvShows.name;

    constructor(props) {
        super(props);
        this.state = {
            servers: [],
            libraries: [],
            tvshows: [],
            serverselected: false,
            libraryselected: false,
            tvshowselected: false,
            serverloading: true,
            libraryloading: true,
            tvshowloading: true
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
            ? this.state.tvshowloading
                ? <p><em>Loading TV shows...</em></p>
                : TvShows.renderTvShowsTable(this.state.tvshows)
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

    static renderTvShowsTable(tvShows) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <tbody>
                {tvShows.map(tvShow =>
                    <CollapsibleTableRow label={tvShow.title}>
                        <Card>
                            <CardBody>
                                Download season or complete tv show 
                                <ButtonToolbar>
                                    <ButtonGroup className="me-2">
                                        {
                                            tvShow.episodes.map(item => item.seasonNumber)
                                                .filter((value, index, self) => self.indexOf(value) === index).sort((a,b) => a-b).map(
                                                    seasonNumber =>
                                                        <DownloadButton mediaType='tvshow' mediaKey={tvShow.ratingKey} season={seasonNumber}>
                                                            Season {seasonNumber}
                                                        </DownloadButton>
                                            )
                                            
                                        }
                                    </ButtonGroup>
                                    <ButtonGroup>
                                        <DownloadButton color="info" mediaType='tvshow' mediaKey={tvShow.ratingKey}>
                                            Complete TV Show
                                        </DownloadButton>
                                    </ButtonGroup>
                                </ButtonToolbar>
                            </CardBody>
                        </Card>

                        <table className='table table-striped' aria-labelledby="tabelLabel2">
                            <thead>
                            <tr>
                                <th>Season & Episode</th>
                                <th>Episode Title</th>
                                <th>Year</th>
                                <th>Video Codec</th>
                                <th>Resolution</th>
                                <th>Size</th>
                                <th>Download</th>
                            </tr>
                            </thead>
                            <tbody>
                            {tvShow.episodes.map(episode =>
                                <tr>
                                    <td>S{episode.seasonNumber}E{episode.episodeNumber}</td>
                                    <td>{episode.title}</td>
                                    <td>{episode?.year}</td>
                                        <td>{episode.mediaFiles[0].videoCodec}</td>
                                        <td>{episode.mediaFiles[0].videoResolution}</td>
                                        <td>{this.humanizeByteSize(episode.mediaFiles[0].totalBytes)}</td>
                                    <td><DownloadButton mediaType='episode' mediaKey={episode.ratingKey}>Download</DownloadButton></td>
                                </tr>
                                )}
                            </tbody>
                        </table>
                    </CollapsibleTableRow>
                )}
                </tbody>
            </table>
        );
    }

    static humanizeByteSize(size) {
        if(!size)
            return "--";
        const i = size === 0 ? 0 : Math.floor(Math.log(size) / Math.log(1024));
        return (size / Math.pow(1024, i)).toFixed(2) * 1 + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
    }

    async populateServersData() {
        const response = await fetch('api/server');
        const data = await response.json();
        this.setState({servers: data, serverloading: false});
    }

    async populateLibrariesData(server) {
        const uri = 'api/library?' + new URLSearchParams({
            server: server,
            mediaType: 'show'
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
        this.setState({tvshows: data, tvshowloading: false});
    }
}

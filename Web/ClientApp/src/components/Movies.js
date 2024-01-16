import React, {Component, useState} from 'react';
import Dropdown from "./Dropdown";
import DownloadButton from "./DownloadButton";
import {Table} from "reactstrap";
import {Dropdown2} from "./Dropdown2";
import {LibrarySelector} from "./LibrarySelector";

export class Movies extends Component {
    static displayName = Movies.name;

    constructor(props) {
        super(props);
        this.state = {
            movies: [],
            selectedServer: null,
            selectedLibrary: null,
            movieloading: true,
        };
    }

    render() {

        let moviesContent = this.state.movies.length === 0
            ? <p><em>Loading movies...</em></p>
            : Movies.renderMoviesTable(this.state.movies, this.state.selectedServer);

        return (
            <div>
                <h1 id="tabelLabel">Movies</h1>
                <p>Select server and library to see a list of all movies.</p>
                <br/>
                <LibrarySelector onServerSelected={server => {
                    console.log(`onServerSelected: ${server}`)
                    this.setState({selectedServer: server})
                }} onLibrarySelected={library => {
                    console.log(`onLibrarySelected: ${library}`)
                    this.populateMoviesData(library)
                }}/>
                <br/>
                {moviesContent}
            </div>
        );
    }

    static renderMoviesTable(movies, selectedServer) {
        return (
            <Table striped>
                <thead>
                <tr>
                    <th>Title</th>
                    <th>Year</th>
                    <th>Video Codec</th>
                    <th>Resolution</th>
                    <th>Size</th>
                    <th>Download</th>
                </tr>
                </thead>
                <tbody>
                {movies.map(movie =>
                    movie.mediaFiles.map(mediaFile =>
                        <tr key={mediaFile.downloadUri}>
                            <td>{movie.title}</td>
                            <td>{movie.year}</td>
                            <td>{mediaFile.videoCodec}</td>
                            <td>{mediaFile.videoResolution}</td>
                            <td>{this.humanizeByteSize(mediaFile.totalBytes)}</td>
                            <td><DownloadButton
                                mediaType='movie'
                                mediaKey={movie.ratingKey}
                                mediaFileKey={mediaFile.downloadUri}
                                mediaFile={mediaFile}
                                server={selectedServer}
                                downloadBrowserPossible={true}>Download</DownloadButton></td>
                        </tr>)
                )}
                </tbody>
            </Table>
        );
    }

    static humanizeByteSize(size) {
        if (!size)
            return "--";
        const i = size === 0 ? 0 : Math.floor(Math.log(size) / Math.log(1024));
        return (size / Math.pow(1024, i)).toFixed(2) * 1 + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
    }

    async populateMoviesData(libraryId) {
        const uri = 'api/media/movie?' + new URLSearchParams({
            libraryId: libraryId,
            pageNumber: 1,
            pageSize: 100
        });
        const response = await fetch(uri);
        const data = await response.json();
        this.setState({movies: data, movieloading: false});
    }
}

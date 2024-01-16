import React, {useState} from 'react';
import {LibrarySelector} from "./LibrarySelector";
import {MoviesTable} from "./MoviesTable";
import {Spinner} from "reactstrap";

export function Movies(props) {
    const [movies, setMovies] = useState([]);
    const [selectedServer, setSelectedServer] = useState(null);
    const [movieLoading, setMovieLoading] = useState(true);

    const populateMoviesData = async (libraryId) => {
        const uri = 'api/media/movie?' + new URLSearchParams({
            libraryId: libraryId,
            pageNumber: 1,
            pageSize: 100
        });
        const response = await fetch(uri);
        const data = await response.json();
        setMovies(data)
        setMovieLoading(false)
    }
    
    return (
        <div>
            <h1 id="tabelLabel">Movies</h1>
            <p>Select server and library to see a list of all movies.</p>
            <br/>
            <LibrarySelector
                mediaType={"movie"}
                onServerSelected={setSelectedServer}
                onLibrarySelected={library => populateMoviesData(library)}/>
            <br/>
            {movieLoading ?
            <Spinner>Loading...</Spinner> :
            <MoviesTable items={movies} selectedServer={selectedServer}/>}
        </div>
    );
    
}

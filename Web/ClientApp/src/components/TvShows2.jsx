import React, {useState} from 'react';
import {LibrarySelector} from "./LibrarySelector";
import {MoviesTable} from "./MoviesTable";
import {Spinner} from "reactstrap";
import {TvShowsTable} from "./TvShowsTable";

export function TvShows2(props) {
    const [items, setItems] = useState([]);
    const [selectedServer, setSelectedServer] = useState(null);
    const [loading, setLoading] = useState(true);

    const populateData = async (libraryId) => {
        const uri = 'api/media/tvshow?' + new URLSearchParams({
            libraryId: libraryId,
            pageNumber: 1,
            pageSize: 10
        });
        const response = await fetch(uri);
        const data = await response.json();
        setItems(data)
        setLoading(false)
    }

    return (
        <div>
            <h1 id="tabelLabel">TV Shows</h1>
            <p>Select server and library to see a list of all tv shows.</p>
            <br/>
            <LibrarySelector
                mediaType={"show"}
                onServerSelected={setSelectedServer}
                onLibrarySelected={library => {
                    setLoading(true)
                    populateData(library)
                }}/>
            <br/>
            {loading ?
                <Spinner>Loading...</Spinner> :
                <TvShowsTable items={items} selectedServer={selectedServer}/>}
        </div>
    );

}

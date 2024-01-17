import React, {useEffect, useState} from 'react';
import {LibrarySelector} from "./LibrarySelector";
import {Spinner} from "reactstrap";
import {PaginationRow} from "./Pagination";
import {TvShowsTable} from "./TvShowsTable";

function PaginatedTableContainer({libraryId, server}) {
    const [items, setItems] = useState({items: [], totalItems: 0, loading: true});
    const [loading, setLoading] = useState(true);
    const [pageNumber, setPageNumber] = useState(0);
    const pageSize = 10;
    
    useEffect(() => {
        setPageNumber(0)
    }, [libraryId])
    useEffect(() => {
        populateData(libraryId, pageNumber)
    }, [libraryId, pageNumber])
    const populateData = async (libraryId, pageNumber) => {
        const uri = 'api/media/tvshow?' + new URLSearchParams({
            libraryId: libraryId,
            pageNumber: pageNumber + 1,
            pageSize: pageSize
        });
        const response = await fetch(uri);
        const data = await response.json();
        setItems({items: data.items, totalItems: data.totalItems, loading: false})
    }

    return (
        <div>
            {items.loading ?
                <Spinner>Loading...</Spinner> :
                <div>
                    <PaginationRow pages={Math.ceil(items.totalItems / pageSize)} currentPage={pageNumber}
                                   selectPage={setPageNumber}/>
                    <TvShowsTable items={items.items} knownServer={[server]}/>
                </div>
            }
        </div>
    );
}

export function TvShows(props) {
    const [selectedServer, setSelectedServer] = useState(null);
    const [selectedLibrary, setSelectedLibrary] = useState(null);

    return (
        <div>
            <h1 id="tabelLabel">TV Shows</h1>
            <p>Select server and library to see a list of all tv shows.</p>
            <br/>
            <LibrarySelector
                mediaType={"show"}
                onServerSelected={setSelectedServer}
                onLibrarySelected={setSelectedLibrary}/>
            <br/>
            <PaginatedTableContainer libraryId={selectedLibrary} server={selectedServer}/>
        </div>
    );
}

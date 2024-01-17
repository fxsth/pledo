import React, {useEffect, useState} from 'react';
import {LibrarySelector} from "./LibrarySelector";
import {MoviesTable} from "./MoviesTable";
import {Spinner} from "reactstrap";
import {PaginationRow} from "./Pagination";

function PaginatedTableContainer({libraryId, server}) {
    const [items, setItems] = useState({items: [], totalItems: 0, loading: true});
    const [pageNumber, setPageNumber] = useState(0);
    const pageSize = 100;

    useEffect(() => {
        setPageNumber(0)
    }, [libraryId])
    useEffect(() => {
        populateData(libraryId, pageNumber)
    }, [libraryId, pageNumber])
    const populateData = async (libraryId, pageNumber) => {
        const uri = 'api/media/movie?' + new URLSearchParams({
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
                    <MoviesTable items={items.items} selectedServer={server}/>
                </div>
            }
        </div>
    );
}

export function Movies(props) {
    const [selectedServer, setSelectedServer] = useState(null);
    const [selectedLibrary, setSelectedLibrary] = useState(null);

    return (
        <div>
            <h1 id="tabelLabel">Movies</h1>
            <p>Select server and library to see a list of all movies.</p>
            <br/>
            <LibrarySelector
                mediaType={"movie"}
                onServerSelected={setSelectedServer}
                onLibrarySelected={setSelectedLibrary}/>
            <br/>
            <PaginatedTableContainer libraryId={selectedLibrary} server={selectedServer}/>
        </div>
    );

}

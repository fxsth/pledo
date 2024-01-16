import React, {useState} from 'react';
import {LibrarySelector} from "./LibrarySelector";
import {MoviesTable} from "./MoviesTable";
import {PaginationItem, PaginationLink, Spinner} from "reactstrap";

export function Pagination() {
    const [pageNumber, setPageNumber] = useState([]);

    return (
        <Pagination>
            <PaginationItem>
                <PaginationLink
                    previous
                />
            </PaginationItem>
            <PaginationItem>
                <PaginationLink>
                    1
                </PaginationLink>
            </PaginationItem>

            <PaginationItem>
                <PaginationLink
                    next
                />
            </PaginationItem>
        </Pagination>
    );

}

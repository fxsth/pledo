import React from 'react';
import {Pagination, PaginationItem, PaginationLink} from "reactstrap";

export function PaginationRow({ pages, currentPage, selectPage }) {
    const pageClick = (e, page) => {
        e.preventDefault();
        selectPage(page);
    };

    const pagesToShow = 7;
    const firstPageToShow = currentPage - Math.floor(pagesToShow / 2);
    const firstPage = firstPageToShow < 0 ? 0 : firstPageToShow;
    const lastPageToShow = currentPage + Math.floor(pagesToShow / 2);
    const lastPage = lastPageToShow > pages ? pages : firstPage + pagesToShow;
    const displayArray = [...Array(pages).keys()].slice(firstPage, lastPage);

    return (
        <Pagination>
            {pages > pagesToShow && <PaginationItem disabled={currentPage === 0}><PaginationLink onClick={e => pageClick(e, 0)}>&laquo;</PaginationLink></PaginationItem>}
            {pages > pagesToShow && <PaginationItem disabled={currentPage === 0}><PaginationLink onClick={e => pageClick(e, currentPage - 1)}>&lsaquo;</PaginationLink></PaginationItem>}
            {displayArray.map(p => (<PaginationItem disabled={currentPage === p} key={p}><PaginationLink onClick={e => pageClick(e, p)}>{p + 1}</PaginationLink></PaginationItem>))}
            {pages > pagesToShow && <PaginationItem disabled={currentPage === pages - 1}><PaginationLink onClick={e => pageClick(e, currentPage + 1)}>&rsaquo;</PaginationLink></PaginationItem>}
            {pages > pagesToShow && <PaginationItem disabled={currentPage === pages - 1}><PaginationLink onClick={e => pageClick(e, pages - 1)}>&raquo;</PaginationLink></PaginationItem>}
        </Pagination>
    );

}

import React, {useState} from 'react';
import {MoviesTable} from "./MoviesTable";
import {
    Accordion, AccordionBody,
    AccordionHeader,
    AccordionItem,
    Button,
    Col,
    Input,
    Label,
    Row,
    Spinner
} from "reactstrap";
import {TvShowsTable} from "./TvShowsTable";

export function Search(props) {
    const [search, setSearch] = useState({results: null, loading: false});
    const [searchTerm, setSearchTerm] = useState("");

    const searchRequest = async () => {
        setSearch({loading: true, results: null})
        const uri = 'api/media/search?' + new URLSearchParams({
            searchTerm: searchTerm
        });
        const response = await fetch(uri);
        console.log(`response is ok: ${response.ok}`)
        if (response.ok) {
            const data = await response.json();
            setSearch({results: data, loading: false})
        } else
            setSearch({results: null, loading: false})
    }

    return (
        <div>
            <Row className="row-cols-lg-auto g-3 align-items-center">
                <Col>
                    <Label className="visually-hidden"
                           for="searchInput">
                        Email
                    </Label>
                    <Input
                        id="searchInput"
                        name="search"
                        placeholder="Search for ..."
                        onChange={e => setSearchTerm(e.target.value)}
                    />
                </Col>
                <Col>
                    <Button onClick={searchRequest}>
                        Search
                    </Button>
                </Col>
            </Row>
            <br/>
            {search.loading ?
                <Spinner>Loading...</Spinner> :
                search.results != null ?
                    <SearchResults results={search.results}/> :
                    null}
        </div>
    );
}

function SearchResults({results}) {
    if (results == null)
        return

    return (
        <div>
            <SearchResultAccordion title={`Movies (${results.totalMoviesMatching} matching the search term)`}>
                {results.movies ? <MoviesTable items={results.movies} selectedServer={null}/> : null}
            </SearchResultAccordion>
            <SearchResultAccordion title={`TV Shows (${results.totalTvShowsMatching} matching the search term)`}>
                {results.tvShows ? <TvShowsTable items={results.tvShows} selectedServer={null}/> : null}
            </SearchResultAccordion>
        </div>
    )
}

function SearchResultAccordion({title, children}) {
    const [open, setOpen] = useState('0');
    const toggle = (id) => {
        if (open === id) {
            setOpen();
        } else {
            setOpen(id);
        }
    };

    return (
        <div>
            <Accordion open={open} toggle={toggle}>
                <AccordionItem>
                    <AccordionHeader targetId="1">{title}</AccordionHeader>
                    <AccordionBody accordionId="1">
                        {children}
                    </AccordionBody>
                </AccordionItem>
            </Accordion>
        </div>
    )
}

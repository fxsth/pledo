import React, {useEffect, useState} from 'react';
import {Accordion, AccordionBody, AccordionHeader, AccordionItem, Badge, Col, List, Row} from "reactstrap";
import DownloadButton from "./DownloadButton";

export function Playlists() {
    const [loading, setLoading] = useState(false);
    const [playlists, setPlaylists] = useState([]);

    useEffect(() => {
        setLoading(true);
        populateData().then(x => {
            setPlaylists(x);
            setLoading(false);
        });
    }, []);
    const [open, setOpen] = useState('1');
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
                {playlists.map((playlist) =>
                    <AccordionItem>
                        <AccordionHeader targetId={playlist.id}>
                            <Row>
                                <Col>
                                    {playlist.name}
                                </Col>
                                <Col>
                                    <Badge>{playlist.server.name}</Badge>
                                </Col>
                            </Row>
                        </AccordionHeader>
                        <AccordionBody accordionId={playlist.id}>
                            <List>
                                {playlist.items.map((item) =>
                                    <li>
                                        {item.name}
                                    </li>)}

                            </List>
                            <DownloadButton mediaKey={playlist.id} mediaType="playlist">Download playlist</DownloadButton>
                        </AccordionBody>
                    </AccordionItem>)
                }
            </Accordion>
        </div>
    );
}

async function populateData() {
    const response = await fetch('api/playlist');
    return await response.json();
}

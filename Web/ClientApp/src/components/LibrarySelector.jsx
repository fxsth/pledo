import React, {useEffect, useState} from "react"
import {Selection} from "./Selection";
import {Col, Container, Row} from "reactstrap";

export function LibrarySelector(props) {

    const [selectedServer, setSelectedServer] = useState(null);
    const [libraryData, setLibraryData] = useState({servers: [], libraries: []});

    useEffect(() => {
        const fetchData = async () => {
            const uri = 'api/library?' + new URLSearchParams({
                mediaType: props.mediaType
            });
            const response = await fetch(uri);
            const data = await response.json();
            const uniqueServer = data.map(x => x.server).filter(function (v, i, self) {
                return i === self.findIndex(x => x.id === v.id);
            });
            console.log(`Received ${data.length} libraries`)
            return {
                libraries: data,
                servers: uniqueServer
            }
        }

        fetchData().then(result => {
            setLibraryData(result)
        })
            // make sure to catch any error
            .catch(console.error);
    }, []);

    useEffect(() => {
        const filteredLibraries = libraryData.libraries.filter(lib => lib.serverId === selectedServer?.id)
        if (filteredLibraries?.length > 0)
            props.onLibrarySelected(filteredLibraries[0].id)
    }, [libraryData, selectedServer]);

    if (libraryData.libraries === undefined)
        return null;
    if (selectedServer == null && libraryData.servers.length > 0) {
        setSelectedServer(libraryData.servers[0])
        props.onServerSelected(libraryData.servers[0])
    }

    const filteredLibraries = libraryData.libraries.filter(lib => lib.serverId === selectedServer?.id)
    const serverList = libraryData.servers.map((server) =>
        ({label: server.name, value: server.id}))
    const librariesList = filteredLibraries.map((library) =>
        ({
            label: library.name,
            value: library.id
        })
    )

    return (
        <Container>
            <Row>
                <Col xs="auto">
                    <Selection name="servers"
                               title="Select server"
                               items={serverList}
                               onChange={serverId => {
                                   const server = libraryData.servers.find(x => x.id === serverId)
                                   props.onServerSelected(server)
                                   setSelectedServer(server)
                               }}
                    />
                </Col>
                <Col>
                    <Selection name="libraries"
                               title="select libraries"
                               items={librariesList}
                               onChange={library => props.onLibrarySelected(library)}
                    />
                </Col>
            </Row>
        </Container>
    )
}

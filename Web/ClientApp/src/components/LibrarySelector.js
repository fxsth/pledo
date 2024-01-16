import React, {useEffect, useState} from "react"
import {Dropdown2} from "./Dropdown2";
import {Col, Container, Row} from "reactstrap";

export function LibrarySelector(props) {

    const [selectedServer, setSelectedServer] = useState(null);
    const [selectedLibrary, setSelectedLibrary] = useState(null);
    const [libraryData, setLibraryData] = useState({servers: [], libraries: []});

    useEffect(() => {
        const fetchData = async () => {
            const uri = 'api/library?' + new URLSearchParams({
                mediaType: 'movie'
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

    if (libraryData.libraries === undefined)
        return null;
    if (selectedServer == null && libraryData.servers.length > 0) {
        setSelectedServer(libraryData.servers[0].id)
        props.onServerSelected(libraryData.servers[0].id)
    }
    if (selectedLibrary == null && libraryData.libraries.length > 0) {
        setSelectedLibrary(libraryData.libraries[0].id)
        props.onLibrarySelected(libraryData.libraries[0].id)
    }
    const serverList = libraryData.servers.map((server) =>
        ({label: server.name, value: server.id})
    )
    const librariesList = libraryData.libraries.filter(lib => lib.serverId === selectedServer).map((library) =>
        ({
            label: library.name,
            value: library.id
        })
    )
    return (
        <Container>
            <Row>
                <Col xs="auto">
                    <Dropdown2 name="servers"
                               title="Select server"
                               items={serverList}
                               onChange={server => {
                                   setSelectedServer(server)
                                   props.onServerSelected(server)
                                   props.onLibrarySelected(librariesList[0].id)
                               }}
                    />
                </Col>
                <Col>
                    <Dropdown2 name="libraries"
                               title="select libraries"
                               items={librariesList}
                               onChange={library => props.onLibrarySelected(library)}
                    />
                </Col>
            </Row>
        </Container>
    )
}

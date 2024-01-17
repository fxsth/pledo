import React, {useState} from 'react';
import {
    Button,
    DropdownItem,
    DropdownMenu,
    DropdownToggle, UncontrolledDropdown, UncontrolledTooltip
} from "reactstrap";

export function DownloadButton2({
                                    mediaFile,
                                    mediaKey,
                                    mediaFileKey,
                                    mediaType,
                                    season,
                                    serverId,
                                    knownServers,
                                    downloadBrowserPossible,
                                    color,
                                    children
                                }) {
    const [isLoading, setIsLoading] = useState(false);

    const handleClick = () => {
        setIsLoading(true)
        SendDownloadRequest();

    }

    const getDownloadLink = () => {
        try {
            const server = knownServers.find(x => x.id === serverId)
            const host = server.lastKnownUri
            const token = server.accessToken
            const path = mediaFileKey
            const url = new URL(path, host);
            url.searchParams.append("X-Plex-Token", token)
            return url.toString()
        } catch (e) {
            console.log("An error occured while building download link.")
            return ""
        }
    }

    const SendDownloadRequest = async () => {
        const settings = {
            method: 'POST',
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            }
        };
        let input = 'api/download/' + mediaType + '/' + mediaKey;
        if (typeof season !== 'undefined') {
            input = input + '?' + new URLSearchParams({
                season: season
            })
        }
        if (typeof mediaFileKey !== 'undefined') {
            input = input + '?' + new URLSearchParams({
                mediaFileKey: mediaFileKey
            })
        }
        return fetch(input, settings)
            .then(response => {
                if (response.status >= 200 && response.status < 300) {
                    console.log(response);
                } else {
                    alert('Could not add to the download queue');
                }
            }).catch(err => console.log(err)).finally(x => {
                setIsLoading(false)
            });
    }

    if (downloadBrowserPossible)
        return (
            <span>
                    <UncontrolledDropdown group id={'download_button_' + mediaKey}>
                        <Button color={color} disabled={isLoading}
                                onClick={handleClick}>{children}</Button>
                        <DropdownToggle caret/>
                        <DropdownMenu>
                            <DropdownItem header>
                                To download in browser save link as file:
                            </DropdownItem>
                            <DropdownItem>
                                <a href={getDownloadLink()} download>Direct link</a>
                            </DropdownItem>
                        </DropdownMenu>
                    </UncontrolledDropdown>
                    <UncontrolledTooltip
                        target={'download_button_' + mediaKey}>
                        Per default file will be downloaded server-sided.
                    </UncontrolledTooltip>
                </span>
        );
    else
        return (
            <Button color={color} disabled={isLoading}
                    onClick={handleClick}>{children}</Button>
        );

}


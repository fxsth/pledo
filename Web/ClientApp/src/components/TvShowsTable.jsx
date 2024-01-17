import CollapsibleTableRow from "./CollapsibleTableRow";
import {ButtonGroup, ButtonToolbar, Card, CardBody} from "reactstrap";
import DownloadButton from "./DownloadButton";

export function TvShowsTable({items, selectedServer}) {
    const tvShows = items
    const humanizeByteSize = (size) => {
        if(!size)
            return "--";
        const i = size === 0 ? 0 : Math.floor(Math.log(size) / Math.log(1024));
        return (size / Math.pow(1024, i)).toFixed(2) * 1 + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
    }
    
    return (
        <table className='table table-striped' aria-labelledby="tabelLabel">
            <tbody>
            {tvShows.map(tvShow =>
                <CollapsibleTableRow label={tvShow.title}>
                    <Card>
                        <CardBody>
                            Download season or complete tv show
                            <ButtonToolbar>
                                <ButtonGroup className="me-2">
                                    {
                                        tvShow.episodes.map(item => item.seasonNumber)
                                            .filter((value, index, self) => self.indexOf(value) === index).sort((a,b) => a-b).map(
                                            seasonNumber =>
                                                <DownloadButton mediaType='tvshow' mediaKey={tvShow.ratingKey} season={seasonNumber}>
                                                    Season {seasonNumber}
                                                </DownloadButton>
                                        )

                                    }
                                </ButtonGroup>
                                <ButtonGroup>
                                    <DownloadButton color="info" mediaType='tvshow' mediaKey={tvShow.ratingKey}>
                                        Complete TV Show
                                    </DownloadButton>
                                </ButtonGroup>
                            </ButtonToolbar>
                        </CardBody>
                    </Card>

                    <table className='table table-striped' aria-labelledby="tabelLabel2">
                        <thead>
                        <tr>
                            <th>Season & Episode</th>
                            <th>Episode Title</th>
                            <th>Year</th>
                            <th>Video Codec</th>
                            <th>Resolution</th>
                            <th>Size</th>
                            <th>Download</th>
                        </tr>
                        </thead>
                        <tbody>
                        {tvShow.episodes.map(episode =>
                            <tr>
                                <td>S{episode.seasonNumber}E{episode.episodeNumber}</td>
                                <td>{episode.title}</td>
                                <td>{episode?.year}</td>
                                <td>{episode.mediaFiles[0].videoCodec}</td>
                                <td>{episode.mediaFiles[0].videoResolution}</td>
                                <td>{humanizeByteSize(episode.mediaFiles[0].totalBytes)}</td>
                                <td><DownloadButton mediaType='episode' mediaKey={episode.ratingKey}
                                                    mediaFileKey={episode.mediaFiles[0].downloadUri}
                                                    mediaFile={episode.mediaFiles[0]}
                                                    server={selectedServer}
                                                    downloadBrowserPossible={true}>Download</DownloadButton></td>
                            </tr>
                        )}
                        </tbody>
                    </table>
                </CollapsibleTableRow>
            )}
            </tbody>
        </table>
    );
}
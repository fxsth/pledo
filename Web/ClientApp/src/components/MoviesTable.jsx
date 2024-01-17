import {Table} from "reactstrap";
import DownloadButton from "./DownloadButton";
import {DownloadButton2} from "./DownloadButton2";

export function MoviesTable({items, knownServer}) {
    const movies = items
    return (
        <div>
            <Table striped>
                <thead>
                <tr>
                    <th>Title</th>
                    <th>Year</th>
                    <th>Video Codec</th>
                    <th>Resolution</th>
                    <th>Size</th>
                    <th>Download</th>
                </tr>
                </thead>
                <tbody>
                {movies.map(movie =>
                    movie.mediaFiles.map(mediaFile =>
                        <tr key={mediaFile.downloadUri}>
                            <td>{movie.title}</td>
                            <td>{movie.year}</td>
                            <td>{mediaFile.videoCodec}</td>
                            <td>{mediaFile.videoResolution}</td>
                            <td>{humanizeByteSize(mediaFile.totalBytes)}</td>
                            <td><DownloadButton2
                                mediaType='movie'
                                mediaKey={movie.ratingKey}
                                mediaFileKey={mediaFile.downloadUri}
                                mediaFile={mediaFile}
                                knownServers={knownServer}
                                serverId={movie.serverId}
                                downloadBrowserPossible={true}>Download</DownloadButton2></td>
                        </tr>)
                )}
                </tbody>
            </Table>
            <p>Total: {movies.length} items</p>
        </div>
    );
}

function humanizeByteSize(size) {
    if (!size)
        return "--";
    const i = size === 0 ? 0 : Math.floor(Math.log(size) / Math.log(1024));
    return (size / Math.pow(1024, i)).toFixed(2) * 1 + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
}
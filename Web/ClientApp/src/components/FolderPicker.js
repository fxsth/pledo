import React from "react";

export class FolderPicker extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            directories: [],
            currentDirectory: ""
        };
    }

    componentDidMount() {
        this.populateData();
    }

    goBackInPath = (event) => {
        this.getSubdirectories(this.parentname(this.state.currentDirectory))
    }

    loadSubDirectories = (directory, event) => {
        this.getSubdirectories(directory);
    }

    render() {
        return (
            <div>
                <div>
                    <h3>Directory path: {this.state.currentDirectory}</h3>
                    <button onClick={this.goBackInPath}>Back</button>
                </div>
                {this.state.directories.map((directory) =>
                    <div>
                        <button onClick={(e) => this.loadSubDirectories(directory, e)}>{this.directoryname(directory)}</button>
                    </div>
                )}
                <div>
                    <button onClick={this.props.onInputChange(this.state.currentDirectory)}>Save</button>
                </div>
            </div>
        );
    }

    async populateData() {
        const response = await fetch('api/directory');
        const data = await response.json();
        this.setState({directories: data});
    }

    async getSubdirectories(currentDirectory) {
        const response = await fetch('api/directory?' + new URLSearchParams({
            path: currentDirectory
        }));
        const data = await response.json();
        this.setState({directories: data, currentDirectory: currentDirectory});
    }

    parentname(path) {
        return path.split(/[\\/]/).slice(0,-1).join('/');
    }

    directoryname(path) {
        return path.split(/[\\/]/).slice(-1);
    }
}

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

    saveCurrentDirectory = (directory, event) => {
        if (this.prop.onInputChange)
            this.props.onInputChange(this.state.currentDirectory);
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
                        <label
                            onClick={(e) => this.loadSubDirectories(directory, e)}>{this.directoryname(directory)}</label>
                    </div>
                )}
                <div>
                    <button onClick={() => {
                        if (this.props.onInputChange) {
                            this.props.onInputChange(this.state.currentDirectory);
                        }
                    }}>Save
                    </button>
                </div>
            </div>
        );
    }

    async populateData() {
        const response = await fetch('api/directory');
        const data = await response.json();
        this.setState({directories: data.subDirectories, currentDirectory: data.currentDirectory});
    }

    async getSubdirectories(currentDirectory) {
        const response = await fetch('api/directory?' + new URLSearchParams({
            path: currentDirectory
        }));
        const data = await response.json();
        this.setState({directories: data.subDirectories, currentDirectory: data.currentDirectory});
    }

    parentname(path) {
        return path.split(/[\\/]/).slice(0, -1).join('/');
    }

    directoryname(path) {
        return path.split(/[\\/]/).slice(-1);
    }
}

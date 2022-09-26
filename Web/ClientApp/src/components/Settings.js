import React from "react";
import DownloadButton from "./DownloadButton";

export class Settings extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            settings: []
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    componentDidMount() {
        this.populateData();
    }

    handleChange(event) {
        this.setState({value: event.target.value});
    }

    handleSubmit(event) {
        alert('A name was submitted: ' + this.state.value);
        event.preventDefault();
    }

    render() {
        return (
            <div>
                <form onSubmit={this.handleSubmit}>
                    {this.state.settings.map(setting =>
                        <label>
                            {setting.name}:
                            <input  type="text" value={setting.value} onChange={this.handleChange}/>
                        </label>
                    )}
                    <input type="submit" value="Submit"/>
                </form>
            </div>
        );
    }

    async populateData() {
        const response = await fetch('api/setting');
        const data = await response.json();
        this.setState({settings: data, loading: false});
    }
}
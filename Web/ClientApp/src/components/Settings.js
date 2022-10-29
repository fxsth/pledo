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
        const target = event.target;
        const value = target.value;
        const name = target.name;
        const changedSettings = this.state.settings;
        const index = changedSettings.findIndex(object => {
            return object.key === name;
        });
        console.log("Handle Change name: " + name)
        console.log("Handle Change target: " + target)
        console.log("Handle Change value: " + value)
        console.log("Old value: " + changedSettings[index].value)
        console.log("New value: " + value)
        this.setState(state => {
            state.settings[index].value = value
        });
    }

    handleSubmit(event) {
        event.preventDefault();
        const settings = this.state.settings;
        for (let i = 0; i < settings.length; i++) {
            settings[i].value = event.target[i].value;
        }
        settings.map(setting =>console.log(setting.key + " : " + setting.value));
        this.updateSettings(settings);
    }

    render() {
        return (
            <div>
                <form onSubmit={this.handleSubmit}>
                    {this.state.settings.map(setting =>
                        <label>
                            {setting.name}:
                            <input type="text" defaultValue={setting.value} onChange={this.handleChange}/>
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

    async updateSettings(settings) {
        return fetch('api/setting', {
            method: 'POST',
            body: JSON.stringify(settings),
            headers: {
                'Content-Type': 'application/json'
            }
        }).then(response => {
            if (response.status >= 200 && response.status < 300) {
                return response;
                console.log(response);
                window.location.reload();
            } else {
                console.log('Something happened wrong');
            }
        }).catch(err => err);
    }
}
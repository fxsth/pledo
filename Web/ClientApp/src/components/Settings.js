import React from "react";
import {FolderPicker} from "./FolderPicker";

export class Settings extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            settings: []
        };

        this.handleSubmit = this.handleSubmit.bind(this);
    }

    componentDidMount() {
        this.populateData();
    }

    handleSubmit(event) {
        event.preventDefault();
        const settings = this.state.settings;
        const formData = new FormData(event.target);
        for (let i = 0; i < settings.length; i++) {
            settings[i].value = formData.get(settings[i].key)
        }
        // settings.map(setting => console.log(setting.key + " : " + setting.value));
        this.updateSettings(settings);
    }

    openFolderPicker(event) {
        
    }

    render() {
        return (
            <div style={{width: "50%"}}>
                <form onSubmit={this.handleSubmit} style={{width: "100%"}}>
                    {this.state.settings.map(setting =>
                        <div style={{width: "100%", marginBottom: 20}}>
                            <h6>{setting.name}:</h6>
                            <label style={{width: "100%"}}>
                                <input name={setting.key} style={{width: "100%"}} type="text" defaultValue={setting.value}/>
                            </label>
                            <button onClick={()=>{setting.showFolderPicker = true}}>Select directory</button>
                            <FolderPicker onInputChange={
                                (directory)=>{
                                    console.log("Directory set: " + directory)
                                    const settings = this.state.settings;
                                    for (let i = 0; i < settings.length; i++) {
                                        if(settings[i].key === setting.key)
                                            settings[i].value = directory
                                    }
                                    this.setState({settings: settings, loading: false});
                                    this.updateSettings(settings);
                                }
                            }/>
                        </div>
                    )}
                    <input type="reset" value="Cancel"/>
                    <input style={{float: "right"}} type="submit" value="Save"/>
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
            } else {
                console.log('Something happened wrong');
            }
        }).catch(err => err);
    }
}
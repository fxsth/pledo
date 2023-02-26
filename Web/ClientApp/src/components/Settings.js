import React from "react";
import {FolderPicker} from "./FolderPicker";
import {Modal, ModalBody, ModalHeader, Form, FormGroup, Label, Input, InputGroup, Button} from 'reactstrap';
import FilePathSetting from "./FilePathSetting";
import DropdownSetting from "./DropdownSetting";

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

    updateValueOfSetting(key, value) {
        const settings = this.state.settings;
        for (let i = 0; i < settings.length; i++) {
            if (settings[i].key === key)
                settings[i].value = value
        }
        this.setState({[settings]: settings});
    }

    render() {
        return (
            <div>
                <Form onSubmit={this.handleSubmit}>
                    {this.state.settings.map(setting =>
                        <>
                            {setting.type === "path" &&
                                <FilePathSetting
                                    setting={setting}
                                    callback={(directory) => this.updateValueOfSetting(setting.key, directory)}/>
                            }

                            {setting.type === "enum" &&
                                <DropdownSetting
                                    setting={setting}
                                    callback={(directory) => this.updateValueOfSetting(setting.key, directory)}
                                />
                            }
                        </>
                    )}
                    <Input type="reset" value="Cancel"/>
                    <Input type="submit" value="Save"/>
                </Form>
                <br/>
                <Button onClick={this.handleClick.bind(this)} color="danger">Reset database completely</Button>
            </div>
        );
    }

    handleClick(event) {
        const settings = {
            method: 'DELETE'
        };
        fetch('api/setting', settings)
            .then(response => {
                if (response.status >= 200 && response.status < 300) {
                    console.log("Reset database complete.");
                } else {
                    alert('Could not reset database due to an unknown error.');
                }
            })

    }

    async SendDownloadRequest() {
        const settings = {
            method: 'POST',
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            }
        };

        return fetch('api/download/' + this.props.mediaType + '/' + this.props.mediaKey, settings)
            .then(response => {
                if (response.status >= 200 && response.status < 300) {
                    console.log(response);
                } else {
                    alert('Could not add to the download queue');
                }
            }).catch(err => console.log(err)).finally(x => {
                this.setState({
                    isLoading: false
                });
            });
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
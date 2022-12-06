import React from "react";
import {FolderPicker} from "./FolderPicker";
import {Modal, ModalBody, ModalHeader, Form, FormGroup, Label, Input, InputGroup} from 'reactstrap';

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

    updateShowFolderPickerOfSetting(key, show) {
        console.log(`Folderpicker for setting ${key} set to ${show}`)
        const settings = this.state.settings;
        for (let i = 0; i < settings.length; i++) {
            if (settings[i].key === key)
                settings[i].showFolderPicker = show
        }
        this.setState({settings});
    }

    render() {
        return (
            <Form onSubmit={this.handleSubmit}>
                {this.state.settings.map(setting =>
                    <FormGroup>
                        <Label for={setting.key}>{setting.name}</Label>
                        <InputGroup>
                            <Input id={setting.key} name={setting.key} type="text" value={setting.value}/>
                            <button onClick={() => this.updateShowFolderPickerOfSetting(setting.key, true)}>Select
                                directory
                            </button>
                        </InputGroup>
                        <Modal isOpen={setting.showFolderPicker} onHide={() => setting.showFolderPicker = false}>
                            <ModalHeader>Select directory</ModalHeader>
                            <ModalBody>
                                <FolderPicker onInputChange={
                                    (directory) => {
                                        console.log("Directory set: " + directory)
                                        this.updateValueOfSetting(setting.key, directory)
                                        setting.showFolderPicker = false
                                    }
                                }/>
                            </ModalBody>
                        </Modal>
                    </FormGroup>
                )}
                <Input type="reset" value="Cancel"/>
                <Input type="submit" value="Save"/>
            </Form>
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
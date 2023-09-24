import React, {useState} from "react";
import {FolderPicker} from "./FolderPicker";
import {Modal, ModalBody, ModalHeader, FormGroup, Label, Input, InputGroup, Button} from 'reactstrap';

function FilePathSetting(props) {
    const [showFolderPicker, setShowFolderPicker] = useState(false);
    const [setting, setSetting] = useState(props.setting.value);
    let callback = props.callback;
    return (
        <FormGroup>
            <Label for={props.setting.key}>{props.setting.name}</Label>
            <InputGroup>
                <Input id={props.setting.key} name={props.setting.key} type="text" value={setting}
                       onChange={(e) => setSetting(e.target.value)}/>
                <button type="button" onClick={() => setShowFolderPicker(!showFolderPicker)}>Select
                    directory
                </button>
            </InputGroup>
            <small>{setting.description}</small>
            <Modal isOpen={showFolderPicker}>
                <ModalHeader close={
                    <Button className="close"
                            onClick={() => setShowFolderPicker(!showFolderPicker)}
                            type="button">
                        &times;
                    </Button>
                }>Select directory</ModalHeader>
                <ModalBody>
                    <FolderPicker currentDirectory={setting}
                                  onInputChange={
                                      (directory) => {
                                          console.log("Directory set: " + directory)
                                          callback(directory);
                                          setSetting(directory);
                                          setShowFolderPicker(!showFolderPicker)
                                      }
                                  }/>
                </ModalBody>
            </Modal>
        </FormGroup>
    )
}
export default FilePathSetting;
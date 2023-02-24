import React, {useState} from "react";
import {FolderPicker} from "./FolderPicker";
import {Modal, ModalBody, ModalHeader, FormGroup, Label, Input, InputGroup, Button} from 'reactstrap';

function FilePathSetting(props) {
    const [showFolderPicker, setShowFolderPicker] = useState(false);
    const [setting, setSetting] = useState(props.setting);
    let callback = props.callback;
    return (
        <FormGroup>
            <Label for={setting.key}>{setting.name}</Label>
            <InputGroup>
                <Input id={setting.key} name={setting.key} type="text" value={setting.value}
                       onChange={(e) => {setting.value = e.target.value; setSetting(setting);}}/>
                <button onClick={() => setShowFolderPicker(!showFolderPicker)}>Select
                    directory
                </button>
            </InputGroup>
            <Modal isOpen={showFolderPicker}>
                <ModalHeader close={
                    <Button className="close"
                            onClick={() => setShowFolderPicker(!showFolderPicker)}
                            type="button">
                        &times;
                    </Button>
                }>Select directory</ModalHeader>
                <ModalBody>
                    <FolderPicker currentDirectory={setting.value}
                                  onInputChange={
                                      (directory) => {
                                          console.log("Directory set: " + directory)
                                          callback(directory);
                                          setShowFolderPicker(!showFolderPicker)
                                      }
                                  }/>
                </ModalBody>
            </Modal>
        </FormGroup>
    )
}
export default FilePathSetting;
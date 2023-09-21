import React from "react";
import {FormGroup, Label, Input, InputGroup} from 'reactstrap';

function StringSetting(props) {
    let setting = props.setting;
    let callback = props.callback;
    return (
        <FormGroup>
            <Label for={setting.key}>{setting.name}</Label>
            <InputGroup>
                <Input id={setting.key} name={setting.key} type="text" value={setting.value}
                       onChange={(e) => callback(e.target.value)} />
            </InputGroup>
        </FormGroup>
    )
}

export default StringSetting;
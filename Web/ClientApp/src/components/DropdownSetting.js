import React from "react";
import {FormGroup, Label, Input, InputGroup} from 'reactstrap';

function DropdownSetting(props) {
    let setting = props.setting;
    let callback = props.callback;
    return (
        <FormGroup>
            <Label for={setting.key}>{setting.name}</Label>
            <InputGroup>
                <Input id={setting.key} name={setting.key} type="select" value={setting.value}
                       onChange={(e) => callback(e.target.value)}>
                    {setting.options.map((option) => <option value={option.value} label={option.uiName}/>)}
                </Input>
            </InputGroup>
        </FormGroup>
    )
}

export default DropdownSetting;
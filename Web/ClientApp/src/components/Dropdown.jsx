import * as React from 'react';
import {Component} from "react";

class Dropdown extends Component {
    constructor(props) {
        super(props);
        const {title, list} = this.props;
        this.state = {
            isListOpen: false,
            title,
            selectedItem: null,
            keyword: '',
            list
        };
    }

    render() {
        const listItems = this.state.list.map((entry) =>
            <option value={entry.value}>{entry.label}</option>
        );
        return (
            <div>
                <select>{listItems}</select>
            </div>
        );
    }
};

export default Dropdown;
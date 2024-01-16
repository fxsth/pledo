import * as React from 'react';
import {Component} from "react";

class Dropdown extends Component {
    constructor(props) {
        super(props);
        const {title, list, onChange, onSelection} = this.props;
        this.state = {
            isListOpen: false,
            title,
            keyword: '',
            list,
            onChange,
            onSelection
        };
    }

    render() {
        const listItems = this.state.list.map((entry) =>
            <option value={entry.value}>{entry.label}</option>
        );
        if (this.state.list.length === 0)
            listItems.unshift(
                <option>{this.state.title}</option>
            )
        return (
            <div>
                <select onChange={event => this.state.onSelection(event.target.value)}>
                    {listItems}
                </select>
            </div>
        );
    }
};

export default Dropdown;
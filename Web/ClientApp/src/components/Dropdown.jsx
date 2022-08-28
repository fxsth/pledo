import * as React from 'react';
import {Component} from "react";

class Dropdown extends Component {
    constructor(props) {
        super(props);
        const {title, list, onChange} = this.props;
        this.state = {
            isListOpen: false,
            title,
            selectedItem: null,
            keyword: '',
            list,
            onChange
        };
    }

    render() {
        const listItems = this.state.list.map((entry) =>
            <option value={entry.value}>{entry.label}</option>
        );
        listItems.unshift(
            <option>{this.state.title}</option>
        )
        return (
            <div>
                <select onChange={this.state.onChange}>
                    {listItems}
                </select>
            </div>
        );
    }
};

export default Dropdown;
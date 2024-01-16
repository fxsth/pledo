import React, {useState} from "react"

export function Selection(props) {
    
    const listItems = props.items.map((entry) =>
        <option value={entry.value}>{entry.label}</option>
    );
    if (listItems.length === 0)
        listItems.unshift(
            <option>{props.title}</option>
        )

    return (
        <div>
            <select onChange={event => props.onChange(event.target.value)}>
                {listItems}
            </select>
        </div>)
}
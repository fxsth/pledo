import React, {useState} from 'react';

const CollapsibleTableRow = (props) => {
        const [open, setOpen] = useState(false);
        const toggle = () => {
            setOpen(!open);
        };
        return (
            <tr >
                <div>
                    <div onClick={toggle}>{props.label}</div>
                    {open &&
                        <div className="toggle">
                            {props.children}
                        </div>
                    }
                </div>
            </tr>
        )
    }
;
export default CollapsibleTableRow;
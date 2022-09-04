import React, {useState} from 'react';

const CollapsibleTableRow = (props) => {
        const [open, setOPen] = useState(false);
        const toggle = () => {
            setOPen(!open);
        };
        return (
            <tr onClick={toggle}>
                <div>{props.label}
                {open &&
                    <div className="toggle">{props.children}</div>
                }
                </div>
            </tr>
        )
    }
;
export default CollapsibleTableRow;
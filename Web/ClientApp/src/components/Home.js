import React, {Component} from 'react';
import {Overview} from "./Overview";
import {Settings} from "./Settings";
import styles from './home.module.css';

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = {};
    }

    render() {

        return (
            <div><Overview/></div>
        );

    }
}

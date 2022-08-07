import React, { Component } from 'react';
import AccountForm from './AccountForm';

export class Settings extends Component {
  static displayName = Settings.name;

  constructor(props) {
    super(props);
    this.state = { accounts: [], loading: true };
  }

  componentDidMount() {
    this.populateSettingsData();
  }

  static renderAccountTable(accounts) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Username</th>
            <th>Password</th>
            <th>AuthKey</th>
          </tr>
        </thead>
        <tbody>
          {accounts.map(account =>
            <tr key={account.username}>
              <td>{account.username}</td>
              <td>{account.password}</td>
              <td>{account.authKey}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Settings.renderAccountTable(this.state.accounts);

    return (
      <div>
        <h1 id="tabelLabel" >Plex Accounts</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
        <AccountForm/>
      </div>
    );
  }

  async populateSettingsData() {
    const response = await fetch('api/accounts');
    const data = await response.json();
    this.setState({ accounts: data, loading: false });
  }
}

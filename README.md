
# Timer

This is a small application used to quickly log time against Teamwork tasks and projects.

## Configuration

The application configuration data is stored in the appsettings.json file.
During application bootup, an example config file is copied over to this folder, as a reference for any new configuration options introduced due to an update.

### The Teamwork configuration section

  "Teamwork": {
    "ApiKey": "",
    "AuthType": "Basic",
    "TeamworkEndPointUrlBase": "",
    "DaysToConsiderRecent":  14
  }

As a bare minimumm, you will need to set the ApiKey and TeamworkEndPointUrlBase. You can get these values by following the steps here: 
https://apidocs.teamwork.com/docs/teamwork/d1b2de52c3cec-api-key-and-url

Leave AuthType set to Basic.

When logging time, the Timer app will display a list of recent tags, tasks and projects from the last 14 days by default. Alter the DaysToConsiderRecent configuration option to override this.

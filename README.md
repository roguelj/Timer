
# Timer

*Timer* is a small application used to quickly log time against Teamwork (https://www.teamwork.com) tasks and projects.

## Getting Started
Download the latest release and run the installer. You will require the .NET 7 runtime installed. You can get that from https://dotnet.microsoft.com/en-us/download.
Run the installed application, and a small window will appear with a single 'Log Time' button. Right-click the button, and click 'Settings'. This will open the program settings folder. You will need to enter some data into the *appsettings.json* file to be able to log time.

## Configuration

The application configuration data is stored in the *appsettings.json* file.
During application bootup, an example config file is copied over to this folder, as a reference for any new configuration options introduced due to an update.

### The Teamwork configuration section

  "Teamwork": {
    "ApiKey": "",
    "AuthType": "Basic",
    "TeamworkEndPointUrlBase": "",
    "DaysToConsiderRecent":  14
  }

As a bare minimumm, you will need to set the **ApiKey** and **TeamworkEndPointUrlBase**. You can get these values by following the steps here: 
https://apidocs.teamwork.com/docs/teamwork/d1b2de52c3cec-api-key-and-url

Leave **AuthType** set to Basic.

When logging time, the Timer app will display a list of recent tags, tasks and projects from the last 14 days by default. Alter the **DaysToConsiderRecent** configuration option to override this.

## Usage
When the application has been installed and configured, you will be able to log time. Click the 'Log Time' button to proceed. A window will appear allowing you to set the start date/time (by default, this will be set to the end time of the last time entry) and the end date/time (by default, this will be set to the current date & time). Combo boxes allow you to set the Project and Task to log time against, and a list of multi-selectable tags is available also. Click Ok to log the time, or cancel to discard.


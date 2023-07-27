using Prism.Services.Dialogs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Timer.Shared.Extensions;
using Timer.Shared.Models;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;
using Timer.Shared.Services.Interfaces;
using Timer.Shared.ViewModels;
using Res = Timer.Shared.Resources.Resources;

namespace Timer.WPF.ViewModels
{

    public class TimeLogViewModel:Base
    {


        // injected services
        private IDialogService DialogService { get; }


        // commands
        public Prism.Commands.DelegateCommand LogTimeCommand { get; private set; }
        public Prism.Commands.DelegateCommand OpenSettingsCommand { get; private set; }
        public Prism.Commands.DelegateCommand OpenAboutCommand { get; private set; }


        // constructor
        public TimeLogViewModel(ILogger logger, ITimeLogService timeLogService, IDialogService dialogService) :base(logger, timeLogService)
        {

            // initialise services
            this.DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));   


            // setup commands
            this.LogTimeCommand = new Prism.Commands.DelegateCommand(this.LogTimeAsync, this.CanLogTime);
            this.OpenSettingsCommand = new Prism.Commands.DelegateCommand(this.OpenSettings);
            this.OpenAboutCommand = new Prism.Commands.DelegateCommand(this.OpenAbout);


            // add commands to the base collection
            this.Commands.Add(this.LogTimeCommand);
            this.Commands.Add(this.OpenSettingsCommand);
            this.Commands.Add(this.OpenAboutCommand);

        }


        private bool CanLogTime()
        {
            return true; // TODO: implement this. needs to determine successful connection to teamwork
        }

        private async void LogTimeAsync()
        {

            // determine the most sensible start & end dates.
            // get the start date as the time stamp of the last entry end point.
            // the assumption is that the Time Log Entry button is pressed on task finish, so the end date is now
            var startDate = await this.TimeLogService!.GetEndTimeOfLastTimeLogEntryAsync(CancellationToken.None);
            var endDate = DateTime.Now;


            // determine the most recent tags, tasks and projects
            var recentProjects = await this.TimeLogService.RecentProjects(CancellationToken.None);
            var recentTasks = await this.TimeLogService.RecentTasks(CancellationToken.None);
            var recentTags = await this.TimeLogService.RecentTags(CancellationToken.None);


            // get all tags, tasks and projects
            var projects = await this.TimeLogService.Projects(CancellationToken.None);
            var tasks = await this.TimeLogService.Tasks(CancellationToken.None);
            var tags = await this.TimeLogService.Tags(CancellationToken.None);


            // create the parameters to pass along to the dialog
            var parameters = new DialogParameters
            {
                { StartTimeDialogParameterName, startDate.Value.DateTime }, 
                { EndTimeDialogParameterName, endDate},
                { RecentTagsDialogParameterName, recentTags.Select(s => s.ToTag()).ToList() },
                { RecentTasksDialogParameterName, recentTasks },
                { RecentProjectsDialogParameterName, recentProjects },
                { ProjectsDialogParameterName, projects },
                { TasksDialogParameterName, tasks },
                { TagsDialogParameterName, tags.Select(s => s.ToTag()).ToList() }
            };


            // open a new dialog with the details
            this.DialogService.ShowDialog(TimeLogDialogName, parameters, LogTimeAsync);

        }

        private async void LogTimeAsync(IDialogResult dialogResult) 
        {

            if (dialogResult.Result == ButtonResult.OK)
            {

                // get the data from the dialog
                var startDateTime = dialogResult.Parameters.GetValue<DateTime>(StartTimeDialogParameterName);
                var endDateTime = dialogResult.Parameters.GetValue<DateTime>(EndTimeDialogParameterName);
                var tags = dialogResult.Parameters.GetValue<List<KeyedEntity>>(SelectedTagsDialogParameterName);
                var task = dialogResult.Parameters.GetValue<KeyedEntity>(SelectedTaskDialogParameterName);
                var project = dialogResult.Parameters.GetValue<KeyedEntity>(SelectedProjectDialogParameterName);
                var isBillable = dialogResult.Parameters.GetValue<bool>(IsBillableDialogParameterName);
                var description = dialogResult.Parameters.GetValue<string>(DescriptionDialogParameterName);


                await this.TimeLogService!.LogTime(startDateTime, endDateTime, project.Id, task?.Id, tags?.Select(s => s.Id).ToList(),isBillable, description, CancellationToken.None);

            }

        }

        private void OpenSettings()
        {

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {

                var psi = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = Shared.Application.ConfigurationServices.UserDataPath,
                    WorkingDirectory = Shared.Application.ConfigurationServices.UserDataPath
                };

                Process.Start(psi);

            }

        }

        private void OpenAbout()
        {

            var viewAssembly = Assembly.GetExecutingAssembly();
            var sharedAssembly = typeof(Base).Assembly;
                       
            var viewFileVersionInfo = FileVersionInfo.GetVersionInfo(viewAssembly.Location);
            var sharedFileVersionInfo = FileVersionInfo.GetVersionInfo(sharedAssembly.Location);

            var parameters = new DialogParameters
            {
                { AboutBoxTitleParameterName, Res.AboutDialogTitle },
                { AboutBoxTextParameterName, Res.AboutDialogText },
                { AboutBoxSharedVersionParameterName, sharedFileVersionInfo.ProductVersion },
                { AboutBoxViewVersionParameterName, viewFileVersionInfo.ProductVersion }
            };

            this.DialogService.ShowDialog(AboutBoxDialogName, parameters, LogTimeAsync);
        }

    }

}
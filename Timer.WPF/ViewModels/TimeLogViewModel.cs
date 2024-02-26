using Prism.Services.Dialogs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
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
        public Prism.Commands.DelegateCommand LogTimeCommand { get; }
        public Prism.Commands.DelegateCommand OpenSettingsCommand { get; }
        public Prism.Commands.DelegateCommand OpenAboutCommand { get; }


        // constructor
        public TimeLogViewModel(ILogger logger, ITimeLogService timeLogService, IDialogService dialogService) :base(logger, timeLogService)
        {

            // initialise services
            this.DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));   


            // setup commands
            this.LogTimeCommand = new Prism.Commands.DelegateCommand(this.LogTime, this.CanLogTime);
            this.OpenSettingsCommand = new Prism.Commands.DelegateCommand(this.OpenSettings);
            this.OpenAboutCommand = new Prism.Commands.DelegateCommand(this.OpenAbout);


            // add commands to the base collection
            this.AddCommand(this.LogTimeCommand);
            this.AddCommand(this.OpenSettingsCommand);
            this.AddCommand(this.OpenAboutCommand);

        }


        private bool CanLogTime()
        {
            return true; // TODO: implement this. needs to determine successful connection to teamwork
        }

        private void LogTime() => this.DialogService.ShowDialog(TimeLogDialogName, this.LogTimeAsync);

        private async void LogTimeAsync(IDialogResult dialogResult)
        {

            if (dialogResult.Result == ButtonResult.OK)
            {

                // get the data from the dialog
                var startDateTime = dialogResult.Parameters.GetValue<DateTime>(StartTimeDialogParameterName);
                var endDateTime = dialogResult.Parameters.GetValue<DateTime>(EndTimeDialogParameterName);
                var tags = dialogResult.Parameters.GetValue<List<Tag>>(SelectedTagsDialogParameterName);
                var task = dialogResult.Parameters.GetValue<ProjectTask>(SelectedTaskDialogParameterName);
                var project = dialogResult.Parameters.GetValue<Project>(SelectedProjectDialogParameterName);
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
            var viewFileVersionInfo = viewAssembly.GetName().Version;
            var sharedFileVersionInfo = sharedAssembly.GetName().Version;

            var parameters = new DialogParameters
            {
                { AboutBoxTitleParameterName, Res.AboutDialogTitle },
                { AboutBoxTextParameterName, Res.AboutDialogText },
                { AboutBoxSharedVersionParameterName, sharedFileVersionInfo!.ToString() },
                { AboutBoxViewVersionParameterName, viewFileVersionInfo!.ToString() }
            };

            this.DialogService.ShowDialog(AboutBoxDialogName, parameters, this.LogTimeAsync);
        }

    }

}
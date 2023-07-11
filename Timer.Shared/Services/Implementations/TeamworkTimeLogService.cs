using Timer.Shared.Models;
using Timer.Shared.Services.Interfaces;

namespace Timer.Shared.Services.Implementations
{
    internal partial class TeamworkTimeLogService : ITimeLogService
    {
        async Task<DateTimeOffset> ITimeLogService.GetEndTimeOfLastTimeLogEntryAsync(CancellationToken cancellationToken)
        {
            var at = await this.AccessToken();
            var me = await this.Me( cancellationToken); 
            return (await this.MyLastTimeEntry(me.Id, cancellationToken)).TimeLogged;
        }

        async Task<List<KeyedEntity>> ITimeLogService.GetRecentProjectsAsync()
        {
            return await Task.Run(DummyKeyedEntityProjectsDevOnly);
        }

        async Task<List<KeyedEntity>> ITimeLogService.GetRecentTagsAsync()
        {
            return await Task.Run(Dummytags);
        }

        async Task<List<KeyedEntity>> ITimeLogService.GetRecentTasksAsync()
        {
            return await Task.Run(DummyTasks);
        }

        private List<KeyedEntity> DummyKeyedEntityProjectsDevOnly()
        {

            var projectIdeas = new List<string>
            {
                "A simple calculator",
                "A number guessing game",
                "A to-do list app",
                "A note-taking app",
                "A simple text editor",
                "A game of tic-tac-toe",
                "A music player",
                "A drawing app",
                "A digital clock",
                "A simple web browser",
                "A simple chat app",
                "A library management system",
                "A hospital management system",
                "A inventory system",
                "A mini search engine",
                "A resume builder software",
                "A GUI for databases",
                "A build your own Linux commands/DOS commands app",
                "A mini Facebook/Twitter clone",
                "An online banking system",
                "An online ticket booking system",
                "A music organizer",
                "A price comparison website",
                "An Amazon/Flipkart clone",
                "A YouTube clone",
                "A matrimonial website",
                "A WYSIWYG HTML editor",
                "A web scraper",
                "A simple chat program",
                "A quiz website",
                "A Stack Overflow clone",
                "An online voting system",
                "An expense tracker app/website",
                "A chatbot"
            };

            var list = projectIdeas.ToList().Select(s => new KeyedEntity(projectIdeas.IndexOf(s), s)).ToList();
            return list;
        }

        private List<KeyedEntity> DummyTasks()
        {
            List<string> tasks = new List<string>
            {
                "Write a simple function to calculate the factorial of a number",
                "Create a class to represent a book and its properties",
                "Write a program to read a file and print its contents",
                "Create a simple GUI application using Windows Forms",
                "Write a program to solve a simple math problem",
                "Create a class to represent a user and its properties",
                "Write a program to read and write data from a database",
                "Create a simple web application using ASP.NET",
                "Write a program to send an email",
                "Create a class to represent a product and its properties",
                "Write a program to read and write data from a file in JSON format",
                "Create a simple RESTful API",
                "Write a program to use a third-party API",
                "Create a class to represent a customer and its properties",
                "Write a program to create and manage a Windows Service",
                "Create a simple mobile app using Xamarin",
                "Write a program to use the Windows Speech API",
                "Create a class to represent a bank account and its properties",
                "Write a program to manage a shopping cart",
                "Create a simple game using the Unity Engine",
                "Write a program to use the OpenCV library",
                "Create a class to represent a product order and its properties",
                "Write a program to manage a social media application",
                "Create a simple machine learning model",
                "Write a program to use the Google Cloud Platform",
                "Create a class to represent a user profile and its properties",
                "Write a program to manage a content management system",
                "Create a simple blockchain application",
                "Write a program to use the Amazon Web Services"
            };

            var list = tasks.ToList().Select(s => new KeyedEntity(tasks.IndexOf(s), s)).ToList();
            return list;
        }
    
        private List<KeyedEntity> Dummytags()
        { 
            List<string> tags = new List<string>
            {
                "CODE",
                "BUG",
                "FEATURE",
                "FLAG",

            };

            var list = tags.ToList().Select(s => new KeyedEntity(tags.IndexOf(s), s)).ToList();
            return list;
        }
    }

}

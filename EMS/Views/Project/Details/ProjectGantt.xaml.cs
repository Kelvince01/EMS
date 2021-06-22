using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EMS.Views.Charts;
using Syncfusion.UI.Xaml.Gantt;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace EMS.Views.Project.Details
{
    public sealed partial class ProjectGantt : UserControl
    {
        public ProjectGantt()
        {
            this.InitializeComponent();
        }
    }

    /// <summary>
    /// The project tracker view model class.
    /// </summary>
    public class ProjectTrackerViewModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Charts.ProjectTrackerViewModel"/> class.
        /// </summary>
        public ProjectTrackerViewModel()
        {
            this.TaskCollection = this.GetData();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the appointment item source.
        /// </summary>
        /// <value>The appointment item source.</value>
        public ObservableCollection<TaskDetail> TaskCollection { get; set; }

        /// <summary>
        /// Gets or sets the gantt resources.
        /// </summary>
        /// <value>The gantt resources.</value>
        public GanttResourceCollection ResourceCollection { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The data.</returns>
        public ObservableCollection<TaskDetail> GetData()
        {
            ObservableCollection<TaskDetail> schedule = new ObservableCollection<TaskDetail>
                                                            {
                                                                new TaskDetail
                                                                    {
                                                                        StartDate = new DateTime(2014, 2, 3),
                                                                        FinishDate = new DateTime(2014, 3, 6),
                                                                        Name = "Project Schedule",
                                                                        ID = "1"
                                                                    }
                                                            };

            ObservableCollection<TaskDetail> scheduleProcess = new ObservableCollection<TaskDetail>
                                                                   {
                                                                       new TaskDetail
                                                                           {
                                                                               StartDate = new DateTime(2014, 3, 20),
                                                                               FinishDate = new DateTime(2014, 3, 27),
                                                                               Name = "Planning",
                                                                               ID = "2"
                                                                           },
                                                                       new TaskDetail
                                                                           {
                                                                               StartDate = new DateTime(2014, 3, 20),
                                                                               FinishDate = new DateTime(2014, 4, 4),
                                                                               Name = "Design",
                                                                               ID = "7"
                                                                           },
                                                                       new TaskDetail
                                                                           {
                                                                               StartDate = new DateTime(2014, 3, 9),
                                                                               FinishDate = new DateTime(2014, 4, 8),
                                                                               Name = "Implementation Phase",
                                                                               ID = "12"
                                                                           },
                                                                       new TaskDetail
                                                                           {
                                                                               StartDate = new DateTime(2014, 3, 27),
                                                                               FinishDate = new DateTime(2014, 4, 28),
                                                                               Name = "Integration",
                                                                               ID = "37"
                                                                           },
                                                                       new TaskDetail
                                                                           {
                                                                               StartDate = new DateTime(2014, 3, 25),
                                                                               FinishDate = new DateTime(2014, 4, 26),
                                                                               Name = "Final Testing",
                                                                               ID = "38"
                                                                           },
                                                                       new TaskDetail
                                                                           {
                                                                               StartDate = new DateTime(2014, 3, 26),
                                                                               FinishDate = new DateTime(2014, 3, 26),
                                                                               Name = "Final Delivery",
                                                                               ID = "39"
                                                                           }
                                                                   };
            schedule[0].Children = scheduleProcess;

            ObservableCollection<TaskDetail> planning = new ObservableCollection<TaskDetail>
                                                            {
                                                                new TaskDetail
                                                                    {
                                                                        StartDate = new DateTime(2014, 3, 27),
                                                                        FinishDate = new DateTime(2014, 4, 2),
                                                                        Name = "Plan timeline",
                                                                        ID = "3",
                                                                        Progress = 100
                                                                    },
                                                                new TaskDetail
                                                                    {
                                                                        StartDate = new DateTime(2014, 3, 28),
                                                                        FinishDate = new DateTime(2014, 4, 5),
                                                                        Name = "Plan budget",
                                                                        ID = "4",
                                                                        Progress = 100
                                                                    },
                                                                new TaskDetail
                                                                    {
                                                                        StartDate = new DateTime(2014, 3, 27),
                                                                        FinishDate = new DateTime(2014, 4, 3),
                                                                        Name = "Allocate resources",
                                                                        ID = "5",
                                                                        Progress = 100
                                                                    },
                                                                new TaskDetail
                                                                    {
                                                                        StartDate = new DateTime(2014, 3, 27),
                                                                        FinishDate = new DateTime(2014, 3, 27),
                                                                        Name = "Planning complete",
                                                                        ID = "6",
                                                                        Progress = 100
                                                                    }
                                                            };
            scheduleProcess[0].Children = planning;
            this.ResourceCollection = this.GetResources();

            // To define resource for a task.
            scheduleProcess[0].Resources.Add("0");
            scheduleProcess[1].Resources.Add("1");
            scheduleProcess[2].Resources.Add("2");
            scheduleProcess[3].Resources.Add("3");
            scheduleProcess[4].Resources.Add("4");
            scheduleProcess[5].Resources.Add("5");
            return schedule;
        }

        /// <summary>
        /// Gets the resources for the project.
        /// </summary>
        /// <returns>The resources.</returns>
        private GanttResourceCollection GetResources()
        {
            GanttResourceCollection resources = new GanttResourceCollection
                                                    {
                                                        new GanttResource { ID = "1", Name = "Planning" },
                                                        new GanttResource { ID = "2", Name = "Design" },
                                                        new GanttResource { ID = "3", Name = "Implementation Phase" },
                                                        new GanttResource { ID = "4", Name = "Integration" },
                                                        new GanttResource { ID = "5", Name = "Final Testing" }
                                                    };

            return resources;
        }

        #endregion
    }
}

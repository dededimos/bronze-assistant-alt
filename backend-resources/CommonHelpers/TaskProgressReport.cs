using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers
{
    /// <summary>
    /// An Object Representing the Progress of a Task (35% , 100items loaded of/1000 items e.t.c.)
    /// </summary>
    public class TaskProgressReport
    {
        /// <summary>
        /// The Percentage completed of the Task
        /// </summary>
        public double PercentCompleted { get => StepsCompleted * 100 / TotalSteps; }
        /// <summary>
        /// The Steps of the Task Completed
        /// </summary>
        public double StepsCompleted { get; set; } = 0;
        /// <summary>
        /// The Total Steps of the Task
        /// </summary>
        public double TotalSteps { get; set; } = 1;
        /// <summary>
        /// The Remaining Steps of the Task
        /// </summary>
        public double RemainingSteps { get => TotalSteps - StepsCompleted; }
        /// <summary>
        /// The Current Steps Description
        /// </summary>
        public string CurrentStepDescription { get; set; } = string.Empty;

        public TaskProgressReport(double totalSteps , double stepsCompleted , string currentStepDescription)
        {
            TotalSteps = totalSteps;
            StepsCompleted = stepsCompleted;
            CurrentStepDescription = currentStepDescription;
        }
    }
}

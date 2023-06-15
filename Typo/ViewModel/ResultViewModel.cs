using System;
using System.Collections.Generic;
using Typo.Model.Exercise;
using Typo.Model.Milestone;
using Typo.Utils;
using Typo.View;

namespace Typo.ViewModel
{
    public abstract class ResultViewModel
    {
        public static List<MilestoneModel>? CompletedMilestones { get; set; }
        public ExerciseResultModel Result { get; set; }
        public int Correct { get { return Result.AmountCorrect; } }
        public int Incorrect { get { return Result.AmountWrong; } }
        public string TimeSpent { get { return Result.TimeToString(); } }
        public int Score { get { return Result.Score; } }
        public string PercentageCorrect { get { return Result.GetPercentage() + "%"; } }

        public abstract Command AgainCommand { get; set; }
        public Command BackCommand { get; set; }

        public ResultViewModel(ExerciseResultModel? result)
        {
            if (result == null) 
                throw new ArgumentNullException("Result is null");
            Result = result;
            BackCommand = new Command(BackToMenu);
            CompletedMilestones = MilestoneUtils.UpdateMilestones(Result);
        }

        /// <summary>
        /// Navigates the application to the student home view. Displays the dialog for completed milestones if necessary.
        /// </summary>
        public void BackToMenu()
        {
            if (CompletedMilestones?.Count > 0)
            {
                App.CurrentDialog = new MilestoneUnlockedView();
            }
            App.CurrentWindow = new StudentHomeView();
        }
    }
}

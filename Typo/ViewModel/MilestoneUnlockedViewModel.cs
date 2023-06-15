using System;
using System.Collections.Generic;
using Typo.Model.Milestone;

namespace Typo.ViewModel
{
    public class MilestoneUnlockedViewModel
    {
        public List<MilestoneModel> CompletedMilestonesList { get; set; }
        public Command CloseCommand { get; set; }
        public MilestoneUnlockedViewModel()
        {
            if (ResultViewModel.CompletedMilestones == null)
            {
                throw new NullReferenceException("Error: No completed milestones found");
            }
            CompletedMilestonesList = ResultViewModel.CompletedMilestones;
            CloseCommand = new Command(Close);
        }

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        public void Close()
        {
            ResultViewModel.CompletedMilestones?.Clear();
            App.CurrentDialog = null;
        }
    }
}

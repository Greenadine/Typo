using System;
using System.Collections.Generic;
using Typo.Model.Milestone;
using Typo.Utils;
using Typo.View.Sidebar;

namespace Typo.ViewModel
{
    public class StudentMilestoneViewModel
    {
        // Sidebar
        public StudentSidebarView Sidebar { get; set; }

        public List<MilestoneModel> MilestoneList { get; set; }

        public StudentMilestoneViewModel()
        {
            Sidebar = new();
            MilestoneList = GetMilestones();
        }

        /// <summary>
        /// Gets all the milestones of the student from the database.
        /// </summary>
        /// <returns>A List containing the milestones.</returns>
        public List<MilestoneModel> GetMilestones()
        {
            List<MilestoneModel> list = new();
            foreach (Dictionary<string, object> resultData in MilestoneUtils.GetMilestones(UserUtils.Username))
            {
                list.Add(ConstructorViewModel.ConstructMilestone(resultData));
            }
            return list;
        }
    }
}

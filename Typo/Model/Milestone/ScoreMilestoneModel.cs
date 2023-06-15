using System;

namespace Typo.Model.Milestone
{
    public class ScoreMilestoneModel : MilestoneModel
    {
        public int ScoreThreshold { get; set; }

        public ScoreMilestoneModel(int id, string name, string description, MilestoneType type, string imagePath, int progression, int threshold, int scoreThreshold, DateTime? completionDate)
            : base(id, name, description, type, imagePath, progression, threshold, completionDate)
        {
            ScoreThreshold = scoreThreshold;
        }
    }
}

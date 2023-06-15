using System;
using System.Windows.Media;
using Brush = System.Windows.Media.SolidColorBrush;

namespace Typo.Model.Milestone
{
    public class MilestoneModel
    {

        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public Brush ProgressbarColor { get { return GetProgressbarColor(); } }
        public MilestoneType Type { get; }
        public string ImagePath { get; }
        public int Progression
        {
            get => _progression;
            set
            {
                if (value >= Threshold)
                {
                    value = Threshold;
                    CompletionDate = DateTime.Now;
                }
                _progression = value;
            }
        }
        private int _progression;
        public int Threshold { get; }
        public string RequirementString { get { return GetRequirementString(); } }
        public string CompletionDateString
        {
            get
            {
                if (CompletionDate != null)
                {
                    return CompletionDate.Value.ToString("d");
                }
                return string.Empty;
            }
        }

        public DateTime? CompletionDate { get; set; }

        public MilestoneModel(int id, string name, string description, MilestoneType type, string imagePath, int progression, int threshold, DateTime? completionDate)
        {
            Id = id;
            Name = name;
            Description = description;
            Type = type;
            Threshold = threshold;
            ImagePath = imagePath;
            Progression = progression;
            if (completionDate != null)
            {
                CompletionDate = completionDate.Value.Date;
            }
        }

        /// <summary>
        /// Function that returns the string that is displayed on the progressbar on the milestone page.
        /// </summary>
        /// <returns></returns>
        public string GetRequirementString()
        {
            if (CompletionDate == null)
            {
                return Progression + "/" + Threshold;
            }
            return CompletionDateString;
        }

        /// <summary>
        /// Function that returns the color of the progressbar for the milestones page. Is based of the ratio of completion
        /// </summary>
        /// <returns>A SolidColorBrush with the corrisponding color</returns>
        private Brush GetProgressbarColor()
        {
            if (Threshold == 0)
            {
                return new(Colors.Red);
            }
            double ratio = (double)(Progression / Threshold);
            if (ratio < 0.41)
            {
                return new(Colors.Red);
            }
            else if (ratio < 0.71)
            {
                return new(Colors.Orange);
            }
            else if (ratio < 1)
            {
                return new(Colors.Green);
            }
            return new(Colors.Gold);
        }
    }
}

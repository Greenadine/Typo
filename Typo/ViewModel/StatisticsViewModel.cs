using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Typo.Model;
using Typo.Model.Exercise;
using Typo.Utils;
using Typo.View.Sidebar;

namespace Typo.ViewModel
{
    public class StatisticsViewModel : PropertyChangable, IViewModel, IHelp
    {
        public StudentSidebarView Sidebar { get; set; }

        public List<ExerciseResultModel> FilteredResults { get; set; } = new List<ExerciseResultModel>();

        public List<int> GraphComboBoxItemSource { get; set; } = new List<int>() { 1, 2, 5, 7, 14, 31, 365 };

        #region Binding properties
        private int _selectedGraphAmount;
        public int SelectedGraphAmount
        {
            get => _selectedGraphAmount;
            set
            {
                _selectedGraphAmount = value;
                UpdateScreen();
                OnPropertyChanged(nameof(SelectedGraphAmount));
            }
        }

        private GraphModel _graphModel;
        public GraphModel GraphModel
        {
            get => _graphModel;
            set
            {
                _graphModel = value;
                OnPropertyChanged(nameof(GraphModel));
            }
        }

        public List<ExerciseResultModel> Results { get; set; }
        private ExerciseResultModel _selected;
        public ExerciseResultModel Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged(string.Empty);
            }
        }

        public string Name { get { return Selected.ExerciseName; } }
        public int HighestScore { get; set; }

        public int AttemptNR { get { return Selected.Attempt; } }
        public int Score { get { return Selected.Score; } }
        public string Time { get { return Selected.TimeToString(); } }
        public string PercentageCorrect { get { return Selected.GetPercentage() + "%"; } }
        private double _averageScore;
        public double AverageScore
        {
            get => _averageScore;
            set
            {
                _averageScore = value;
                OnPropertyChanged(nameof(AverageScore));
            }
        }
        private string _averageCorrect = string.Empty;
        public string AverageCorrect
        {
            get => _averageCorrect;
            set
            {
                _averageCorrect = value;
                OnPropertyChanged(nameof(AverageCorrect));
            }
        }
        #endregion

        public StatisticsViewModel()
        {
            Sidebar = new();

            // Default values for the labels of the detail table labels
            _selected = new(UserUtils.Username, 0, "Naam", 0, 0, 0, 0, 1, 0, DateTime.Now);
            _graphModel = new(new());
            //Gets the exercises from the DB
            Results = CreateExerciseList();

            UpdateScreen();
        }

        /// <summary>
        /// Function to construct a list of all exercises made by the logged in student
        /// </summary>
        /// <returns>A list<ExerciseResultModel> containing the data of all made exercises</returns>
        private List<ExerciseResultModel> CreateExerciseList()
        {
            List<ExerciseResultModel> list = new();
            foreach (Dictionary<string, object> resultData in StatisticsUtils.GetStatisticsListData(UserUtils.Username))
            {
                list.Add(ConstructorViewModel.ConstructResultModel(UserUtils.Username, resultData));
            }
            return list;
        }

        /// <summary>
        /// Updates the graph, average correct and the average score displayed on the screen.
        /// </summary>
        private void UpdateScreen()
        {
            UpdateGraph();
            AverageCorrect = CalculateAverageCorrect();
            AverageScore = CalculateAverageScore();
        }

        /// <summary>
        /// Updates the graphData
        /// </summary>
        private void UpdateGraph()
        {
            FilteredResults = Results
                .Where(x => x.Date > DateTime.Today.AddDays(-SelectedGraphAmount))
                .OrderByDescending(x => x.Date)
                .Reverse()
                .ToList();

            GraphModel = new(SetScoreAsGraphPoints(FilteredResults));
            OnPropertyChanged(nameof(FilteredResults));
        }

        /// <summary>
        /// Converts a list of results into a list of points using the Score for Y and an increment of 50 pixels for X
        /// </summary>
        /// <param name="graphData"></param>
        /// <returns>returns a list of points created using the score of the results in graphData</returns>
        private PointCollection SetScoreAsGraphPoints(List<ExerciseResultModel> graphData)
        {
            HighestScore = 0;
            PointCollection result = new();
            for (int i = 0; i < graphData.Count; i++)
            {
                if (i < graphData.Count)
                {
                    if (graphData[i].Score > HighestScore)
                    {
                        HighestScore = graphData[i].Score;
                    }

                    double x = i * 50;
                    double y = 1000 - graphData.ElementAt(i).Score;
                    result.Add(new Point(x, y));
                }
            }
            return result;

        }

        /// <summary>
        /// Function to calculate the average % over all made exercises
        /// </summary>
        /// <returns>The average % the student achieved with a % mark at the end</returns>
        private string CalculateAverageCorrect()
        {
            if (FilteredResults.Count < 1)
            {
                return "0%";
            }
            double CorrectSum = 0;
            foreach (ExerciseResultModel stats in FilteredResults)
            {
                CorrectSum += stats.GetPercentage();
            }

            return ((double)Math.Floor((double)CorrectSum / FilteredResults.Count * 100) / 100) + "%";
        }
        /// <summary>
        /// calculates the average score for a specific student
        /// </summary>
        /// <returns>a double floored of all the scores divided by the amount of scores</returns>
        private double CalculateAverageScore()
        {
            if (FilteredResults.Count < 1)
            {
                return 0;
            }
            int ScoreSum = 0;
            foreach (ExerciseResultModel stats in FilteredResults)
            {
                ScoreSum += stats.Score;
            }
            return (double)Math.Floor((double)ScoreSum / FilteredResults.Count * 100) / 100;
        }

        /// <summary>
        /// Returns the help information for this window.
        /// </summary>
        /// <returns>The WindowHelpModel for this window.</returns>
        public WindowHelpModel GetHelp()
        {
            return new("Hulp - Statistieken", "Hier kun je terug zien hoe goed je de afgelopen oefeningen hebt gemaakt. Bekijk meer over elke poging, of bekijk je overzicht in de grafiek.\n\n" +
                "Je kan ook selecteren van welke periode je je resultaten wilt terugzien.");
        }

        /// <summary>
        /// Updates the nav button backgrounds based on the current screen.
        /// </summary>
        public void Update()
        {
            if (Sidebar.DataContext is IViewModel vm)
            {
                vm.Update();
            }
        }
    }
}
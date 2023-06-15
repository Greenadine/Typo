using Typo.Model.Exercise;
using Typo.Utils;
using Typo.ViewModel;

namespace UnitTests {
    [TestFixture]
    public class ResultCalculationTest {

        [TestCase(2, 0, 0, 1000)]
        [TestCase(2, 0, 60, 940)]
        [TestCase(2, 0, 120, 880)]
        [TestCase(2, 0, 180, 820)]
        [TestCase(2, 0, 240, 760)]
        [TestCase(2, 0, 300, 700)]
        [TestCase(1, 1, 0, 500)]
        [TestCase(1, 1, 60, 470)]
        [TestCase(1, 1, 120, 440)]
        [TestCase(1, 1, 180, 410)]
        [TestCase(1, 1, 240, 380)]
        [TestCase(1, 1, 300, 350)]
        [TestCase(10000,34904,430, 126)]
        public void TestScoreCalculationWords(int numberCorrect, int numberIncorrect, int seconds, int expected) {
            ExerciseUtils.SelectedExercise = new WordsExerciseModel(0,0,"",Difficulty.Hard,false,new List<string> {"a","b" });
            WordExerciseViewModel vm = new WordExerciseViewModel();
            vm.TimerInt = seconds;
            int score = vm.CalculateScore(numberCorrect, (numberCorrect + numberIncorrect));
            Assert.That(score, Is.EqualTo(expected));
        }
        [TestCase(2, 0, 0, 0)]
        [TestCase(2, 0, 60, 60)]
        [TestCase(2, 0, 120, 120)]
        [TestCase(2, 0, 180, 180)]
        [TestCase(2, 0, 240, 240)]
        [TestCase(2, 0, 300, 300)]
        [TestCase(1, 1, 0, 0)]
        [TestCase(1, 1, 60, 30)]
        [TestCase(1, 1, 120, 60)]
        [TestCase(1, 1, 180, 90)]
        [TestCase(1, 1, 240, 120)]
        [TestCase(1, 1, 300, 150)]
        [TestCase(10000, 34904, 6785, 1511)]

        public void TestScoreCalculationMarathon(int numberCorrect,int numberIncorrect,int seconds, int expected) {
            ExerciseUtils.SelectedExercise = new MarathonExerciseModel(0, 0, "", Difficulty.Hard, false, new List<string> { "a", "b" });
            MarathonExerciseViewModel vm = new MarathonExerciseViewModel();
            vm.ElapsedTime = seconds;
            int score = vm.CalculateScore(numberCorrect, (numberCorrect + numberIncorrect));
            Assert.That(score, Is.EqualTo(expected));
        }
    }
}
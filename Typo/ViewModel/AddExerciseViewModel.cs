using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Typo.Model;
using Typo.Utils;
using Typo.View;
using Typo.View.Sidebar;
using static Typo.Model.Exercise.Mode;
using Path = System.IO.Path;

namespace Typo.ViewModel
{
    public class AddExerciseViewModel : PropertyChangable, IViewModel, IHelp
    {
        // Sidebar
        public TeacherSidebarView Sidebar { get; set; }

        // Variables
        private string _errorMessage = string.Empty;
        private string _content = string.Empty;

        #region Binding properties
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public string Name { get; set; } = string.Empty;
        public int Mode { get; set; }
        public int Difficulty { get; set; }
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }
        #endregion

        #region Commands
        public Command AddExerciseCommand { get; private set; }
        public Command ImportCommand { get; private set; }
        public Command ClearContentCommand { get; private set; }
        public Command ReturnCommand { get; private set; }
        #endregion

        public AddExerciseViewModel()
        {
            Sidebar = new();

            AddExerciseCommand = new(AddExercise);
            ImportCommand = new(Import);
            ClearContentCommand = new(ClearContent);
            ReturnCommand = new(Return);
        }

        /// <summary>
        /// Navigates the application to the teacher home view.
        /// </summary>
        private void Return()
        {
            App.CurrentWindow = new TeacherHomeView();
        }

        /// <summary>
        /// Checks all the input of the view before adding a new exercise.
        /// </summary>
        private bool CheckInput()
        {
            // Check if the name isn't empty
            string name = Name.Trim();
            if (name.Length == 0)
            {
                SetErrorMessage("Vul een naam in");
                return false;
            }

            // Check if a mode is selected
            if (Mode == -1)
            {
                SetErrorMessage("Selecteer een modus");
                return false;
            }

            // Check if a difficulty is selected
            if (Difficulty == -1)
            {
                SetErrorMessage("Selecteer een moeilijkheid");
                return false;
            }

            // Check if the content isn't empty
            string content = Content.Trim();
            if (content.Length == 0)
            {
                switch (Mode)
                {
                    case (int)Words:
                        SetErrorMessage("Vul een aantal woorden in");
                        return false;
                    case (int)Text:
                        SetErrorMessage("Vul een tekst in");
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Adds a new exercise to the database, based on the input of the view.
        /// </summary>
        private void AddExercise()
        {
            if (!CheckInput())
            {
                return;
            }

            string name = Name.Trim();
            string content = Content.Trim();

            int classId = 1; // TODO implement
            bool visibility = true; // TODO implement

            // If the exercise wasn't successfull added
            if (!ExerciseUtils.AddExercise(name, classId, Mode, Difficulty, visibility, content))
            {
                App.ShowMessageBox("Er is een fout opgetreden bij het aanmaken van de oefening.", "Fout", MessageBoxButton.OK);
                return;
            }
            App.ShowMessageBox("De oefening is aangemaakt.", "Success", MessageBoxButton.OK);

            // Return to home
            App.CurrentWindow = new TeacherHomeView();
        }

        /// <summary>
        /// Imports text from a file.
        /// </summary>
        private void Import()
        {
            if (Mode == -1)
            {
                SetErrorMessage("Selecteer een modus");
                return;
            }

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Open text file"; // dialog title
            fileDialog.InitialDirectory = @"c:\"; // starting point filedialog
            fileDialog.Filter = "txt files (*.txt)|*.txt|pdf files (*.pdf) |*.pdf;"; // filters for .txt and .pdf
            fileDialog.ShowDialog();

            string inputString = ExtractText(fileDialog); // fills textbox in DialogAddAssignmentView
            if (Mode == (int)Words || Mode == (int)Marathon)
            {
                inputString = inputString.Replace("\t", "").Replace("\n", ";").Replace(" ", ";");
            }

            SetErrorMessage(string.Empty);
            Content = inputString;
        }

        /// <summary>
        /// Extracts the text from the file selected by the dialog.
        /// </summary>
        /// <param name="fileDialog">The dialog.</param>
        /// <returns>The text from the selected file.</returns>
        private string ExtractText(OpenFileDialog fileDialog)
        {
            if (Path.GetExtension(fileDialog.FileName) == ".pdf") // checks if extension is .pdf
            {
                PdfReader reader = new(fileDialog.FileName);

                StringWriter output = new();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    output.WriteLine(PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy()));
                }

                return output.ToString(); // fills textbox in DialogAddAssignmentView
            }

            if (Path.GetExtension(fileDialog.FileName) == ".txt") // checks if extension is .txt
            {
                StreamReader sr = File.OpenText(fileDialog.FileName); // creates a StreamReader to open and get the text from the .txt file
                using (sr)
                {
                    string txtText = sr.ReadToEnd(); // fills textbox  in DialogAddAssignmentView with text from StreamReader
                    return txtText;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Clears the content text box.
        /// </summary>
        private void ClearContent()
        {
            Content = string.Empty;
        }

        /// <summary>
        /// Changes the error message that is being displayed.
        /// </summary>
        /// <param name="message"></param>
        private void SetErrorMessage(string message)
        {
            ErrorMessage = message;
            App.CurrentWindow.UpdateLayout();
        }

        /// <summary>
        /// Returns the help information for this window.
        /// </summary>
        /// <returns>The WindowHelpModel for this window.</returns>
        public WindowHelpModel GetHelp()
        {
            return new("Hulp bij nieuwe oefeningen toevoegen", "Om een nieuwe oefening toe te voegen, kiest u eerst een naam voor de nieuwe oefening.\n" +
                "Vervolgens selecteert u wat voor een soort oefening het is. Bij een woordenmodus oefening typt de leerling telkens zo snel mogelijk een een woord dat wordt getoond op het scherm correct over." +
                "Het doel is om ze allemaal correct over te typen.\nBij een tekstmodus oefening typt de leerling een gehele tekst over binnen een bepaalde tijd. " +
                "Aan het einde van elke oefening wordt meteen getoond hoe de leerling gepresteerd heeft.");
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

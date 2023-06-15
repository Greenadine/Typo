namespace Typo.Model.User
{
    public class StudentModel : UserModel
    {
        // Properties
        public int? ClassId { get; }

        // Constructors
        public StudentModel(string username, string name, int? classId) : base(username, name)
        {
            ClassId = classId;
        }
    }
}

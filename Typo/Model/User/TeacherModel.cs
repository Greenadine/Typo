using System.Collections.Generic;

namespace Typo.Model.User
{
    public class TeacherModel : UserModel
    {
        // Properties
        public List<int> ClassIds { get; }

        // Constructors
        public TeacherModel(string username, string name, List<int> classIds) : base(username, name)
        {
            ClassIds = classIds;
        }
    }
}

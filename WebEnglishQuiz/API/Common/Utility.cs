namespace API.Common
{
    public static class Utility
    {
        public static string NowToString()
        {
            return "";
        }
        public class ResponseStatus
        {
            public string Message { get; set; } = Variables.ResponseOk;
            public object Data { get; set; } = null;
            public ResponseStatus(string message, object data)
            {
                Message = message;
                Data = data;
            }
            public ResponseStatus(string message)
            {
                Message = message;
            }
        }
        public class MostSubjectTest
        {
            public string username { get; set; }
            public int subject { get; set; }
            public int times { get; set; }
        }
        public class CorrectAnswer
        {
            public int Correct { get; set; }
            public int Question { get; set; }
        }
        public class UserQuizList
        {
            public string Username { get; set; }
            public int Subjectno { get; set; }
            public int Testno { get; set; }
        }
        public class SubjectDashboard
        {
            public string Name { get; set; }
            public int Enroll { get; set; }
            public int QuizNum { get; set; }
            public int QuestionNum { get; set; }
            public int PassingPercentage { get; set; }
        }
        public static bool Compare(string a, string b)
        {
            a = a.ToLower().Trim();
            b = b.ToLower().Trim();
            return a.Equals(b);
        }
    }
}

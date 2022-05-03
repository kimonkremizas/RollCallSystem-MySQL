using RollCallSystem.Database;

namespace RollCallSystem.APIModels
{
    public class TrimmedLesson
    {
        public int Id { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public DateTime StartTime { get; set; }
        public int? Code { get; set; }
        public DateTime? CodeTime { get; set; }
        public int? CampusId { get; set; }
        public string CampusName { get; set; }

        public TrimmedLesson() { }

        public TrimmedLesson(int id, DateTime startTime, int? subjectId = null, int? code = null, DateTime? codeTime = null, int? campusId = null)
        {
            Id = id;
            SubjectId = subjectId;
            Code = code;
            CodeTime = codeTime;
            CampusId = campusId;
            StartTime = startTime;
            SubjectName = "";
        }
    }
}

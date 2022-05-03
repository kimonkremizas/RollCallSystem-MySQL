namespace RollCallSystem.APIModels
{
    public class TrimmedSubject
    {
        public int Id { get; set; }
        public int? TeacherId { get; set; }
        public string Name { get; set; } = null!;

        public TrimmedSubject() { }

        public TrimmedSubject(int id, string name, int? teacherId = null)
        {
            Id = id;
            Name = name;
            TeacherId = teacherId;
        }
    }
}

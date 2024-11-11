namespace Sprachfeatures
{
    public enum Gender
    {
        Male,
        Female,
        Diverse
    }

    public class PersonUsingTuples
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public Gender Gender { get; set; } = Gender.Diverse;

        public (string FirstName, string MiddleName, string LastName, Gender Gender) GetFullName()
        {
            return (FirstName, !string.IsNullOrWhiteSpace(MiddleName) ? MiddleName : string.Empty, LastName, Gender);
        }

        // Vor C# 7
        public Tuple<string, string, string, Gender> GetFullNameOld()
        {
            var middleName = !string.IsNullOrWhiteSpace(MiddleName) ? MiddleName : string.Empty;
            return Tuple.Create(FirstName, middleName, LastName, Gender);
        }
    }
}

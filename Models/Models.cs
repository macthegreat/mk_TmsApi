public record EnrollmentRecord(
    string Id,
    string StudentId,
    string CourseCode,
    DateTime EnrolledAt
);
//add course class
// public class Course
// {
//     public int Id { get; init; }
//     public required string Code { get; init; }

//     public required string Title
//     {
//         get;
//         set => field = !string.IsNullOrWhiteSpace(value)
//             ? value
//             : throw new ArgumentException(
//                 "Title cannot be empty or whitespace.",
//                 nameof(value)
//             );
//     }

//     public int Capacity
//     {
//         get;
//         set => field = value > 0
//             ? value
//             : throw new ArgumentOutOfRangeException(
//                 nameof(value),
//                 "Capacity must be greater than zero."
//             );
//     }

//     public int EnrolledCount { get; set; }
// }


// // add student class
// public class Student
// {
//     public int Id { get; init; }

//     public string RegistrationNumber { get; set; } = null!;

//     public required string Name
//     {
//         get;
//         set => field = !string.IsNullOrWhiteSpace(value)
//             ? value
//             : throw new ArgumentException(
//                 "Name cannot be empty.",
//                 nameof(value)
//             );
//     }

//     public int Age
//     {
//         get;
//         set => field = value is >= 16 and <= 100
//             ? value
//             : throw new ArgumentOutOfRangeException(
//                 nameof(value),
//                 "Age must be between 16 and 100."
//             );
//     }

//     public decimal GPA
//     {
//         get;
//         set => field = value is >= 0.0m and <= 4.0m
//             ? value
//             : throw new ArgumentOutOfRangeException(
//                 nameof(value),
//                 "GPA must be between 0.0 and 4.0."
//             );
//     }

//     public bool IsActive { get; set; }
// }

// adding interface for gradable items

public interface IGradable
{
    string Title { get; }

    decimal CalculateGrade();
}

// adding quiz class that implements IGradable

public class Quiz : IGradable
{
    public required string Title { get; init; }

    public required int CorrectAnswers { get; init; }

    public required int TotalQuestions { get; init; }

    public decimal CalculateGrade()
    {
        if (TotalQuestions == 0)
            return 0m;

        return (decimal)CorrectAnswers
            / TotalQuestions * 100m;
    }
}

// adding lab assignment class that implements IGradable

public class LabAssignment : IGradable
{
    public required string Title { get; init; }

    public required decimal FunctionalityScore { get; init; }

    public required decimal CodeQualityScore { get; init; }

    public decimal CalculateGrade()
    {
        return (FunctionalityScore * 0.7m)
             + (CodeQualityScore * 0.3m);
    }




}
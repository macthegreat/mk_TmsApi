namespace MK_TmsApi.Entities;

public class Course
{
    public int Id {get; set;}
     public required string Code {get;set;}

     public required string Title{get;set;}
     public int capacity{get;set;}
    
    public ICollection<Enrollment> Enrollments {get;set;} =new List<Enrollment>();

}
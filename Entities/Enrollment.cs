namespace MK_TmsApi.Entities;

public class Enrollment
{
    public int Id {get;set;}
    public int StudentId{get;set;}
    public int CourseId{get;set;}
    public decimal? Grade{get;set;}
    public DateTime EnrolledAt {get;set;} =DateTime.UtcNow;

    public Student student {get;set;}=null;
    public Course course {get;set;} =null;

}
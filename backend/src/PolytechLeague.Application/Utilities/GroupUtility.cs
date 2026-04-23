using System;

namespace PolytechLeague.Application.Utilities;

public static class GroupUtility
{
    private const int AcademicYearStartMonth = 9;

    public static int CalculateCourse(int enrollmentYear, DateTime referenceDate)
    {
        if (enrollmentYear <= 0) return 0;
        
        int course = referenceDate.Year - enrollmentYear;
        if (referenceDate.Month >= AcademicYearStartMonth)
        {
            course++;
        }

        if (course < 1) course = 1;
        return course;
    }

    public static string CalculateGroupName(int specialty, int enrollmentYear, int groupNumber, DateTime referenceDate)
    {
        int course = CalculateCourse(enrollmentYear, referenceDate);
        if (course > 4) return "Випускник";
        if (course < 1) return "Абітурієнт";

        return $"{specialty}{course}{groupNumber}";
    }

    public static (int specialty, int enrollmentYear, int groupNumber) ParseGroup(string groupName, DateTime referenceDate)
    {
        if (string.IsNullOrWhiteSpace(groupName) || groupName.Length < 3)
        {
            return (0, 0, 0);
        }

        if (int.TryParse(groupName.Substring(0, 1), out int specialty) &&
            int.TryParse(groupName.Substring(1, 1), out int course) &&
            int.TryParse(groupName.Substring(2, 1), out int groupNumber))
        {
            int enrollmentYear = referenceDate.Year - course;
            if (referenceDate.Month >= AcademicYearStartMonth)
            {
                enrollmentYear++;
            }
            return (specialty, enrollmentYear, groupNumber);
        }

        return (0, 0, 0);
    }

    public static string GetCurrentGroupName(int specialty, int enrollmentYear, int groupNumber)
    {
        return CalculateGroupName(specialty, enrollmentYear, groupNumber, DateTime.Now);
    }
}

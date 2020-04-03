
using cw4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw4.DAL
{
    public interface IStudentDbService
    {
        IEnumerable<Student> GetStudents();
        IEnumerable<Enrollment> GetStudentEnrollments(string idStudent);
    }
}

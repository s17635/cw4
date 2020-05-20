
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
        bool CheckIndex(string index);
        bool CheckLoginAndPassword(string login, string password);
        void SetRefreshToken(string login, string refToken);
        bool CheckLoginAndRefreshToken(string login, string refToken);
        void AddStudent(Student student);
    }
}

using cw4.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw4.DAL
{
    public class StudentDbService : IStudentDbService
    {
        private string SqlConn = "Data Source=db-mssql;Initial Catalog=s17635;Integrated Security=True";
        public IEnumerable<Student> GetStudents()
        {
            var output = new List<Student>();
            using(var client = new SqlConnection(SqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = "SELECT * FROM Student";

                    client.Open();
                    var dr = command.ExecuteReader();

                    while(dr.Read())
                    {
                        output.Add(new Student
                        {
                            IndexNumber = dr["IndexNumber"].ToString(),
                            FirstName = dr["FirstName"].ToString(),
                            LastName = dr["LastName"].ToString(),
                            BirthDate = DateTime.Parse(dr["BirthDate"].ToString()),
                            IdEnrollment = int.Parse(dr["IdEnrollment"].ToString())
                        });
                    }
                }
            }

            return output;
        }

        public IEnumerable<Enrollment> GetStudentEnrollments(string idStudent)
        {
            var output = new List<Enrollment>();
            using (var client = new SqlConnection(SqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = "SELECT e.IdEnrollment, Semester, e.IdStudy, StartDate, Name as StudyName FROM Enrollment e INNER JOIN Student s on s.IdEnrollment = e.IdEnrollment INNER JOIN Studies st on st.IdStudy=e.IdStudy  WHERE s.IndexNumber = @index";
                    command.Parameters.AddWithValue("index", idStudent);

                    client.Open();
                    var dr = command.ExecuteReader();

                    while (dr.Read())
                    {
                        output.Add(new Enrollment
                        {
                            IdEnrollment = int.Parse(dr["IdEnrollment"].ToString()),
                            Semester = int.Parse(dr["Semester"].ToString()),
                            IdStudy = int.Parse(dr["IdStudy"].ToString()),
                            StartDate = DateTime.Parse(dr["StartDate"].ToString()),
                            StudyName = dr["StudyName"].ToString()
                        });
                    }
                }
            }

            return output;
        }

        public bool CheckIndex(string index)
        {
            using (var client = new SqlConnection(SqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber=@indexNumber";
                    command.Parameters.AddWithValue("indexNumber", index);
                    client.Open();

                    var dr = command.ExecuteReader();
                    return dr.Read();
                }
            }
        }
    }
}

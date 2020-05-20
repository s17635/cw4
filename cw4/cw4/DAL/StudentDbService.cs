using cw4.Models;
using cw4.Other;
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
            using (var client = new SqlConnection(SqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = "SELECT * FROM Student";

                    client.Open();
                    var dr = command.ExecuteReader();

                    while (dr.Read())
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
                    client.Open();
                    command.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber=@indexNumber";
                    command.Parameters.AddWithValue("indexNumber", index);

                    var dr = command.ExecuteReader();
                    return dr.Read();
                }
            }
        }
        public bool CheckLoginAndPassword(string login, string password)
        {
            using (var client = new SqlConnection(SqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    client.Open();
                    command.CommandText = "SELECT IndexNumber,Password,Salt FROM Student WHERE IndexNumber=@indexNumber";
                    command.Parameters.AddWithValue("indexNumber", login);

                    var dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        string indexNumberDB = (string)dr["IndexNumber"];
                        string passwordDB = (string)dr["Password"];
                        string saltDB = (string)dr["Salt"];

                        return PasswordHasher.Validate(password, saltDB, passwordDB);
                    }
                    else
                        return false;
                }
            }
        }

        public bool CheckLoginAndRefreshToken(string login, string refToken)
        {
            using (var client = new SqlConnection(SqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    client.Open();
                    command.CommandText = "SELECT IndexNumber,RefreshToken FROM Student WHERE IndexNumber=@indexNumber AND RefreshToken=@refreshToken";
                    command.Parameters.AddWithValue("indexNumber", login);
                    command.Parameters.AddWithValue("refreshToken", refToken);

                    var dr = command.ExecuteReader();
                    return dr.Read();
                }
            }
        }

        public void SetRefreshToken(string login, string refToken)
        {
            using (var client = new SqlConnection(SqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    client.Open();
                    command.CommandText = "UPDATE Student SET RefreshToken=@refreshToken WHERE IndexNumber=@indexNumber";
                    command.Parameters.AddWithValue("indexNumber", login);
                    command.Parameters.AddWithValue("refreshToken", refToken);

                    var dr = command.ExecuteNonQuery();
                }
            }
        }

        public void AddStudent(Student student)
        {
            using (var client = new SqlConnection(SqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    client.Open();
                    command.CommandText = "INSERT INTO Student(IndexNumber,FirstName,LastName,BirthDate,IdEnrollment,Password,Salt) VALUES (@indexNumber,@firstName,@lastName,@birthDate,@idEnrollment,@password,@salt)";
                    command.Parameters.AddWithValue("indexNumber", student.IndexNumber);
                    command.Parameters.AddWithValue("firstName", student.FirstName);
                    command.Parameters.AddWithValue("lastName", student.LastName);
                    command.Parameters.AddWithValue("birthDate", student.BirthDate);
                    command.Parameters.AddWithValue("idEnrollment", student.IdEnrollment);
                    command.Parameters.AddWithValue("password", student.Password);
                    command.Parameters.AddWithValue("salt", student.Salt);

                    var dr = command.ExecuteNonQuery();
                }
            }
        }
    }
}

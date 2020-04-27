using cw4.DTOs.Requests;
using cw4.Models;
using cw4.Other;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw4.DAL
{
    public class EnrollmentDbService : IEnrollmentDbService
    {
        private string SqlConn = "Data Source=db-mssql;Initial Catalog=s17635;Integrated Security=True";

        public EnrollResult AddAndEnrollStudent(EnrollStudentResponse enrollStudentResponse)
        {
            using (var client = new SqlConnection(SqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    client.Open();

                    var tran = client.BeginTransaction();

                    try
                    {
                        // Checking if studies exist
                        command.CommandText = "SELECT IdStudy FROM Studies WHERE name=@name";
                        command.Parameters.AddWithValue("name", enrollStudentResponse.Studies);
                        command.Transaction = tran;
                        Enrollment resultEnrollment;

                        var dr = command.ExecuteReader();
                        if (!dr.Read())
                        {
                            dr.Close();
                            tran.Rollback();
                            return new EnrollResult
                            {
                                Code = 400,
                                Message = "Studies don't exist"
                            };
                        }
                        else
                        {
                            int idStudy = (int)dr["IdStudy"];
                            int semester = 1;
                            int idEnrollment = -1;
                            command.CommandText = "SELECT * FROM Enrollment WHERE IdStudy=@idStudy AND Semester=@semester";
                            command.Parameters.AddWithValue("idStudy", idStudy);
                            command.Parameters.AddWithValue("semester", semester);
                            dr.Close();
                            dr = command.ExecuteReader();
                            if (!dr.Read())
                            {
                                dr.Close();
                                command.CommandText = "SELECT Max(IdEnrollment) as max from Enrollment";
                                int maxIdEnrollment = -1;
                                dr = command.ExecuteReader();
                                if (dr.Read())
                                {
                                    maxIdEnrollment = (int)dr["max"];
                                    maxIdEnrollment++;
                                    idEnrollment = maxIdEnrollment;
                                }

                                dr.Close();
                                DateTime now = DateTime.Now;
                                command.CommandText = "INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate) VALUES (@idEnrollment,@semester2,@idStudy2,@date)";
                                command.Parameters.AddWithValue("idEnrollment", idEnrollment);
                                command.Parameters.AddWithValue("semester2", semester);
                                command.Parameters.AddWithValue("idStudy2", idStudy);
                                command.Parameters.AddWithValue("date", now);
                                command.ExecuteNonQuery();

                                resultEnrollment = new Enrollment
                                {
                                    IdEnrollment = idEnrollment,
                                    Semester = semester,
                                    IdStudy = idStudy,
                                    StartDate = now
                                };
                            }
                            else
                            {
                                idEnrollment = (int)dr["IdEnrollment"];
                                resultEnrollment = new Enrollment
                                {
                                    IdEnrollment = idEnrollment,
                                    Semester = (int)dr["Semester"],
                                    IdStudy = (int)dr["IdStudy"],
                                    StartDate = (DateTime)dr["StartDate"]
                                };
                            }

                            dr.Close();
                            command.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber=@indexNumber";
                            command.Parameters.AddWithValue("indexNumber", enrollStudentResponse.IndexNumber);
                            dr = command.ExecuteReader();
                            if (dr.Read())
                            {
                                dr.Close();
                                tran.Rollback();
                                return new EnrollResult
                                {
                                    Code = 400,
                                    Message = "Student with this index number is already present in the Database"
                                };
                            }

                            dr.Close();
                            command.CommandText = "INSERT INTO Student(IndexNumber,FirstName,LastName,BirthDate,IdEnrollment) VALUES (@indexNumber2,@firstName,@lastName,@birthDate,@idEnrollment2)";
                            command.Parameters.AddWithValue("indexNumber2", enrollStudentResponse.IndexNumber);
                            command.Parameters.AddWithValue("firstName", enrollStudentResponse.FirstName);
                            command.Parameters.AddWithValue("lastName", enrollStudentResponse.LastName);
                            command.Parameters.AddWithValue("birthDate", enrollStudentResponse.BirthDate);
                            command.Parameters.AddWithValue("idEnrollment2", idEnrollment);
                            command.ExecuteNonQuery();


                            dr.Close();
                            tran.Commit();

                        }
                        return new EnrollResult
                        {
                            Code = 201,
                            Message = "Ok",
                            Enrollment = resultEnrollment
                        };
                    }
                    catch (SqlException)
                    {
                        tran.Rollback();
                    }
                    return new EnrollResult
                    {
                        Code = 500,
                        Message = "SQL Exception"
                    };
                }
            }
        }

        public EnrollResult PromoteStudent(PromoteStudentRequest request)
        {
            using (var client = new SqlConnection(SqlConn))
            {
                using (var command = new SqlCommand())
                {
                    Enrollment result = null;
                    command.Connection = client;
                    client.Open();

                    command.CommandText = "SELECT IdEnrollment FROM Enrollment e INNER JOIN Studies s ON e.IdStudy = s.IdStudy WHERE s.Name=@studyName AND Semester=@sem";
                    command.Parameters.AddWithValue("studyName", request.Studies);
                    command.Parameters.AddWithValue("sem", request.Semester);
                    var dr = command.ExecuteReader();
                    if(!dr.Read())
                    {
                        return new EnrollResult
                        {
                            Code = 404,
                            Message = "W tabeli Enrollment nie istnieje wpis o podanych wartościach"
                        };
                    }
                    dr.Close();

                    command.CommandText = "PromoteStudents";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("Studies", request.Studies);
                    command.Parameters.AddWithValue("Semester", request.Semester);
                    dr = command.ExecuteReader();

                    if(dr.Read())
                    {
                        result = new Enrollment
                        {
                            IdEnrollment = (int)dr["IdEnrollment"],
                            Semester = (int)dr["Semester"],
                            IdStudy = (int)dr["IdStudy"],
                            StartDate = (DateTime)dr["StartDate"]
                        };
                    }

                    return new EnrollResult
                    {
                        Code = 201,
                        Enrollment = result
                    };
                }
            }
        }
    }
}
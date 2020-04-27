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
                            command.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdStudy=@idStudy AND Semester=@semester";
                            command.Parameters.AddWithValue("idStudy", idStudy);
                            command.Parameters.AddWithValue("semester", semester);
                            dr.Close();
                            dr = command.ExecuteReader();
                            if (!dr.Read())
                            {
                                command.CommandText = "INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate) VALUES (10,@semester,@idStudy,@date)";
                                command.Parameters.AddWithValue("idStudy", idStudy);
                                command.Parameters.AddWithValue("semester", semester);
                                command.Parameters.AddWithValue("date", DateTime.Now);
                                //dr.Close();
                                command.ExecuteNonQuery();
                                return new EnrollResult
                                {
                                    Code = 200,
                                    Message = "test"
                                };
                            }

                            dr.Close();
                            tran.Commit();
                        }
                        return new EnrollResult
                        {
                            Code = 200,
                            Message = "Ok"
                        };
                    }
                    catch (SqlException)
                    {
                        tran.Rollback();
                    }
                    return new EnrollResult
                    {
                        Code = 500,
                        Message = "test exception"
                    };
                }
            }
        }
    }
}
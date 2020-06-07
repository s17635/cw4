using cw4.DTOs.Requests;
using cw4.Models;
using cw4.Other;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw4.DAL
{
    public class EnrollmentDbService : IEnrollmentDbService
    {
        private readonly s17635Context context;

        public EnrollmentDbService()
        {
            context = new s17635Context();
        }
        public EnrollResult AddAndEnrollStudent(EnrollStudentResponse enrollStudentResponse)
        {
            Enrollment resultEnrollment;

            // Checking if studies exist
            var studies = context.Studies.FirstOrDefault(s => s.Name == enrollStudentResponse.Studies);

            if (studies == null)
            {
                return new EnrollResult
                {
                    Code = 400,
                    Message = "Studies don't exist"
                };
            }
            else
            {
                int idStudy = studies.IdStudy;
                int semester = 1;
                int idEnrollment = -1;
                var existingEnrollment = context.Enrollment.FirstOrDefault(e => e.IdStudy == idStudy && e.Semester == semester);
                if (existingEnrollment == null)
                {
                    var maxIdEnrollmentDB = context.Enrollment.Max(e => e.IdEnrollment);
                    idEnrollment = maxIdEnrollmentDB + 1;

                    DateTime now = DateTime.Now;

                    resultEnrollment = new Enrollment
                    {
                        IdEnrollment = idEnrollment,
                        Semester = semester,
                        IdStudy = idStudy,
                        StartDate = now
                    };
                    context.Enrollment.Add(resultEnrollment);

                }
                else
                {
                    idEnrollment = existingEnrollment.IdEnrollment;
                    resultEnrollment = new Enrollment
                    {
                        IdEnrollment = idEnrollment,
                        Semester = existingEnrollment.Semester,
                        IdStudy = existingEnrollment.IdStudy,
                        StartDate = existingEnrollment.StartDate
                    };
                }

                var stud = context.Student.FirstOrDefault(s => s.IndexNumber == enrollStudentResponse.IndexNumber);
                if (stud != null)
                {
                    return new EnrollResult
                    {
                        Code = 400,
                        Message = "Student with this index number is already present in the Database"
                    };
                }

                var newStud = new Student()
                {
                    IndexNumber = enrollStudentResponse.IndexNumber,
                    FirstName = enrollStudentResponse.FirstName,
                    LastName = enrollStudentResponse.LastName,
                    BirthDate = enrollStudentResponse.BirthDate,
                    IdEnrollment = idEnrollment
                };
                context.Student.Add(newStud);
                context.SaveChanges();

            }
            return new EnrollResult
            {
                Code = 201,
                Message = "Ok",
                Enrollment = resultEnrollment
            };
        }


        public EnrollResult PromoteStudent(PromoteStudentRequest request)
        {
            Enrollment result = null;

            var existingEnrollment = (from e in context.Enrollment
                                      join s in context.Studies
                                      on e.IdStudy equals s.IdStudy
                                      where s.Name == request.Studies && e.Semester == request.Semester
                                      select e).FirstOrDefault();


            if (existingEnrollment == null)
            {
                return new EnrollResult
                {
                    Code = 404,
                    Message = "W tabeli Enrollment nie istnieje wpis o podanych wartościach"
                };
            }

            var newEnrollment = (from e in context.Enrollment
                                 join s in context.Studies
                                 on e.IdStudy equals s.IdStudy
                                 where s.Name == request.Studies && e.Semester == (request.Semester + 1)
                                 select e).FirstOrDefault();

            if (newEnrollment == null)
            {
                result = new Enrollment()
                {
                    IdEnrollment = context.Enrollment.Max(e => e.IdEnrollment) + 1,
                    Semester = request.Semester + 1,
                    IdStudy = existingEnrollment.IdStudy,
                    StartDate = DateTime.Now
                };
                context.Enrollment.Add(result);
            }
            else
            {
                result = newEnrollment;
            }

            var studentsToUpdate = context.Student.ToList();
            studentsToUpdate.Where(s => s.IdEnrollment == existingEnrollment.IdEnrollment).ToList().ForEach(s => s.IdEnrollment = newEnrollment.IdEnrollment);
            context.SaveChanges();

            return new EnrollResult
            {
                Code = 201,
                Enrollment = result
            };
        }

    }
}
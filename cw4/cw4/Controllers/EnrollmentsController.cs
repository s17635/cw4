using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw4.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using cw4.DAL;
using cw4.Other;
using cw4.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        /*
         * HttpGet - pobierz
         * HttpPost - dodaj zasób
         * HttpPut - zaktualizuj zasób
         * HttpPatch - załataj zasób (zaktualizuj część)
         * HttpDelete - usuń zasób
         */

        private readonly IEnrollmentDbService enrollmentDbService;

        public EnrollmentsController(IEnrollmentDbService enrollmentDbService)
        {
            this.enrollmentDbService = enrollmentDbService;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult EnrollStudent([FromBody] EnrollStudentRequest student)
        {
            EnrollStudentResponse response = new EnrollStudentResponse
            {
                IndexNumber = student.IndexNumber,
                FirstName = student.FirstName,
                LastName = student.LastName,
                BirthDate = DateTime.Parse(student.BirthDate),
                Studies = student.Studies
            };
            EnrollResult result = enrollmentDbService.AddAndEnrollStudent(response);
            if (result.Code == 201)
            {
                return StatusCode(result.Code, result.Enrollment);
            }
            else
            {
                return StatusCode(result.Code, result.Message);
            }
        }

        [HttpPost("promotions")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudent([FromBody] PromoteStudentRequest request)
        {
            EnrollResult result = enrollmentDbService.PromoteStudent(request);
            if (result.Code == 201)
            {
                return StatusCode(result.Code, result.Enrollment);
            }
            else
            {
                return StatusCode(result.Code, result.Message);
            }
        }


    }
}
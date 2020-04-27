using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw4.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using cw4.DAL;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        /*
         * HttpGet - pobierz
         * HttpPost - dodaj zasób
         * HttpPut - zaktualizuj zasób
         * HttpPatch - załataj zasób (zaktualizuj część)
         * HttpDelete - usuń zasób
         */

        private readonly IStudentDbService studentDbService;

        public StudentsController(IStudentDbService studentDbService)
        {
            this.studentDbService = studentDbService;
        }

        [HttpGet]
        public IActionResult GetStudents([FromQuery] string orderBy)
        {
            return Ok(studentDbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentEnrollments([FromRoute] string id)
        {
            return Ok(studentDbService.GetStudentEnrollments(id));
        }

        [HttpPost]
        public IActionResult AddStudent([FromBody] Student student)
        {
            // add to db, generating index number
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult ActualizeStudent([FromRoute] int id)
        {
            return Ok("Aktualizacja dokończona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent([FromRoute] int id)
        {
            return Ok("Usuwanie ukończone");
        }
    }
}
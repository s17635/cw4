using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw4.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using cw4.DAL;
using cw4.DTOs.Requests;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using cw4.Other;

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

        public IConfiguration Configuration;

        public StudentsController(IStudentDbService studentDbService, IConfiguration configuration)
        {
            this.studentDbService = studentDbService;
            this.Configuration = configuration;
        }


        [HttpGet("{id}")]
        public IActionResult GetStudentEnrollments([FromRoute] string id)
        {
            return Ok(studentDbService.GetStudentEnrollments(id));
        }

        [HttpPost]
        public IActionResult AddStudent([FromBody] Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            student.Salt = PasswordHasher.CreateSalt();
            student.Password = PasswordHasher.Create(student.Password, student.Salt);
            studentDbService.AddStudent(student);
            return Ok(student);
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (studentDbService.CheckLoginAndPassword(request.Login, request.Password))
            {
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier,"1"),
                new Claim(ClaimTypes.Name,request.Login),
                new Claim(ClaimTypes.Role,"employee")
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                    (
                        issuer: "Gakko",
                        audience: "Students",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(10),
                        signingCredentials: creds
                    );

                Guid refreshToken = Guid.NewGuid();
                studentDbService.SetRefreshToken(request.Login, refreshToken.ToString());

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken
                });
            }
            else
            {
                return Unauthorized("Niepoprawny login lub haslo");
            }
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken(RefreshTokenRequest request)
        {
            if (studentDbService.CheckLoginAndRefreshToken(request.Login, request.RefreshToken))
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier,"1"),
                    new Claim(ClaimTypes.Name,request.Login),
                    new Claim(ClaimTypes.Role,"employee")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                    (
                        issuer: "Gakko",
                        audience: "Students",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(10),
                        signingCredentials: creds
                    );

                Guid refreshToken = Guid.NewGuid();
                studentDbService.SetRefreshToken(request.Login, refreshToken.ToString());

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken
                });
            }
            else
            {
                return Unauthorized("Niepoprawny refresh token");
            }
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(studentDbService.GetStudentsEF());
        }

        [HttpPut("{id}")]
        public IActionResult ActualizeStudentEF([FromRoute] string id)
        {
            return Ok(studentDbService.ActualizeStudent(id));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudentEF([FromRoute] string id)
        {
            return Ok(studentDbService.DeleteStudent( id));
        }
    }
}
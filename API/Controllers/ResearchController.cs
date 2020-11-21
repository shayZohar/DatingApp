using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ResearchController : BaseApiController
    {
        private readonly DataContext _context;

        public ResearchController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("researchers")]
        public async Task<ActionResult<List<Researcher>>> getResearchers()
        {

            ////////////////////////////////////////////////////////////
            using var con = new SqliteConnection(_context.Database.GetConnectionString());
            con.Open();
            string stm = "SELECT * FROM researchers";

            using var cmd = new SqliteCommand(stm, con);
            await using SqliteDataReader rdr = cmd.ExecuteReader();
            List<Researcher> researchers = new List<Researcher>();
            while (rdr.Read())
            {
                Researcher res = new Researcher();
                res.Id = rdr.GetInt32(0);
                res.Firstname = rdr.GetString(1);
                res.Lastname = rdr.GetString(2);
                res.Email = rdr.GetString(3);
                res.Phone = rdr.GetString(4);
                Console.WriteLine($"{res.Firstname} {res.Lastname} {res.Email}");
                researchers.Add(res);
            }
            con.Close();
            //////////////////////////////////////////////////////////////////////////
            return researchers;
        }

        [HttpGet("researches")]
        public async Task<ActionResult<List<Research>>> getResearches()
        {

            ////////////////////////////////////////////////////////////
            using var con = new SqliteConnection(_context.Database.GetConnectionString());
            con.Open();
            string stm = "SELECT * FROM researchs";

            using var cmd = new SqliteCommand(stm, con);
            await using SqliteDataReader rdr = cmd.ExecuteReader();
            List<Research> researches = new List<Research>();
            while (rdr.Read())
            {
                Research res = new Research();
                res.Id = rdr.GetInt32(0);
                res.Name = rdr.GetString(1);
                res.University = rdr.GetString(2);
                researches.Add(res);
            }
            con.Close();
            //////////////////////////////////////////////////////////////////////////
            return researches;
        }

        [HttpGet("researchers/{researchid}")]
        public async Task<ActionResult<List<Researcher>>> getResearchersByResearchId(int researchid)
        {

            using var con = new SqliteConnection(_context.Database.GetConnectionString());
            con.Open();
            string stm = "SELECT * FROM researchers INNER JOIN researchers_researchs " +
            "ON researchers_researchs.research_id = " + researchid +
            " AND researchers._id = researchers_researchs.researcher_id";

            using var cmd = new SqliteCommand(stm, con);
            await using SqliteDataReader rdr = cmd.ExecuteReader();
            List<Researcher> researchers = new List<Researcher>();
            while (rdr.Read())
            {
                Researcher res = new Researcher();
                res.Id = rdr.GetInt32(0);
                res.Firstname = rdr.GetString(1);
                res.Lastname = rdr.GetString(2);
                res.Email = rdr.GetString(3);
                res.Phone = rdr.GetString(4);
                researchers.Add(res);
            }
            con.Close();
            //////////////////////////////////////////////////////////////////////////
            return researchers;
        }

        [HttpGet("researches/{researcherid}")]
        public async Task<ActionResult<List<Research>>> getResearchesByResearcherId(int researcherid)
        {

            ////////////////////////////////////////////////////////////
            using var con = new SqliteConnection(_context.Database.GetConnectionString());
            con.Open();
            string stm = "SELECT * FROM researchs INNER JOIN researchers_researchs " +
            "ON researchers_researchs.researcher_id = " + researcherid +
            " AND researchs._id = researchers_researchs.research_id";

            using var cmd = new SqliteCommand(stm, con);
            await using SqliteDataReader rdr = cmd.ExecuteReader();
            List<Research> researches = new List<Research>();
            while (rdr.Read())
            {
                Research res = new Research();
                res.Id = rdr.GetInt32(0);
                res.Name = rdr.GetString(1);
                res.University = rdr.GetString(2);
                researches.Add(res);
            }
            con.Close();
            //////////////////////////////////////////////////////////////////////////
            return researches;
        }

        [HttpPut("researchers/updateresearcher")]
        public async Task<ActionResult> updateResearcher(Researcher resToUpdate)
        {

            ////////////////////////////////////////////////////////////
            using var con = new SqliteConnection(_context.Database.GetConnectionString());
            con.Open();
            string stm = "UPDATE researchers SET first_name = \'" + resToUpdate.Firstname + "\' , last_name= \'" +
            resToUpdate.Lastname + "\' , email= \'" + resToUpdate.Email + "\' , phone= \'" + resToUpdate.Phone + "\' WHERE _id =" + resToUpdate.Id + " ;";

            using var cmd = new SqliteCommand(stm, con);
            await using SqliteDataReader rdr = cmd.ExecuteReader();
            con.Close();
            //////////////////////////////////////////////////////////////////////////;
            return Ok();
        }

        [HttpPost("researchers/deletefromresearch")]
        public async Task<ActionResult> deleteResearcherFromResearch(ResearcherResearch re)
        {
            using var con = new SqliteConnection(_context.Database.GetConnectionString());
            con.Open();
            string stm = "DELETE FROM researchers_researchs WHERE researcher_id= "
                        + re.researcherId + " AND research_id= " + re.researchId + ";";

            using var cmd = new SqliteCommand(stm, con);
            await using SqliteDataReader rdr = cmd.ExecuteReader();
            con.Close();
            //////////////////////////////////////////////////////////////////////////;
            return Ok();
        }

        [HttpPost("researchers/addresearcher")]
        public async Task<ActionResult> addResearcher(Researcher re)
        {
            using var con = new SqliteConnection(_context.Database.GetConnectionString());
            con.Open();
            string stm = "INSERT INTO researchers (first_name, last_name, email, phone) VALUES (\'" + re.Firstname+ "\',\'"+ re.Lastname +"\',\'" +
            re.Email+"\',\'" + re.Phone + " \')  ;";

            using var cmd = new SqliteCommand(stm, con);
            await using SqliteDataReader rdr = cmd.ExecuteReader();
            con.Close();
            //////////////////////////////////////////////////////////////////////////;
            return Ok();
        }

        [HttpPost("researchers/deleteresearcher")]
        public async Task<ActionResult> deleteResearcher(Researcher re)
        {
            using var con = new SqliteConnection(_context.Database.GetConnectionString());
            con.Open();
            string stm = "DELETE FROM researchers WHERE _id= " + re.Id + " ;";

            using var cmd = new SqliteCommand(stm, con);
            await using SqliteDataReader rdr = cmd.ExecuteReader();
            con.Close();
            //////////////////////////////////////////////////////////////////////////;
            return Ok();
        }

    }




}
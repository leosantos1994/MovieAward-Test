using Application.DB;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.WebPages;
using EFCore.BulkExtensions;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieAwardController : ControllerBase
    {
        MSSQLContext _repository;

        public MovieAwardController(MSSQLContext ctx)
        {
            _repository = ctx;
        }

        [HttpDelete, DisableRequestSizeLimit]
        [Route("ClearData")]
        public IActionResult ClearData()
        {
            try
            {
                _repository.BulkDelete(_repository.Movie.ToList());
                _repository.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("Upload")]
        public IActionResult Upload(List<Movie> movies)
        {
            try
            {
                if (movies.Count <= 0)
                    return BadRequest();
                _repository.BulkInsert(movies);
                _repository.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("GetMaxDifference")]
        public IActionResult GetMaxDifferenceBetweenTwoConsectiveWins()
        {
            try
            {
                List<Movie> movies = _repository.Movie.Where(x => x.Winner).ToList();
                var helper = new List<MovieHelperModel>();

                foreach (var groupedMovies in movies.Where(x => x.Winner).GroupBy(x => x.Producer))
                {
                    if (groupedMovies.Count() >= 2)
                        helper.Add(new MovieHelperModel() { Producer = groupedMovies.FirstOrDefault().Producer, DateDifference = groupedMovies.Sum(x => x.Year) });
                }
                string producer = helper.Where(x => x.DateDifference == helper.Max(x => x.DateDifference)).FirstOrDefault()?.Producer;

                return Ok(producer);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("GetConsecutiveWinner")]
        public IActionResult GetConsecutiveWinnerProducer()
        {
            try
            {
                List<Movie> movies = _repository.Movie.Where(x => x.Winner).ToList();
                string producer = GetConsecutive(movies);
                return Ok(producer);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


        private string GetConsecutive(List<Movie> movies)
        {
            string producer = "";
            foreach (var item in movies)
            {
                if (item.Producer.Contains(producer))
                    return item.Producer;

                producer = item.Producer.Split(" ")[0];
            }
            return "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Movie_API.Models;
using DbConnection;

namespace Movie_API.Controllers
{

    public class SearchQuery
    {
        public string query { get; set; }
    }

    public class HomeController : Controller
    {
        
        // API Key (v3 auth)
        // Value: f280f7c3732f01b778a4f9edabad8961

        // API Read Access Token (v4 auth)
        // eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJmMjgwZjdjMzczMmYwMWI3NzhhNGY5ZWRhYmFkODk2MSIsInN1YiI6IjVhY2U3ZjNhOTI1MTQxMGMwNjAyYTIxNSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.xp7vNy9bDQHYdgboXfWoKt8Zls2sCEEYysOdUZoZisE

        // Example API Request
        // https://api.themoviedb.org/3/movie/550?api_key=f280f7c3732f01b778a4f9edabad8961
        
        private readonly DbConnector _dbConnector;

        public HomeController(DbConnector connect)
        {
            _dbConnector = connect;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("movies")]
        public JsonResult GetMovies()
        {
            List<Dictionary<string, object>> AllNotes = _dbConnector.Query("SELECT title, vote_average, release_date FROM movies");

            return Json(AllNotes);
        }

        [HttpPost]
        [Route("movies")]
        public JsonResult SearchMovies([FromBody] SearchQuery searchStr)
        {
            var MovieInfo = new Dictionary<string, object>();
            var searchResults = new {
                title = "",
                vote_average = "",
                release_date = ""
            };

            WebRequest.GetMovieDataAsync(searchStr.query, ApiResponse =>
                {
                    MovieInfo = ApiResponse;
                    
                    JArray jArray = MovieInfo["results"] as JArray;
                    JObject jObject = (JObject)jArray[0] as JObject;

                    searchResults = new {
                        title = (string)jObject.GetValue("title"),
                        vote_average = (string)jObject.GetValue("vote_average"),
                        release_date = (string)jObject.GetValue("release_date")
                    };

                    _dbConnector.Query($"INSERT INTO movies (title, vote_average, release_date) VALUES ('{searchResults.title.Replace("'", "''")}', '{searchResults.vote_average}', '{searchResults.release_date}')");
                }
            ).Wait();

            return Json(searchResults);
        }

    }
}

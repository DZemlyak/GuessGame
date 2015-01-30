using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuessGame.Controllers
{
    public class HomeController : Controller
    {
        private const string Host = "Host";
        private const string Winner = "Winner";
        private const string SecretNumber = "SecretNumber";
        private const string History = "History";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult GetStatus() {
            var host = System.Web.HttpContext.Current.Application.Get(Host);
            if (host != null)
                return Json(new {
                    Host = host
                }, JsonRequestBehavior.AllowGet);

            var winner = System.Web.HttpContext.Current.Application.Get(Winner);
            if (winner == null)
                return Json(new {
                    Status = "The Game hasn't started yet. Start new Game!"
                }, JsonRequestBehavior.AllowGet);

            var secretNumber = System.Web.HttpContext.Current.Application.Get(SecretNumber);
            return Json(new {
                Winner = winner,
                SecretNumber = secretNumber
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GuessNumber(int number, string userName) {
            var host = System.Web.HttpContext.Current.Application.Get(Host);
            string result;

            if (host == null)
                return Json(new {
                    Result = "The Game hasn't started yet!"
                }, JsonRequestBehavior.AllowGet);

            if ((string) host == userName)
                return Json(new {
                    Result = "You can't play!"
                }, JsonRequestBehavior.AllowGet);
            
            var secretNumber = (int) System.Web.HttpContext.Current.Application.Get(SecretNumber);
            if (number > secretNumber)
                result = "Your number is bigger!";
            else if (number < secretNumber)
                result = "Your number is smaller!";
            else {
                result = "Right! You win!";
                System.Web.HttpContext.Current.Application.Set(Host, null);
                System.Web.HttpContext.Current.Application.Set(Winner, userName);
            }
            
            var history = ((Dictionary<string, List<int>>) System.Web.HttpContext.Current.Application.Get(History));
            history[userName].Add(number);

            return Json(new {
                Result = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StartGame(int number, string userName) {
            System.Web.HttpContext.Current.Application.Set(Host, userName);
            System.Web.HttpContext.Current.Application.Set(SecretNumber, number);
            System.Web.HttpContext.Current.Application.Set(History, new Dictionary<string, List<int>>());
            return Json(new {
                Result = ""
            }, JsonRequestBehavior.AllowGet);
        }

        public void SetHistory(string userName)
        {
            ((Dictionary<string, List<int>>)System.Web.HttpContext.Current.Application.Get(History))
                .Add(userName, new List<int>());
        }

        public ActionResult ShowHistory(string userName)
        {
            var history = String.Empty;
            var historyValues = ((Dictionary<string, List<int>>)System.Web.HttpContext.Current.Application.Get(History));
            history = historyValues[userName].Aggregate(history, (current, number) => current + number + ", ");
            history = history.Substring(0, history.Length - 2);
            return Json(new {
                History = "Your guesses: " + history
            }, JsonRequestBehavior.AllowGet);
        }

        public void SetAllHistory()
        {
            if (System.Web.HttpContext.Current.Application.Get(History) == null)
                System.Web.HttpContext.Current.Application.Set(History, new Dictionary<string, List<int>>());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SportAsso.Models;

namespace SportAsso.Controllers
{
    public class DisciplinesController : Controller
    {

        private dbSportAssoEntities db = new dbSportAssoEntities();
        // GET: Disciplines

        public static utilisateur FindUserByLogin(string login)
        {
            dbSportAssoEntities db = new dbSportAssoEntities();
            foreach (utilisateur u in db.utilisateur)
            {
                if (u.login == login)
                {
                    return u;
                }
            }
            return new utilisateur();
        }

        public static string GetUserFullNameByLogin(string login)
        {
            utilisateur u = FindUserByLogin(login);
            return u.prenom + ' ' + u.nom;
        }

        public static long GetIdByUserName(string login)
        {
            dbSportAssoEntities db = new dbSportAssoEntities();
            foreach (utilisateur u in db.utilisateur)
            {
                if (u.login == login)
                {
                    return u.utilisateur_id;
                }
            }
            return 0;
        }

        public ActionResult Index(string sortOrder)
        {
            if (User.Identity.Name != null && User.IsInRole("encadrant"))
            {
                long id = GetIdByUserName(User.Identity.Name);

                /*discipline*/
                IQueryable<discipline> discipline = from di in db.discipline where di.encadrant_id == id select di;
                ViewBag.discipline = discipline.ToList<discipline>();
                if (!discipline.Any())
                {
                    ViewBag.hasDiscipline = "true";
                }
                else
                {
                    ViewBag.hasDiscipline = "false";
                }

                /*sections*/
                IQueryable<section> section = from di in db.section where di.encadrant_id == id select di;
                ViewBag.section = section.ToList<section>();
                if (!section.Any())
                {
                    ViewBag.hasSection = "true";
                }
                else
                {
                    ViewBag.hasSection = "false";
                }

                /*seance*/
                IQueryable<seance> seance = from di in db.seance where di.encadrant_id == id select di;
                ViewBag.seance = seance.ToList<seance>();
                if (!seance.Any())
                {
                    ViewBag.hasSeance = "true";
                }
                else
                {
                    ViewBag.hasSeance = "false";
                }
                return View(discipline.ToList());
            }
            else
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                var disciplines = from d in db.discipline
                                  select d;

                switch (sortOrder)
                {
                    case "name_desc":
                        disciplines = disciplines.OrderByDescending(d => d.discipline_nom);
                        break;

                    default:
                        disciplines = disciplines.OrderBy(d => d.discipline_id);
                        break;
                }
                if (User.Identity.Name != null && User.IsInRole("admin"))
                {

                    return View(disciplines.ToList());
                }
                else
                {
                    /*discipline*/
                    IQueryable<discipline> discipline = from di in db.discipline select di;
                    ViewBag.disciplineAll = discipline.ToList<discipline>();
                    if (!discipline.Any())
                    {
                        ViewBag.hasDiscipline = "true";
                    }
                    else
                    {
                        ViewBag.hasDiscipline = "false";
                    }



                    return View(discipline.ToList());
                }
            }
        }

        // GET: Disciplines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            discipline discipline = db.discipline.Find(id);
            if (discipline == null)
            {
                return HttpNotFound();
            }
            return View(discipline);
        }

        // GET: Disciplines/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Disciplines/Create
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "discipline_id")] discipline discipline)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                // TODO: Add insert logic here
                db.discipline.Add(discipline);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Disciplines/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            discipline discipline = db.discipline.Find(id);
            if (discipline == null)
            {
                return HttpNotFound();
            }

            return View(discipline);
        }

        // POST: Disciplines/Edit/5
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "discipline_id,discipline_nom,encadrant_id,description")] discipline discipline)
        {
            if (ModelState.IsValid)
            {
                db.Entry(discipline).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(discipline);
        }

        // GET: Disciplines/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            discipline discipline = db.discipline.Find(id);
            if (discipline == null)
            {
                return HttpNotFound();
            }
            return View(discipline);
        }

        // POST: Disciplines/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            discipline discipline = db.discipline.Find(id);
            db.discipline.Remove(discipline);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

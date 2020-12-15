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
    public class SectionsController : Controller
    {

        private dbSportAssoEntities db = new dbSportAssoEntities();
        

        public static string FindDisciplineNameById(long? id)
        {
            if (id.HasValue)
            {
                dbSportAssoEntities db = new dbSportAssoEntities();
                return db.discipline.Find(id).discipline_nom;
            }
            return "empty";
        }

        public static int calculNbPLace(section section)
        {
            return (int)(section.places_max - section.nbParticipant);
        }

        public static string FindUserFullNameById(long? id)
        {
            if (id.HasValue)
            {
                dbSportAssoEntities db = new dbSportAssoEntities();
                utilisateur u = db.utilisateur.Find(id);
                return u.prenom + ' ' + u.nom;
            }
            return "non affecté";
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

        // GET: Sections
        public ActionResult Index(long? id, string sortOrder)
        {
            ViewBag.maDiscipline = FindDisciplineNameById(id);
            if (User.Identity.Name != null && User.IsInRole("encadrant"))
            {
                /*sections*/
                IQueryable<section> section = from di in db.section where (di.discipline_id == id) select di;
                List<section> sections = new List<section>();
                foreach (section sec in section)
                {
                    if (sec.encadrant_id == GetIdByUserName(User.Identity.Name))
                    {
                        sections.Add(sec);
                    }
                }
                ViewBag.section = sections;
                if (!sections.Any())
                {
                    ViewBag.hasSection = "true";
                }
                else
                {
                    ViewBag.hasSection = "false";
                }
                return View(sections.ToList());
            }
            else
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_asc" : "";

                var sections = from s in db.section
                               select s;

                switch (sortOrder)
                {
                    case "name_asc":
                        sections = sections.OrderBy(s => s.section_nom);
                        break;

                    default:
                        sections = sections.OrderBy(s => s.section_id);
                        break;
                }

                if (User.Identity.Name != null && User.IsInRole("admin"))
                {
                    return View(sections.ToList());
                }
                else
                {
                    /*sections*/
                    IQueryable<section> section = from di in db.section where di.discipline_id == id select di;
                    ViewBag.section = section.ToList<section>();
                    if (!section.Any())
                    {
                        ViewBag.hasSection = "true";
                    }
                    else
                    {
                        ViewBag.hasSection = "false";
                    }
                    return View(section.ToList());
                }
            }
        }

        // GET: Sections/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            section section = db.section.Find(id);
            if (section == null)
            {
                return HttpNotFound();
            }
            return View(section);
        }

        // GET: Sections/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sections/Create
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "section_id")] section section)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                db.section.Add(section);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {

                return View();
            }
        }

        // GET: Sections/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            section section = db.section.Find(id);
            if (section == null)
            {
                return HttpNotFound();
            }
            ViewBag.discipline_id = new SelectList(db.discipline, "discipline_id", "label", section.discipline_id);
            return View(section);
        }

        // POST: Sections/Edit/5
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "section_id,discipline_id,section_nom,encadrant_id,places_max,nbParticipant,prix,description")] section section)
        {
            if (ModelState.IsValid)
            {
                db.Entry(section).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.discipline_id = new SelectList(db.discipline, "discipline_id", "label", section.discipline_id);
            return View(section);
        }

        // GET: Sections/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            section section = db.section.Find(id);
            if (section == null)
            {
                return HttpNotFound();
            }
            return View(section);
        }

        // POST: Sections/Delete/5
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            section section = db.section.Find(id);
            db.section.Remove(section);
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

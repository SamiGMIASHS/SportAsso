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
    
    public class SeancesController : Controller
    {
        private dbSportAssoEntities db = new dbSportAssoEntities();

        public static string FindSectionNameById(long? id)
        {
            if (id.HasValue)
            {
                dbSportAssoEntities db = new dbSportAssoEntities();
                return db.section.Find(id).section_nom;
            }
            return "empty";
        }

        public static string FindFullTitleById(long section_id)
        {
            dbSportAssoEntities db = new dbSportAssoEntities();
            section s = db.section.Find(section_id);
            discipline d = db.discipline.Find(s.discipline_id);

            return d.discipline_nom + " - " + s.section_nom;
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

        public static string FindLieuFullNameById(long? id)
        {
            if (id.HasValue)
            {
                dbSportAssoEntities db = new dbSportAssoEntities();
                lieu l = db.lieu.Find(id);
                return l.lieu_nom;
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

        public static string FindDisciplineNameById(long? id)
        {
            if (id.HasValue)
            {
                dbSportAssoEntities db = new dbSportAssoEntities();
                return db.discipline.Find(id).discipline_nom;
            }
            return "empty";
        }

        public static section FindSectionById(long? id)
        {
            if (id.HasValue)
            {
                dbSportAssoEntities db = new dbSportAssoEntities();
                return db.section.Find(id);
            }
            return null;
        }

        // GET: Seances

        public ActionResult Index(long? id, string sortOrder)
        {
            ViewBag.maSection = FindSectionNameById(id);
            if (FindSectionById(id) != null)
            {
                ViewBag.maDiscipline = FindDisciplineNameById(FindSectionById(id).discipline_id);
            }
            if (User.Identity.Name != null && User.IsInRole("encadrant"))
            {
                /*seance*/
                IQueryable<seance> seance = from di in db.seance where (di.section_id == id) select di;
                List<seance> seances = new List<seance>();
                foreach (seance sea in seance)
                {
                    if (sea.encadrant_id == GetIdByUserName(User.Identity.Name))
                    {
                        seances.Add(sea);
                    }
                }
                ViewBag.seances = seances;
                if (!seances.Any())
                {
                    ViewBag.hasSeance = "true";
                }
                else
                {
                    ViewBag.hasSeance = "false";
                }
                return View(seances.ToList());
            }
            else
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "jour_asc" : "";

                var seances = from s in db.seance
                              select s;

                switch (sortOrder)
                {
                    case "jour_asc":
                        seances = seances.OrderBy(s => s.jour);
                        break;

                    default:
                        seances = seances.OrderBy(s => s.seance_id);
                        break;
                }

                if (User.Identity.Name != null && User.IsInRole("admin"))
                {

                    return View(seances.ToList());
                }
                else
                {
                    /*seance*/
                    IQueryable<seance> seance = from di in db.seance where di.section_id == id select di;
                    ViewBag.seances = seance.ToList<seance>();
                    if (!seance.Any())
                    {
                        ViewBag.hasSeance = "true";
                    }
                    else
                    {
                        ViewBag.hasSeance = "false";
                    }
                    return View(seance.ToList());
                }
            }
        }

        public static string GetNameSectionByID(long id)
        {
            dbSportAssoEntities db = new dbSportAssoEntities();
            return db.section.Find(id).section_nom;
        }

        public static string GetNameDisciplineBySeanceID(long id)
        {
            dbSportAssoEntities db = new dbSportAssoEntities();
            long dis_id = (long)db.section.Find(id).discipline_id;
            return db.discipline.Find(dis_id).discipline_nom;
        }

        // GET: Seances/Details/5
        [Authorize(Roles = " admin")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            seance seance = db.seance.Find(id);
            if (seance == null)
            {
                return HttpNotFound();
            }
            return View(seance);
        }

        // GET: Seances/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create(long? id)
        {
            ViewBag.lieu_id = new SelectList(db.lieu, "lieu_id", "lieu_nom");
            if (id.HasValue)
            {
                ViewBag.section_id = new SelectList(db.section, "section_id", "description", id);
            }
            else
            {
                ViewBag.section_id = new SelectList(db.section, "section_id", "description");
            }
            var responsables = new List<SelectListItem>();
            foreach (utilisateur u in db.utilisateur)
            {
                if (u.role_utilisateur == "encadrant")
                {
                    responsables.Add(new SelectListItem() { Text = u.prenom + " " + u.nom + " " + u.login, Value = "" + u.utilisateur_id });
                }
            }
            ViewBag.encadrant_id = responsables;

            var jours = new List<SelectListItem>();
            jours.Add(new SelectListItem() { Text = "Lundi", Value = "Lundi" });
            jours.Add(new SelectListItem() { Text = "Mardi", Value = "Mardi" });
            jours.Add(new SelectListItem() { Text = "Mercredi", Value = "Mercredi" });
            jours.Add(new SelectListItem() { Text = "Jeudi", Value = "Jeudi" });
            jours.Add(new SelectListItem() { Text = "Vendredi", Value = "Vendredi" });
            jours.Add(new SelectListItem() { Text = "Samedi", Value = "Samedi" });
            jours.Add(new SelectListItem() { Text = "Dimanche", Value = "Dimanche" });

            ViewBag.jour = jours;
            //ViewBag.encadrant_id = new SelectList(db.utilisateur, "utilisateur_id", "login");
            return View();
        }

        // POST: Seances/Create
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "seance_id,lieu_id,encadrant_id,jour,heure_debut,heure_fin,section_id")] seance seance)
        {
            if (ModelState.IsValid)
            {
                db.seance.Add(seance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.lieu_id = new SelectList(db.lieu, "lieu_id", "lieu_nom", seance.lieu_id);
            ViewBag.section_id = new SelectList(db.section, "section_id", "description", seance.section_id);
           
            var responsables = new List<SelectListItem>();
            foreach (utilisateur u in db.utilisateur)
            {
                if (u.role_utilisateur == "encadrant")
                {
                    responsables.Add(new SelectListItem() { Text = u.prenom + " " + u.nom + " " + u.login, Value = "" + u.utilisateur_id });
                }
            }
            ViewBag.encadrant_id = responsables;
            var jours = new List<SelectListItem>();
            jours.Add(new SelectListItem() { Text = "Lundi", Value = "Lundi" });
            jours.Add(new SelectListItem() { Text = "Mardi", Value = "Mardi" });
            jours.Add(new SelectListItem() { Text = "Mercredi", Value = "Mercredi" });
            jours.Add(new SelectListItem() { Text = "Jeudi", Value = "Jeudi" });
            jours.Add(new SelectListItem() { Text = "Vendredi", Value = "Vendredi" });
            jours.Add(new SelectListItem() { Text = "Samedi", Value = "Samedi" });
            jours.Add(new SelectListItem() { Text = "Dimanche", Value = "Dimanche" });

            ViewBag.jour = jours;
            return View(seance);
        }

        public static bool isJourSelected(seance s, string jour)
        {
            if (s.jour == jour)
            {
                return true;
            }
            return false;
        }

        // GET: Seances/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            seance seance = db.seance.Find(id);
            if (seance == null)
            {
                return HttpNotFound();
            }
            ViewBag.lieu_id = new SelectList(db.lieu, "lieu_id", "lieu_nom", seance.lieu_id);
            ViewBag.section_id = new SelectList(db.section, "section_id", "description", seance.section_id);
            var responsables = new List<SelectListItem>();
            foreach (utilisateur u in db.utilisateur)
            {
                if (u.role_utilisateur == "encadrant")
                {
                    if (u.utilisateur_id == seance.encadrant_id)
                    {
                        responsables.Add(new SelectListItem() { Text = u.prenom + " " + u.nom + " " + u.login, Value = "" + u.utilisateur_id, Selected = true });
                    }
                    else
                    {
                        responsables.Add(new SelectListItem() { Text = u.prenom + " " + u.nom + " " + u.login, Value = "" + u.utilisateur_id, Selected = false });
                    }

                }
            }
            ViewBag.encadrant_id = responsables;
            var jours = new List<SelectListItem>();
            jours.Add(new SelectListItem() { Text = "Lundi", Value = "Lundi", Selected = isJourSelected(seance, "Lundi") });
            jours.Add(new SelectListItem() { Text = "Mardi", Value = "Mardi", Selected = isJourSelected(seance, "Mardi") });
            jours.Add(new SelectListItem() { Text = "Mercredi", Value = "Mercredi", Selected = isJourSelected(seance, "Mercredi") });
            jours.Add(new SelectListItem() { Text = "Jeudi", Value = "Jeudi", Selected = isJourSelected(seance, "Jeudi") });
            jours.Add(new SelectListItem() { Text = "Vendredi", Value = "Vendredi", Selected = isJourSelected(seance, "Vendredi") });
            jours.Add(new SelectListItem() { Text = "Samedi", Value = "Samedi", Selected = isJourSelected(seance, "Samedi") });
            jours.Add(new SelectListItem() { Text = "Dimanche", Value = "Dimanche", Selected = isJourSelected(seance, "Dimanche") });

            ViewBag.jour = jours;
            return View(seance);
        }

        // POST: Seances/Edit/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "seance_id,lieu_id,encadrant_id,jour,heure_debut,heure_fin,section_id")] seance seance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.lieu_id = new SelectList(db.lieu, "lieu_id", "lieu_nom", seance.lieu_id);
            ViewBag.section_id = new SelectList(db.section, "section_id", "description", seance.section_id);
            var responsables = new List<SelectListItem>();
            foreach (utilisateur u in db.utilisateur)
            {
                if (u.role_utilisateur == "encadrant")
                {
                    if (u.utilisateur_id == seance.encadrant_id)
                    {
                        responsables.Add(new SelectListItem() { Text = u.prenom + " " + u.nom + " " + u.login, Value = "" + u.utilisateur_id, Selected = true });
                    }
                    else
                    {
                        responsables.Add(new SelectListItem() { Text = u.prenom + " " + u.nom + " " + u.login, Value = "" + u.utilisateur_id, Selected = false });
                    }

                }
            }
            ViewBag.encadrant_id = responsables;
            ViewBag.encadrant_id = responsables;
            var jours = new List<SelectListItem>();
            jours.Add(new SelectListItem() { Text = "Lundi", Value = "Lundi", Selected = isJourSelected(seance, "Lundi") });
            jours.Add(new SelectListItem() { Text = "Mardi", Value = "Mardi", Selected = isJourSelected(seance, "Mardi") });
            jours.Add(new SelectListItem() { Text = "Mercredi", Value = "Mercredi", Selected = isJourSelected(seance, "Mercredi") });
            jours.Add(new SelectListItem() { Text = "Jeudi", Value = "Jeudi", Selected = isJourSelected(seance, "Jeudi") });
            jours.Add(new SelectListItem() { Text = "Vendredi", Value = "Vendredi", Selected = isJourSelected(seance, "Vendredi") });
            jours.Add(new SelectListItem() { Text = "Samedi", Value = "Samedi", Selected = isJourSelected(seance, "Samedi") });
            jours.Add(new SelectListItem() { Text = "Dimanche", Value = "Dimanche", Selected = isJourSelected(seance, "Dimanche") });

            ViewBag.jour = jours;
            return View(seance);
        }

        // GET: Seances/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            seance seance = db.seance.Find(id);
            if (seance == null)
            {
                return HttpNotFound();
            }
            return View(seance);
        }

        // POST: Seances/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            seance seance = db.seance.Find(id);
            db.seance.Remove(seance);
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

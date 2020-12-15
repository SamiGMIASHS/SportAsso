using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SportAsso.Models;

namespace SportAsso.Controllers
{
    public class UtilisateursController : Controller
    {

        private dbSportAssoEntities db = new dbSportAssoEntities();

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

        // GET: Utilisateurs
        //[Authorize(Roles = "admin")]
        public ActionResult Index(string sortOrder, string searchString)
        {
            
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_asc" : "";
                ViewBag.RoleSortParm = String.IsNullOrEmpty(sortOrder) ? "role_desc" : "";

                var utilisateurs = from u in db.utilisateur
                                   select u;

                if (!String.IsNullOrEmpty(searchString))
                {
                    utilisateurs = utilisateurs.Where(u => u.prenom.Contains(searchString)
                                           || u.nom.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_asc":
                        utilisateurs = utilisateurs.OrderBy(u => u.nom);
                        break;

                    case "role_desc":
                        utilisateurs = utilisateurs.OrderByDescending(u => u.role_utilisateur);
                        break;

                    default:
                        utilisateurs = utilisateurs.OrderBy(u => u.utilisateur_id);
                        break;
                }
                return View(utilisateurs.ToList());
           }

        

        

        // GET: Utilisateurs/Details/5
        [Authorize(Roles = "admin")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            utilisateur utilisateur = db.utilisateur.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(utilisateur);
        }

        // GET: Utilisateurs/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            var types = new List<SelectListItem>();
            types.Add(new SelectListItem() { Text = "Adhérent", Value = "adherent" });
            types.Add(new SelectListItem() { Text = "Encadrant", Value = "encadrant" });
            types.Add(new SelectListItem() { Text = "Administrateur", Value = "admin" });

            ViewBag.role_utilisateur = types;
            return View();
        }

        // POST: Utilisateurs/Create
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "utilisateur_id,login,password,prenom,nom,adresse,telephone,role_utilisateur")] utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                db.utilisateur.Add(utilisateur);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var types = new List<SelectListItem>();
            types.Add(new SelectListItem() { Text = "Adhérent", Value = "adherent" });
            types.Add(new SelectListItem() { Text = "Encadrant", Value = "encadrant" });
            types.Add(new SelectListItem() { Text = "Administrateur", Value = "admin" });

            ViewBag.role_utilisateur = types;

            return View(utilisateur);
        }

        public static bool isMyRole(utilisateur u, string role)
        {
            if (u.role_utilisateur == role)
            {
                return true;
            }
            return false;
        }

        // GET: Utilisateurs/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            utilisateur utilisateur = db.utilisateur.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }

            var types = new List<SelectListItem>();
            types.Add(new SelectListItem() { Text = "Adhérent", Value = "adherent", Selected = isMyRole(utilisateur, "adherent") });
            types.Add(new SelectListItem() { Text = "Encadrant", Value = "encadrant", Selected = isMyRole(utilisateur, "encadrant") });
            types.Add(new SelectListItem() { Text = "Administrateur", Value = "admin", Selected = isMyRole(utilisateur, "admin") });

            ViewBag.role_utilisateur = types;

            return View(utilisateur);
        }


        // POST: Utilisateurs/Edit/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "utilisateur_id,login,password,prenom,nom,adresse,telephone,role_utilisateur")] utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(utilisateur).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return View();
                }
            }

            var types = new List<SelectListItem>();
            types.Add(new SelectListItem() { Text = "Adhérent", Value = "adherent", Selected = isMyRole(utilisateur, "adherent") });
            types.Add(new SelectListItem() { Text = "Encadrant", Value = "encadrant", Selected = isMyRole(utilisateur, "encadrant") });
            types.Add(new SelectListItem() { Text = "Administrateur", Value = "admin", Selected = isMyRole(utilisateur, "admin") });

            ViewBag.role_utilisateur = types;

            return View(utilisateur);
        }

        // GET: Utilisateurs/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            utilisateur utilisateur = db.utilisateur.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(utilisateur);
        }

        // POST: Utilisateurs/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            utilisateur utilisateur = db.utilisateur.Find(id);
            db.utilisateur.Remove(utilisateur);
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

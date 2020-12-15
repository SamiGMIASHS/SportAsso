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
    public class DossiersController : Controller
    {

        private dbSportAssoEntities db = new dbSportAssoEntities();

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

        // GET: Dossiers
        public ActionResult Index()
        {
            return View(db.dossier.ToList());
        }

        // GET: Dossiers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dossier document = db.dossier.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Dossiers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dossiers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "dossier_id,utilisateur_id,certificat_medical,fiche_renseignement,assurance,paiement,dossier_complet")] dossier dossier)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.dossier.Add(dossier);
                    db.SaveChanges();
                    if(User.IsInRole("admin")) { 
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index","Disciplines");
                    }
                }
                catch
                {
                    return View();
                }
            }

            ViewBag.utilisateur_id = new SelectList(db.utilisateur, "utilisateur_id", "login", dossier.utilisateur_id);
            return View(dossier);
        }

        // GET: Dossiers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dossier dossier = db.dossier.Find(id);
            if (dossier == null)
            {
                return HttpNotFound();
            }
            ViewBag.utilisateur_id = new SelectList(db.utilisateur, "utilisateur_id", "login", dossier.utilisateur_id);
            return View(dossier);
        }

        // POST: Dossiers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "dossier_id,utilisateur_id,certificat_medical,fiche_renseignement,assurance,paiement,dossier_complet")] dossier dossier)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   
                    db.Entry(dossier).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                    
                }
                catch
                {
                    return View();
                }
            }
            ViewBag.utilisateur_id = new SelectList(db.utilisateur, "utilisateur_id", "login", dossier.utilisateur_id);
            return View(dossier);
        }

        // GET: Dossiers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dossier dossier = db.dossier.Find(id);
            if (dossier == null)
            {
                return HttpNotFound();
            }
            ViewBag.utilisateur_id = new SelectList(db.utilisateur, "utilisateur_id", "login", dossier.utilisateur_id);
            return View(dossier);
        }

        // POST: Dossiers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            dossier dossier = db.dossier.Find(id);
            db.dossier.Remove(dossier);
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

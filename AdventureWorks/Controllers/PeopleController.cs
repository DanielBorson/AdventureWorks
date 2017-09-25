using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdventureWorks.Models;
using System.Linq.Dynamic;

namespace AdventureWorks.Controllers
{
    public class PeopleController : Controller
    {
        private AdventureWorksEntities db = new AdventureWorksEntities();
        private int pageSize = 25;

        // GET: People
        /* public ActionResult Index()
        {
            var people = db.People.Where(p => p.PersonType == "EM").Include(p => p.Employee).Include(p => p.BusinessEntity).Include(p => p.Password);

            return View(people.ToList());
        } */

        public ActionResult Index(int page = 1, string sort = "LastName", string sortDir = "asc", string search = "")
        {
            int pageSize = 25;
            int totalRecords = 0;
            if (page < 1) page = 1;
            int skip = (page * pageSize) - pageSize;

            var ppl = GetPeople(search, sort, sortDir, skip, pageSize, out totalRecords);

            ViewBag.TotalRows = totalRecords;
            ViewBag.Search = search;

            /*if (Request.IsAjaxRequest())
                return PartialView("_PersonView", ppl.ToList());
            else */
                return View(ppl.ToList());

        }

        public List<Person> GetPeople(string search, string sort, string sortDir, int skip, int pageSize, out int totalRecords)
        {
            using (AdventureWorksEntities ent = new AdventureWorksEntities())
            {
                var ppl = ent.People.Where(p => p.FirstName.Contains(search) || p.LastName.Contains(search));
                totalRecords = ppl.Count();

                ppl = ppl.OrderBy(sort + " " + sortDir);
                if(pageSize > 0)
                {
                    ppl = ppl.Skip(skip).Take(pageSize);
                }
                return ppl.ToList();
            }
           
        }

        // GET: People/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // GET: People/Create
        public ActionResult Create()
        {
            ViewBag.BusinessEntityID = new SelectList(db.Employees, "BusinessEntityID", "NationalIDNumber");
            ViewBag.BusinessEntityID = new SelectList(db.BusinessEntities, "BusinessEntityID", "BusinessEntityID");
            ViewBag.BusinessEntityID = new SelectList(db.Passwords, "BusinessEntityID", "PasswordHash");
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BusinessEntityID,PersonType,NameStyle,Title,FirstName,MiddleName,LastName,Suffix,EmailPromotion,AdditionalContactInfo,Demographics,rowguid,ModifiedDate")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.People.Add(person);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BusinessEntityID = new SelectList(db.Employees, "BusinessEntityID", "NationalIDNumber", person.BusinessEntityID);
            ViewBag.BusinessEntityID = new SelectList(db.BusinessEntities, "BusinessEntityID", "BusinessEntityID", person.BusinessEntityID);
            ViewBag.BusinessEntityID = new SelectList(db.Passwords, "BusinessEntityID", "PasswordHash", person.BusinessEntityID);
            return View(person);
        }

        // GET: People/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            ViewBag.BusinessEntityID = new SelectList(db.Employees, "BusinessEntityID", "NationalIDNumber", person.BusinessEntityID);
            ViewBag.BusinessEntityID = new SelectList(db.BusinessEntities, "BusinessEntityID", "BusinessEntityID", person.BusinessEntityID);
            ViewBag.BusinessEntityID = new SelectList(db.Passwords, "BusinessEntityID", "PasswordHash", person.BusinessEntityID);
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BusinessEntityID,PersonType,NameStyle,Title,FirstName,MiddleName,LastName,Suffix,EmailPromotion,AdditionalContactInfo,Demographics,rowguid,ModifiedDate")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BusinessEntityID = new SelectList(db.Employees, "BusinessEntityID", "NationalIDNumber", person.BusinessEntityID);
            ViewBag.BusinessEntityID = new SelectList(db.BusinessEntities, "BusinessEntityID", "BusinessEntityID", person.BusinessEntityID);
            ViewBag.BusinessEntityID = new SelectList(db.Passwords, "BusinessEntityID", "PasswordHash", person.BusinessEntityID);
            return View(person);
        }

        [HttpPost]
        public ActionResult Update(Person person)
        {
            if (ModelState.IsValid)
            {
                Person pers = db.People.Single(p => p.BusinessEntityID == person.BusinessEntityID);
                pers.FirstName = person.FirstName;
                pers.MiddleName = person.MiddleName;
                pers.LastName = person.LastName;
                pers.Suffix = person.Suffix;
                pers.EmailPromotion = person.EmailPromotion;
                pers.ModifiedDate = DateTime.Now;
                db.Entry(pers).State = EntityState.Modified;
                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        public PartialViewResult Search(string searchString)
        {
            List<Person> personListCollection = new List<Person>();
            personListCollection = db.People.Where(p => p.PersonType == "EM").Include(p => p.Employee).Include(p => p.BusinessEntity).Include(p => p.Password).ToList();
            
            if (Request.IsAjaxRequest())
            {
                if (!string.IsNullOrEmpty(searchString) && personListCollection != null && personListCollection.Count > 0)
                {
                    var searchedlist = (from list in personListCollection where list.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 || list.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 select list).ToList();
                    return PartialView("_PersonView", searchedlist);
                }
                else
                {
                    return PartialView("_PersonView", personListCollection);
                }
            }
            else
            {
                return PartialView("_PersonView", personListCollection);
            }
        }

        // GET: People/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Person person = db.People.Find(id);
            db.People.Remove(person);
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

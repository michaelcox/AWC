using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;

namespace AWC.WebUI.Controllers
{
    public class ClientsController : Controller
    {
        private IRepository _repo;

        public ClientsController(IRepository repository)
        {
            _repo = repository;
        }

        //
        // GET: /Clients/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Clients/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Clients/Create

        [HttpPost]
        public ActionResult Create(Client client)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repo.Add(client);
                    _repo.CommitChanges();
                    return RedirectToAction("Create");
                }
                return View(client);
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Clients/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Clients/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Clients/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Clients/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Castle.Core.Internal;
using WebApp2021.BL;
using WebApp2021.DAL;
using WebApp2021.Models;
using WebApp2021.Utils;
using Microsoft.AspNetCore.Mvc;

namespace WebApp2021.Controllers
{
    public class StoreController : Controller
    {
        private readonly StoreBL storeBL;
        private readonly AppDbContext db;

        public StoreController(AppDbContext db)
        {
            this.storeBL = new StoreBL(db);
            this.db = db;
        }

        // GET: Store
        public ActionResult Index([FromQuery] StoreQuery query)
        {
            Func<Store, bool> namePredicate = s => true;
            Func<Store, bool> cityPredicate = s => true;
            Func<Store, bool> tagsPredicate = s => true;

            if (query.name != null)
                namePredicate = s => s.Name == query.name;
            if (query.city != null)
                cityPredicate = s => s.City == query.city;
            if (!query.tags.IsNullOrEmpty())
                tagsPredicate = s => s.Tags.Select(st => st.TagId).Any(id => query.tags.Contains(id));

            var stores = this.storeBL.GetStores(s => namePredicate(s) && cityPredicate(s) && tagsPredicate(s));
            ViewData["Tags"] = this.storeBL.GetTags();

            return View(stores);
        }

        // GET: Store/Create
        public ActionResult Create()
        {
            UserState state = UserController.GetPermission(this.HttpContext.Session);

            if (state == UserState.NotLoggedIn)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));

            if (state != UserState.Manager)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));

            ViewData["Tags"] = this.storeBL.GetTags();
            ViewData["checkedTags"] = new HashSet<int>();
            ViewData["SubmitLabel"] = "Create";

            return View();
        }

        // POST: Store/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [IfModelIsInvalid]
        public async Task<ActionResult> Create([FromForm] Store store, string[] tags)
        {
            UserState state = UserController.GetPermission(this.HttpContext.Session);

            if (state == UserState.NotLoggedIn)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));

            if (state != UserState.Manager)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (await this.storeBL.AddStore(store) > 0)
                    {
                        foreach (var tagId in tags)
                        {
                            store.Tags.Add(new StoreTag
                            {
                                StoreId = store.Id,
                                TagId = int.Parse(tagId),
                            });
                        }
                        if (await this.storeBL.UpdateStore(store) > 0)
                        {
                            transaction.Commit();
                            return RedirectToAction("Index");
                        }
                        else
                            transaction.Rollback();
                    }
                    return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.InternalServerError));
                }
                catch
                {
                    transaction.Rollback();
                    return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.InternalServerError));
                }
            }
        }

        // GET: Store/Edit/:id
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.BadRequest));

            UserState state = UserController.GetPermission(this.HttpContext.Session);

            if (state == UserState.NotLoggedIn)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));

            if (state != UserState.Manager)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));

            Store store = await this.storeBL.GetStoreById(id.Value);

            if (store == null)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.NotFound));

            ViewData["Tags"] = this.storeBL.GetTags();
            ViewData["checkedTags"] = store.Tags.Select(st => st.TagId).ToHashSet();
            ViewData["SubmitLabel"] = "Edit";

            return View(store);
        }

        // POST: Store/Edit/:id
        [HttpPost]
        [ValidateAntiForgeryToken]
        [IfModelIsInvalid]
        public async Task<ActionResult> Edit([FromForm] Store store, string[] tags)
        {
            UserState state = UserController.GetPermission(this.HttpContext.Session);

            if (state == UserState.NotLoggedIn)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));

            if (state != UserState.Manager)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));

            Store oldStore = await this.storeBL.GetStoreById(store.Id);
            oldStore.Tags.Clear();

            foreach (var tagId in tags)
            {
                oldStore.Tags.Add(new StoreTag
                {
                    StoreId = store.Id,
                    TagId = int.Parse(tagId),
                });
            }

            foreach (PropertyInfo prop in store.GetType().GetProperties())
            {
                if (Attribute.IsDefined(prop, typeof(IsUpdatableAttribute)) && prop.SetMethod != null)
                    prop.SetValue(oldStore, prop.GetValue(store));
            }

            if (await this.storeBL.UpdateStore(oldStore) > 0)
            {
                return RedirectToAction("Index");
            }

            return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.InternalServerError));
        }

        // POST: Store/Delete/:id
        [HttpPost]
        public async Task<JsonResult> Delete([FromForm]int? id)
        {
            if (id == null)
                return Json(string.Format("/Error/?code={0}", (int)HttpStatusCode.BadRequest));

            Store store = await this.storeBL.GetStoreById(id.Value);

            if (store == null)
                return Json(string.Format("/Error/?code={0}", (int)HttpStatusCode.NotFound));

            if (!UserController.IsManager(this.HttpContext.Session))
                return Json(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));

            if (await this.storeBL.DeleteStore(id.Value) > 0)
                return Json(this.Url.Action("Index"));

            return Json(string.Format("/Error/?code={0}", (int)HttpStatusCode.InternalServerError));
        }
    }
    public class StoreQuery
    {
        public string name { get; set; }
        public string city { get; set; }

        public int[] tags { get; set; }
    }
}

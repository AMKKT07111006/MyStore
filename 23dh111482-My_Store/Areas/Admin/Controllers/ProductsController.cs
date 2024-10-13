using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _23dh111482_My_Store.Models;
using _23dh111482_My_Store.Models.ViewModel;

namespace _23dh111482_My_Store.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private My_StoreEntities db = new My_StoreEntities();

        // GET: Admin/Products
        public ActionResult Index(string SearchTerm, decimal? MinPrice, decimal? MaxPrice
            ,string SortOrder, int? Page)
        {
            var model = new ProductSearchVM();
            var products = db.Products.AsQueryable();

            //Tìm kiếm dựa trên từ khóa
            if(!string.IsNullOrEmpty(SearchTerm) )
            {
                model.SearchTerm = SearchTerm;
                products = products.Where(p => p.ProductName.Contains(SearchTerm)
                                    || p.ProductDescription.Contains(SearchTerm) 
                                    || p.Category.CategoryName.Contains(SearchTerm));
            }    

            //Tìm kiếm dựa trên giá tối thiểu
            if (MinPrice.HasValue)
            {
                model.MinPrice = MinPrice.Value;
                products = products.Where(p => p.ProductPrice <= MaxPrice.Value);
            }

            //Áp dụng sắp xếp dựa trên lựa chọn người dùng
            switch(SortOrder)
            {
                case "name_asc": products = products.OrderBy(p => p.ProductName); break;
                case "name_desc": products = products.OrderByDescending(p => p.ProductName); break;
                case "price_asc": products = products.OrderBy(p => p.ProductPrice); break;
                case "price_desc": products = products.OrderByDescending(p => p.ProductPrice); break;
                default: //mặc định xếp theo tăng dần
                    products = products.OrderBy(p => p.ProductName); break;
            }

            model.Products = products.ToList();
            return View(model);
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductPrice,ProductImage,ProductDescription")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductPrice,ProductImage,ProductDescription")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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


using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        //ApplicationDbContext db = new ApplicationDbContext(); .Net core application
        // GET: CategoryController
        private readonly IUnitOfWork _unitofwork;
        public CategoryController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        public ActionResult Index()
        {

            List<Category> objCategoryList = _unitofwork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            try
            {
                if (obj.Name == obj.DisplayOrder.ToString())
                {
                    ModelState.AddModelError("Name", "The Display cannot exactly match the Name");
                }

                if (ModelState.IsValid)
                {
                    _unitofwork.Category.Add(obj);
                    _unitofwork.Save();
                    TempData["success"] = "Category created successfully";
                    return RedirectToAction(nameof(Index));
                }
                return View();

            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryController/Edit/5   
        public ActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitofwork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Category obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _unitofwork.Category.Update(obj);
                    _unitofwork.Save();
                    TempData["success"] = "Category updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitofwork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Category? obj = _unitofwork.Category.Get(u => u.Id == id);
                if (obj == null)
                {
                    return NotFound();
                }
                _unitofwork.Category.Remove(obj);
                _unitofwork.Save();
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

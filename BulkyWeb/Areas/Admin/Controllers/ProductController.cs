
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        //ApplicationDbContext db = new ApplicationDbContext(); .Net core application
        // GET: ProductController
        private readonly IUnitOfWork _unitofwork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitofwork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofwork = unitofwork;
            _webHostEnvironment = webHostEnvironment;
        }
        public ActionResult Index()
        {

            List<Product> objProductList = _unitofwork.Product.GetAll(includeProperties:"Category").ToList();

            return View(objProductList);
        }

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductController/Create
        public ActionResult Upsert(int? id)
        {
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = _unitofwork.Category
             .GetAll().Select(u => new SelectListItem
             {
                 Text = u.Name,
                 Value = u.Id.ToString()
             }),
                Product = new Product()
            };
            if(id==null || id == 0)
            {
                //update 
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitofwork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
            
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = Path.Combine(wwwRootPath, @"images\product");

                        if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                        {
                            //delete the old image
                            var oldImagePath =
                                Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        productVM.Product.ImageUrl = @"\images\product\" + fileName;
                    }

                    if (productVM.Product.Id == 0)
                    {
                        _unitofwork.Product.Add(productVM.Product);
                    }
                    else
                    {
                        _unitofwork.Product.Update(productVM.Product);
                    }
                    _unitofwork.Save();
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // To populate back the exception if the modelState is not Valid
                    productVM.CategoryList = _unitofwork.Category
                         .GetAll().Select(u => new SelectListItem
                         {
                             Text = u.Name,
                             Value = u.Id.ToString()
                         });

                    return View(productVM);
                }
            }
            catch
            {
                return View();
            }
        }
  
        //// GET: CategoryController/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == 0 || id == null)
        //    {
        //        return NotFound();
        //    }
        //    Product? productFromDb = _unitofwork.Product.Get(u => u.Id == id);
        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}

        //// POST: CategoryController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id)
        //{
        //    try
        //    {
        //        Product? obj = _unitofwork.Product.Get(u => u.Id == id);
        //        if (obj == null)
        //        {
        //            return NotFound();
        //        }
        //        _unitofwork.Product.Remove(obj);
        //        _unitofwork.Save();
        //        TempData["success"] = "Product deleted successfully";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitofwork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });

        }

        
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitofwork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
               
                return Json(new { success = false, message = "Error while deleting" });
            }
            var oldImagePath =
                           Path.Combine(_webHostEnvironment.WebRootPath,
                           productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Exists(oldImagePath);
            }
            _unitofwork.Product.Remove(productToBeDeleted);
            _unitofwork.Save();

            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}


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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public CompanyController(IUnitOfWork unitofwork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofwork = unitofwork;
        }
        public ActionResult Index()
        {

            List<Company> objCompanyList = _unitofwork.Company.GetAll().ToList();

            return View(objCompanyList);
        }

        // GET: CompanyController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CompanyController/Create
        public ActionResult Upsert(int? id)
        {
            if(id==null || id == 0)
            {
                //update 
                return View(new Company());
            }
            else
            {
                //update
               Company company = _unitofwork.Company.Get(u => u.Id == id);
                return View(company);
            }
            
        }

        // POST: CompanyController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    if (company.Id == 0)
                    {
                        _unitofwork.Company.Add(company);
                    }
                    else
                    {
                        _unitofwork.Company.Update(company);
                    }
                    _unitofwork.Save();
                    TempData["success"] = "Company created successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
               return View(company);
                }
            }
            catch
            {
                return View();
            }
        }
  
      
        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitofwork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });

        }
    
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = _unitofwork.Company.Get(u => u.Id == id);    
            _unitofwork.Company.Remove(companyToBeDeleted);
            _unitofwork.Save();

            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}

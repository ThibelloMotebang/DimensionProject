using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dimension2.Data;
using Dimension2.Models;
using Microsoft.AspNetCore.Authorization;
using Dimension2.Areas;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Dimension2.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly DimensionDataContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public EmployeesController(DimensionDataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employees
        [Authorize(Roles = "Manager, Employee")]
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            #region Column Filters
            ViewData["CurrentSort"] = sortOrder;
            ViewData["EmpNrSortParam"] = sortOrder == "empnr_desc" ? "EmpNr" : "empnr_desc";
            ViewData["EmpAgeSortParam"] = sortOrder == "empage_desc" ? "EmpAge" : "empage_desc";
            ViewData["JobLevelSortParam"] = sortOrder == "joblevel_desc" ? "JobLevel" : "joblevel_desc";
            ViewData["JobRoleSortParam"] = sortOrder == "jobrole_desc" ? "JobRole" : "jobrole_desc";
            ViewData["DepSortParam"] = sortOrder == "dep_desc" ? "Dep" : "dep_desc";
            ViewData["GenderSortParam"] = sortOrder == "gender_desc" ? "Gender" : "gender_desc";

            ViewData["CurrentFilter"] = searchString;
            #endregion Column Filters

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            // Find the user via their email
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            // Get the role for the user
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Manager") || roles.Contains("Admin"))
            {

                var employees = from s in _context.Employee.Include(e => e.Edu).Include(e => e.Emp).Include(e => e.Job) select s;
                //Filter data based on search param
                if (!String.IsNullOrEmpty(searchString))
                {
                    employees = employees.Where(s => s.Job.Department.Contains(searchString)
                                           || s.Job.JobRole.Contains(searchString)
                                           || s.Emp.Gender.Contains(searchString)
                                           || s.Job.JobLevel.ToString() == (searchString)
                                           || s.Emp.Age.ToString() == (searchString)
                                           || s.EmployeeNumber.ToString() == (searchString));
                }

                //Individual Sorting for each column
                employees = sortOrder switch
                {
                    "EmpNr" => employees.OrderBy(s => s.EmployeeNumber),
                    "empnr_desc" => employees.OrderByDescending(s => s.EmployeeNumber),
                    "EmpAge" => employees.OrderBy(s => s.Emp.Age),
                    "empage_desc" => employees.OrderByDescending(s => s.Emp.Age),
                    "JobLevel" => employees.OrderBy(s => s.Job.JobLevel),
                    "joblevel_desc" => employees.OrderByDescending(s => s.Job.JobLevel),
                    "JobRole" => employees.OrderBy(s => s.Job.JobRole),
                    "jobrole_desc" => employees.OrderByDescending(s => s.Job.JobRole),
                    "Dep" => employees.OrderBy(s => s.Job.Department),
                    "dep_desc" => employees.OrderByDescending(s => s.Job.Department),
                    "Gender" => employees.OrderBy(s => s.Emp.Gender),
                    "gender_desc" => employees.OrderByDescending(s => s.Emp.Gender),
                    _ => employees.OrderBy(s => s.EmployeeNumber),
                };
                int pageSize = 10;
                var pagedList = await PaginatedList<Employee>.CreateAsync(employees.AsNoTracking(), pageNumber ?? 1, pageSize);
                return View(pagedList);
            }
            else
            {
                var employees = from s in _context.Employee.Include(e => e.Edu).Include(e => e.Emp).Include(e => e.Job).Where(e => e.Emp.Email == User.Identity.Name) select s;

                int pageSize = 10;
                var pagedList = await PaginatedList<Employee>.CreateAsync(employees.AsNoTracking(), pageNumber ?? 1, pageSize);
                return View(pagedList);
            }

        }

        // GET: Employees/Details/5
        [Authorize(Roles = "Manager, Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            return View(await _context.GetbyIdAsync(id));
        }

        // GET: Employees/Create
        [Authorize(Roles = "Manager, Employee")]
        public IActionResult Create()
        {
            ViewData["EducationField"] = new SelectList(_context.EmployeeEducation.Select(x => x.EducationField).Distinct());
            ViewData["BusinessTravel"] = new SelectList(_context.JobInformation.Select(x => x.BusinessTravel).Distinct());
            ViewData["MaritalStatus"] = new SelectList(_context.EmployeeDetails.Select(x => x.MaritalStatus).Distinct());
            ViewData["JobRole"] = new SelectList(_context.JobInformation.Select(x => x.JobRole).Distinct());
            ViewData["Gender"] = new SelectList(_context.EmployeeDetails.Select(x => x.Gender).Distinct());
            ViewData["Department"] = new SelectList(_context.JobInformation.Select(x => x.Department).Distinct());

            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Employee")]
        public async Task<IActionResult> Create(Employee employee)
        {
            return RedirectToAction("Index", await _context.CreateAsync(employee));
        }

        // GET: Employees/Edit/5
        [Authorize(Roles = "Manager, Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            var employee = await _context.Employee
           .Include(e => e.Emp)
           .Include(e => e.Edu)
           .Include(e => e.EmpHistory)
           .Include(e => e.Job)
           .Include(e => e.MonthlyIncome)
           .Include(e => e.Satisfactions)
           .Include(e => e.EmpPerformance)
           .FirstOrDefaultAsync(m => m.EmployeeNumber == id);

            if (employee == null)
            {
                return NotFound();
            }

            ViewData["EducationField"] = new SelectList(_context.EmployeeEducation.Select(x => x.EducationField).Distinct());
            ViewData["BusinessTravel"] = new SelectList(_context.JobInformation.Select(x => x.BusinessTravel).Distinct());
            ViewData["MaritalStatus"] = new SelectList(_context.EmployeeDetails.Select(x => x.MaritalStatus).Distinct());
            ViewData["JobRole"] = new SelectList(_context.JobInformation.Select(x => x.JobRole).Distinct());
            ViewData["Gender"] = new SelectList(_context.EmployeeDetails.Select(x => x.Gender).Distinct());
            ViewData["Department"] = new SelectList(_context.JobInformation.Select(x => x.Department).Distinct());

            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Employee")]
        public async Task<IActionResult> Edit(int? id, Employee employee)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", await _context.UpdateAsync(id, employee));
            }

            return View(employee);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "Manager, Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Edu)
                .Include(e => e.Emp)
                .Include(e => e.EmpHistory)
                .Include(e => e.EmpPerformance)
                .Include(e => e.Job)
                .Include(e => e.MonthlyIncome)
                .Include(e => e.Satisfactions)
                .FirstOrDefaultAsync(m => m.EmployeeNumber == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return RedirectToAction("Index", await _context.DeleteAsync(id));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.EmployeeNumber == id);
        }
    }
}
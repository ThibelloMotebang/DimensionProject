using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dimension2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dimension2.Controllers
{
    [Authorize(Policy = "readonlypolicy")]
    public class GraphsController : Controller
    {
        private readonly DimensionDataContext _context;

        public GraphsController(DimensionDataContext context)
        {
            _context = context;
        }

        public IActionResult TimeGraphs()
        {
            #region PieChart
            int female = _context.Employee.Where(s => s.Emp.Gender == "female").Select(s => s).Count();
            int male = _context.Employee.Where(s => s.Emp.Gender == "male").Select(s => s).Count();

            ViewBag.FEM = female;
            ViewBag.MA = male;
            #endregion PieChart

            #region BarChart

            (int, int, int, int, int, int) ageTuple = (_context.Employee.Where(a => a.Emp.Age < 20).Select(a => a).Count(),
                                                        _context.Employee.Where(a => a.Emp.Age >= 20 && a.Emp.Age < 30).Select(a => a).Count(),
                                                        _context.Employee.Where(a => a.Emp.Age >= 30 && a.Emp.Age < 40).Select(a => a).Count(),
                                                        _context.Employee.Where(a => a.Emp.Age >= 40 && a.Emp.Age < 50).Select(a => a).Count(),
                                                        _context.Employee.Where(a => a.Emp.Age >= 50 && a.Emp.Age < 60).Select(a => a).Count(),
                                                        _context.Employee.Where(a => a.Emp.Age >= 60).Select(a => a).Count());

            ViewData["AGE"] = ageTuple;
            #endregion BarChart

            #region LineChart
            (int, int, int, int, int) yearAtCompTuple = (_context.Employee.Where(h => h.EmpHistory.YearsAtCompany <= 2).Select(h => h).Count(),
                                                         _context.Employee.Where(h => h.EmpHistory.YearsAtCompany > 2 && h.EmpHistory.YearsAtCompany <= 4).Select(h => h).Count(),
                                                         _context.Employee.Where(h => h.EmpHistory.YearsAtCompany > 4 && h.EmpHistory.YearsAtCompany <= 6).Select(h => h).Count(),
                                                         _context.Employee.Where(h => h.EmpHistory.YearsAtCompany > 6 && h.EmpHistory.YearsAtCompany <= 8).Select(h => h).Count(),
                                                         _context.Employee.Where(h => h.EmpHistory.YearsAtCompany >= 10).Select(h => h).Count());
            ViewData["COMPYEARS"] = yearAtCompTuple;

            (int, int, int, int, int) yearsCurRole = (_context.Employee.Where(h => h.EmpHistory.YearsInCurrentRole <= 2).Select(h => h).Count(),
                                                         _context.Employee.Where(h => h.EmpHistory.YearsInCurrentRole > 2 && h.EmpHistory.YearsInCurrentRole <= 4).Select(h => h).Count(),
                                                         _context.Employee.Where(h => h.EmpHistory.YearsInCurrentRole > 4 && h.EmpHistory.YearsInCurrentRole <= 6).Select(h => h).Count(),
                                                         _context.Employee.Where(h => h.EmpHistory.YearsInCurrentRole > 6 && h.EmpHistory.YearsInCurrentRole <= 8).Select(h => h).Count(),
                                                         _context.Employee.Where(h => h.EmpHistory.YearsInCurrentRole >= 10).Select(h => h).Count());
            ViewData["CURROLE"] = yearsCurRole;

            (int, int, int, int, int) yearSinceProm = (_context.Employee.Where(h => h.EmpHistory.YearsSinceLastPromotion <= 2).Select(h => h).Count(),
                                                         _context.Employee.Where(h => h.EmpHistory.YearsSinceLastPromotion > 2 && h.EmpHistory.YearsSinceLastPromotion <= 4).Select(h => h).Count(),
                                                         _context.Employee.Where(h => h.EmpHistory.YearsSinceLastPromotion > 4 && h.EmpHistory.YearsSinceLastPromotion <= 6).Select(h => h).Count(),
                                                         _context.Employee.Where(h => h.EmpHistory.YearsSinceLastPromotion > 6 && h.EmpHistory.YearsSinceLastPromotion <= 8).Select(h => h).Count(),
                                                         _context.Employee.Where(h => h.EmpHistory.YearsSinceLastPromotion >= 10).Select(h => h).Count());
            ViewData["PROM"] = yearSinceProm;

            (int, int, int, int, int) yearsCurMan = (_context.Employee.Where(h => h.EmpHistory.YearsWithCurrManager <= 2).Select(h => h).Count(),
                                                        _context.Employee.Where(h => h.EmpHistory.YearsWithCurrManager > 2 && h.EmpHistory.YearsWithCurrManager <= 4).Select(h => h).Count(),
                                                        _context.Employee.Where(h => h.EmpHistory.YearsWithCurrManager > 4 && h.EmpHistory.YearsWithCurrManager <= 6).Select(h => h).Count(),
                                                        _context.Employee.Where(h => h.EmpHistory.YearsWithCurrManager > 6 && h.EmpHistory.YearsWithCurrManager <= 8).Select(h => h).Count(),
                                                        _context.Employee.Where(h => h.EmpHistory.YearsWithCurrManager >= 10).Select(h => h).Count());
            ViewData["CURMAN"] = yearsCurMan;
            #endregion LineChart

            #region PolarAreaChart
            (int, int, int) maritalStatTuple = (_context.Employee.Where(m => m.Emp.MaritalStatus == "Single").Select(m => m).Count(),
                                                _context.Employee.Where(m => m.Emp.MaritalStatus == "Divorced").Select(m => m).Count(),
                                                _context.Employee.Where(m => m.Emp.MaritalStatus == "Married").Select(m => m).Count());

            ViewData["MARSTAT"] = maritalStatTuple;
            #endregion PolarAreaChart

            return View();
        }
    }
}
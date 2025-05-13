using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyManagerFinal.Core.Interfaces;
using CompanyManagerFinal.Core.Models;

namespace CompanyManagerFinal.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _companyService.GetAllCompnysAsync();
            return View(companies);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyModel company)
        {
            CompanyModel companyResult = new();
            if (!ModelState.IsValid)
                return View(company);
            else
                await _companyService.CreateCompanyAsync(company);
            return RedirectToAction("Index");
        }
        [HttpGet]
        [ActionName("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View("Create", company);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyModel company)
        {
            company.CreatedAt = DateTime.SpecifyKind(company.CreatedAt, DateTimeKind.Utc);
            if (ModelState.IsValid)
            {
                try
                {
                    await _companyService.UpdateCompanyAsync(company);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine(ex.Message);
                    StatusCode(500, "Ошибка сохранения в бд");
                }
            }
            return View(company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) //Удалить уже работает, вообще просто осталось подвязать действия к контроллеру, я думаю там и остальные формы если доделать, то всё можно через контроллер подключить
        {
            Console.WriteLine($"Попытка удалить компанию: {id}");

            var company = await _companyService.GetCompanyByIdAsync(id);
            await _companyService.DeleteCompanyByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return RedirectToAction("Index");
            }

            await _companyService.DeleByIdsAsync(ids);
            return RedirectToAction("Index");
        }
    }
}
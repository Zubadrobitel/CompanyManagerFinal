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
        public async Task<IActionResult> Create(int? id, bool isEdit)
        {
            if(isEdit)
            {
                if (id != null || id != 0)
                {
                    var company = await _companyService.GetCompanyByIdAsync(id);
                    if (company == null)
                    {
                        return NotFound();
                    }
                    return View(company);
                }
                else return View();
            }
            else
            {
                return View();
            }
        }

        //Оставил как пример на сколько меньше методы при делении по ответственности, любой джун посмотрит и скажет о это создание чиназес суда,
        //Он сможет даже не смотреть, как там всё устроено на уровне сервисов, или работы с бд, всё сделается за него
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(CompanyModel company)
        //{
        //    CompanyModel companyResult = new();
        //    if (!ModelState.IsValid)
        //        return View(company);
        //    else
        //        await _companyService.CreateCompanyAsync(company);
        //    return RedirectToAction("Index");
        //}

        //[HttpGet]
        //[ActionName("Edit")]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var company = await _companyService.GetCompanyByIdAsync(id);
        //    if (company == null)
        //    {
        //        return NotFound();
        //    }
        //    return View("Create", company);
        //}

        //Приготовься здесь много SOLID, дело в том, что объединённый код нарушает минимум 3 принципа SOLID(SRP, OCP, ISP).Дальше примеры + пояснение почему 
        //Если кратко:
        //Single Responsibility Principle(SRP)
        //Нарушение: Метод Save(объединённый Create/Edit) теперь имеет две причины для изменения:
        //Логика создания компании.
        //Логика редактирования компании.
        //Нарушение: Код не закрыт для изменений.
        //Добавление нового поля только для создания(например, Password) потребует модификации общего метода:
        //Interface Segregation Principle(ISP)
        //Нарушение: Клиенты(например, JavaScript) вынуждены зависеть от «жирного» API.
        //Проблема:
        //Единственный метод Save принимает все возможные поля(и для создания, и для редактирования).
        //Клиент для создания всё равно должен передавать Id = 0, даже если это не нужно.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyModel company, bool isEdit, int? id) //Большое количество аргументов, усложняет масштабируемость, понимание кода и время до выхода в продакшн, может привести к тому, что в метод может передаваться 100500 параметров и понять, что делает метод становится сложнее
        {
            if (!ModelState.IsValid)
            {
                return View("Create", company);
            }
            //Лишних проверок можно избежать при делении метода на методы по принципу SRP, не знаю какие паттерны разработки используются в организации может так и задуманно? Но такой код нарушает архетиктуру
            if (isEdit && (!id.HasValue || id.Value == 0))
            {
                ModelState.AddModelError("", "Для редактирования требуется ID");
                return View("Create", company);
            }
            //Противоречивые параметры, но проверка нужна для избижания крит ошибок
            if (isEdit && (!id.HasValue || id.Value == 0))
            {
                ModelState.AddModelError("", "Для редактирования требуется ID");
                return View("Create", company);
            }
            //Появляется бизнес логика на уровне контроллера/менеджера(избыточно и конфликтует с принципами SOLID)
            //Что делать, если для созданию и редактированию понадобятся разные поля? Много избыточной логики
            //(Например, Password требуется только при создании 
            //TODO меня тоже тут можно подловить SRP соблюдён нооо частично, плохое решение Представление Create.cshtml используется для двух сценариев. Это единственное нарушение SRP
            //Но проект небольшой и разделение приведёт к дублированию кода без реальной пользы, если я добавлю ещё одну страницу)
            if (isEdit) 
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
                        //Как обрабатывать ошибки, специфичные для каждого сценария?
                        //(При создании: «Компания с таким именем уже существует». При редактировании: «Нельзя изменить архивную запись»)
                        //Как тестировать метод, где смешаны две логики?
                        //(В 2 раза больше тест - кейсов)
                        //Затраты ресурсов
                        //(Хотя я и сам считаю, что в наше время об этом смысла говорить нет, сейчас достаточно мощные пк/сервера, но при написании больших приложений это критично)
                    }
                }
                return View(company);
            }
            else 
            {
                CompanyModel companyResult = new();
                if (!ModelState.IsValid)
                    return View(company);
                else
                    await _companyService.CreateCompanyAsync(company);
                return RedirectToAction("Index");
            }
        }
        //Сделал как заказывали, я может не туда лезу но было написанно по принципам проектирования SOLID. Просто ты сам говорил, если знаешь, что поправить не молчи,
        //Ребятам будет легче понять, как работают методы, если будет их чёткое разделения по их ответственности
        //Я взялся просто серьёзно и по своему опыту расписал, мне за это по шапке настучали когда-то, было больно)
        //Дисклеймер я ни на что не притендую, но код будет читабельным и лёгким для понимания даже стажёру, команда же должна легко ориентироваться, в том что пишет каждый

        //[HttpPost, ActionName("Edit")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(CompanyModel company)
        //{
        //    company.CreatedAt = DateTime.SpecifyKind(company.CreatedAt, DateTimeKind.Utc);
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await _companyService.UpdateCompanyAsync(company);
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (DbUpdateException ex) ну тут и я избыточное выкинул, по идеи проверка в сервисе и выкидывать в логер
        //        {
        //            Console.WriteLine(ex.Message);
        //            StatusCode(500, "Ошибка сохранения в бд");
        //        }
        //    }
        //    return View(company);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
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
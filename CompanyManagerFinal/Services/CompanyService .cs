using CompanyManagerFinal.Core.Interfaces;
using CompanyManagerFinal.Core.Models;

namespace CompanyManagerFinal.Services
{
    public class CompanyService : ICompanyService
    {

        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository repository)
        {
            _companyRepository = repository;
        }
        public async Task<CompanyModel?> GetCompanyByIdAsync(int? id)
        {
            var result = await _companyRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(nameof(id));
            return result;
        }
        public async Task<IReadOnlyList<CompanyModel>> GetAllCompnysAsync()=> await _companyRepository.GetAllAsync();
        public async Task CreateCompanyAsync(CompanyModel company)
        {
            try
            {
                company.CreatedAt = DateTime.UtcNow;
                await _companyRepository.CreateAsync(company);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task UpdateCompanyAsync(CompanyModel company)
        {
            try
            {
                await _companyRepository.UpdateAsync(company);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            };
        }
        public async Task DeleteCompanyByIdAsync(int id)
        {
            try
            {
                await _companyRepository.DeleteByIdAsync(id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task DeleByIdsAsync(List<int> ids) => await _companyRepository.DeleteMultipleAsync(ids);
    }
}
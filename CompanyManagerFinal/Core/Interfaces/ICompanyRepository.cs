using CompanyManagerFinal.Core.Models;

namespace CompanyManagerFinal.Core.Interfaces
{
    public interface ICompanyRepository
    {
        Task<CompanyModel?> GetByIdAsync(int? id);
        Task<IReadOnlyList<CompanyModel>> GetAllAsync();
        Task CreateAsync(CompanyModel company);
        Task UpdateAsync(CompanyModel company);
        Task DeleteByIdAsync(int id);
        Task<CompanyModel?> FindCompanyAsync(int id);
        Task DeleteMultipleAsync(List<int> ids);
    }
}
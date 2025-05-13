using CompanyManagerFinal.Core.Interfaces;
using CompanyManagerFinal.Core.Models;
using CompanyManagerFinal.Infastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagerFinal.Infastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DBContext _dbContext;

        public CompanyRepository(DBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<CompanyModel?> GetByIdAsync(int? id)
        {
            return await _dbContext.Companies.Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<CompanyModel>> GetAllAsync()
        {
            return await _dbContext.Companies.ToListAsync();
        }

        public async Task CreateAsync(CompanyModel company)
        {
            try
            {
                await _dbContext.Companies.AddAsync(company);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }             
        }

        public async Task<CompanyModel?>FindCompanyAsync(int id) => await _dbContext.FindAsync<CompanyModel>(id);

        public async Task UpdateAsync(CompanyModel company)
        {
            _dbContext.Companies.Update(company);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var company = await _dbContext.Companies.FindAsync(id);
            if(company != null)
            {
                _dbContext.Companies.Remove(company);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteMultipleAsync(List<int> ids)
        {
            var companyesToDelete = await _dbContext.Companies.Where(c => ids.Contains(c.Id)).ToListAsync();
            if(companyesToDelete.Any())
            {
                _dbContext.Companies.RemoveRange(companyesToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}

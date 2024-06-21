using Domain.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public interface IRepository<T, TUpdate> where TUpdate : class
    {
        public Task<T> CreateAsync(T entity);
        public Task<T> UpdateAsync(T entity);
        public Task<T> PartiallyUpdateAsync(int id, JsonPatchDocument<TUpdate> patchDocument);
        public Task<T> DeleteAsync(int id);
        public Task<(List<T>, PaginationMetaData)> GetAllAsync(int pageNumber, int pageSize, string keyword = null);
        public Task<T> GetByIdAsync(int id);
    }
}

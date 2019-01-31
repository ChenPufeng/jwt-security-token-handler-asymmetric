using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jwt_security_token_handler_asymmetric.Abstraction
{
    public interface IRepository<TModel, TModelId> where TModel: class
    {
         Task AddAsync(TModel obj);
         Task UpdateAsync(TModel obj);
         Task DeleteAsync(TModel obj);
         Task<TModel> GetAsync(Expression<Func<TModel, bool>> predicate, params string[] expands);
         Task<IEnumerable<TModel>> GetAllAsync(Expression<Func<TModel, bool>> predicate, params string[] expands);
    }
}
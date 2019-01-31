using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using jwt_security_token_handler_asymmetric.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace jwt_security_token_handler_asymmetric.Repositores
{
  public abstract class RepositoryBase<TModel, TModelKey> : IRepository<TModel, TModelKey> where TModel : class
  {
    private readonly DbContext context;
    public RepositoryBase(DbContext context)
    {
      this.context = context;
    }

    public async Task AddAsync(TModel model)
    {
      Detach(GetDomainIdPredicate(GetDomainId(model)));
      await context.Set<TModel>().AddAsync(model);
      await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TModel model)
    {
      Detach(GetDomainIdPredicate(GetDomainId(model)));
      context.Set<TModel>().Remove(model);
      await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TModel model)
    {
      Detach(GetDomainIdPredicate(GetDomainId(model)));
      context.Set<TModel>().Update(model);
      context.Set<TModel>().Attach(model);
      await context.SaveChangesAsync();
    }

    public Task<TModel> GetAsync(Expression<Func<TModel, bool>> predicate, params string[] expands)
    {
        var query = context.Set<TModel>().AsQueryable();
        InflateExpands(expands, ref query);
        return query.SingleOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<TModel>> GetAllAsync(Expression<Func<TModel, bool>> predicate, params string[] expands)
    {
        var query = await GetQueryableAsync(expands);

        if (predicate != null)
            return query.Where(predicate);
        else
            return query;
    }

    protected abstract Func<TModel, TModelKey> GetDomainId { get; }
    protected abstract Func<TModelKey, Expression<Func<TModel, bool>>> GetDomainIdPredicate { get; }

    private void Detach(Expression<Func<TModel, bool>> predicate)
    {
        var current = context.Set<TModel>().Local.FirstOrDefault(predicate.Compile());
        if (current != null)
            context.Entry(current).State = EntityState.Detached;
    }

    private Task<IQueryable<TModel>> GetQueryableAsync(params string[] expands)
    {
        return Task.Run(() =>
        {
            var query = context.Set<TModel>().AsQueryable();
            InflateExpands(expands, ref query);
            return query;
        });
    }
    
    private static void InflateExpands<T>(IEnumerable<string> expands, ref IQueryable<T> query)
        where T: class
    {
        if (expands?.Any() != true)
            return;

        foreach (var expand in expands)
        {
            var parts = expand.Split('.');
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
            InflateEacPart(ref query, parts, properties, expand);
        }
    }

    private static void InflateEacPart<T>(ref IQueryable<T> query, string[] parts, PropertyInfo[] properties, string expand)
        where T: class
    {
        for (var i =0;i < parts.Length; i++)
        {
            var property = properties.FirstOrDefault(x => string.Compare(x.Name, parts[i], true) == 0);
            if (property == null)
                break;
            
            if (i == parts.Length -1 &&
                property.PropertyType.IsClass &&
                property.PropertyType != typeof(string))
            {
                query = query.Include(expand);
                break;
            }

            properties = property.PropertyType
                .GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
        }
    }
  }
}
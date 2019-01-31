using System;
using System.Linq.Expressions;
using jwt_security_token_handler_asymmetric.Models;
using Microsoft.EntityFrameworkCore;

namespace jwt_security_token_handler_asymmetric.Repositores
{
  public class ClientRepository : RepositoryBase<Client, Guid>
  {
    public ClientRepository(DbContext context) : base(context)
    {
    }

    protected override Func<Client, Guid> GetDomainId => domain => domain.Id;

    protected override Func<Guid, Expression<Func<Client, bool>>> GetDomainIdPredicate => 
        filter => client =>
            client.Id.Equals(filter);
  }
}
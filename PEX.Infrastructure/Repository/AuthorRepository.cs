using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PEX.Domain.Model;
using PEX.Infrastructure.Core;
using PEX.Infrastructure.Core.Contracts;
using PEX.Infrastructure.Repository.Contracts;

namespace PEX.Infrastructure.Repository;
public class AuthorRepository : BaseEFRepository<Author, int>, IAuthorRepository
{
    public AuthorRepository(IUnitOfWorks unitOfWorks, IServiceProvider serviceProvider) : base(unitOfWorks, serviceProvider)
    {
    }

    public override IQueryable<Author> GetFullDbSet()
    {
        return GetDbSet();

    }
}

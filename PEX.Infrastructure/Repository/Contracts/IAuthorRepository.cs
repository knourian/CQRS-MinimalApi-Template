using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PEX.Domain.Model;
using PEX.Infrastructure.Core.Contracts;

namespace PEX.Infrastructure.Repository.Contracts;
public interface IAuthorRepository : IBaseRepository<Author, int>
{

}

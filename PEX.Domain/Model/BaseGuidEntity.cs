using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEX.Domain.Model;

/// <summary>
/// a BaseEntity Class which has Id of int type
/// </summary>
public abstract class BaseGuidEntity : BaseEntity<Guid>
{

}

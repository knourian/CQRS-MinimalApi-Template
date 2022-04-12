using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEX.Domain.Model;
public class Author : BaseIntEntity
{
	public string FirstName { get; set; } = default!;
	public string LastName { get; set; } = default!;

	public string FullName => FirstName + " " + LastName;
	public DateTime DateOfBirth { get; set; }
	public string? Bio { get; set; }
}

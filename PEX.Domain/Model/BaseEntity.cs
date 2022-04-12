using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEX.Domain.Model;

/// <summary>
/// Base Entity Classes
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseEntity<T>
{
	/// <summary>
	/// Entity Id
	/// </summary>
	/// <value>entity unique id</value>
	[Key]
	[Required]
	public T Id { get; set; } = default(T)!;
}

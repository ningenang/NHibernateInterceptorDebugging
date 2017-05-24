using System;

namespace DAL.DTO.Classes
{
	/// <summary>
	/// This class represents a single data transfer object. Common extensions 
	/// to DTO's should either be put in this class or in extension methods
	/// for this class.
	/// </summary>
	/// <typeparam name="T">The DTO type</typeparam>
	[Serializable]
	public abstract class DTO<T> where T : class, new()
	{
	}
}

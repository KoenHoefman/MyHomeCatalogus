namespace MyHomeCatalogus.Shared.Exceptions
{
	[Serializable]
	public class UniqueConstraintException : Exception
	{
		public List<(string PropertyName, string ErrorMessage)> ValidationErrors { get; } = new();

		public UniqueConstraintException(
			string message,
			List<(string PropertyName, string ErrorMessage)> errors)
			: base(message)
		{
			ValidationErrors = errors;
		}

		public UniqueConstraintException(string propertyName, string errorMessage)
			: base(errorMessage)
		{
			ValidationErrors.Add((propertyName, errorMessage));
		}

		public UniqueConstraintException()
		{
		}

	}
}

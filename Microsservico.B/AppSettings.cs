namespace Microsservico.B
{
	/// <summary>
	/// Essa classe será uma API das variáveis de ambientes citadas no arquivo 'appSettings.json'
	/// Essa classe utilizará do padrão IOptions
	/// </summary>
	public static class AppSettings
	{
		public class RabbitMQ
		{
			public const string Identifier = "RabbitMQ";

			public string Host { get; set; } = String.Empty;

			public string User { get; set; } = String.Empty;

			public string Password { get; set; } = String.Empty;
		}
	}
}

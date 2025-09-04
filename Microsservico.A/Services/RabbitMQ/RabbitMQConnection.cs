using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Microsservico.A.Services.RabbitMQ
{
	/// <summary>
	/// Classe responsável por realizar a conexão com o RabbitMQ
	/// Não terá envolvimento com a abertura de canais ou envio/recebimento de mensagens
	/// Essa classe deve ser injetada como Singleton, ou seja, será instanciada apenas uma vez. Isso se deve ao fato de que a conexão com RabbitMQ é pesada/custosa
	/// por isso recomenda-se a não criar uma conexão a cada request.
	/// Este motivo explicado acima, foi o que motivou termos essa classe separada da classe <seealso cref="RabbitMQPublisher"/>, pois essa última cria uma instância a cada request.
	/// </summary>
	public class RabbitMQConnection : IDisposable
	{
		private readonly ILogger<RabbitMQConnection> _logger;
		private readonly AppSettings.RabbitMQ _settings;
		private IConnection? _connection = null;

		/// <summary>
		/// Construtor
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="options"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public RabbitMQConnection(ILogger<RabbitMQConnection> logger, IOptions<AppSettings.RabbitMQ> options)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_settings = options.Value ?? throw new ArgumentNullException(nameof(options));
		}

		/// <summary>
		/// Responsável por enviar a conexão aos interessados
		/// Este método se torna necessário, pois para criar os channels é necessário a utilização do objeto conexão.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
		{
			await SetConnectionAsync(cancellationToken);

			return _connection ?? throw new ArgumentNullException(nameof(_connection));
		}

		/// <summary>
		/// Responsável por realizar a criação da conexão com o servidor do RabbitMQ
		/// A criação da conexão não está no construtor da classe para podermos utilizar o método assíncrono 'CreateConnectionAsync',
		/// o qual não seria possível devido sua natureza assíncrona.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async Task SetConnectionAsync(CancellationToken cancellationToken)
		{
			if (_connection != null)
				return;

			_logger.LogInformation("{DateTime} - Criando Conexão com RabbitMQ", DateTime.UtcNow);

			ConnectionFactory factory = new()
			{
				Uri = new Uri(_settings.ConnectionString)
			};

			_connection = await factory.CreateConnectionAsync(cancellationToken);
		}

		/// <summary>
		/// Este método será importante para podermos realizar o fechamento da conexão
		/// </summary>
		public void Dispose()
		{
			_connection?.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}

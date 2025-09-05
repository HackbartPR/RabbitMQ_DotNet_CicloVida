using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Microsservico.B.Services.RabbitMQ
{
	/// <summary>
	/// Classe responsável por realizar a leitura das mensagens.
	/// Essa classe será quem realizará a abertura e fechamento dos channels.
	/// Como teremos um BackgroundService utilizando essa classe, ela será instanciada como Singleton, portanto não precisaríamos separar da classe Connection, mas
	/// para manter o padrão, foi separada também.
	/// </summary>
	public class RabbitMQListener : IBrokerListener
	{
		private readonly ILogger<RabbitMQListener> _logger;
		private readonly RabbitMQConnection _connection;
		private IChannel? _channel;

		public RabbitMQListener(ILogger<RabbitMQListener> logger, RabbitMQConnection connection)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_connection = connection ?? throw new ArgumentNullException(nameof(connection));
		}

		/// <summary>
		/// Responsável por receber as mensagens que estão na fila configurada.
		/// Para evitar de ter as regras de negócio dentro de um service, será passado um parâmetro do tipo Func (function/método)
		/// Com esse parametro conseguimos ter a regra de negócio dentro da classe responsável e o service tem apenas a responsábilidade de rodá-la ao 
		/// receber uma mensagem
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task ReceiveMessageAsync(Func<string, CancellationToken, Task> callback, CancellationToken cancellationToken = default)
		{
			await SetChannelAsync(cancellationToken);

			var consumer = new AsyncEventingBasicConsumer(_channel!);

			consumer.ReceivedAsync += async (ch, ea) =>
			{
				string bodyJson = Encoding.UTF8.GetString(ea.Body.ToArray());

				_logger.LogInformation("{DateTime} - Mensagem Recebida: {Message}", DateTime.UtcNow, bodyJson);

				await callback(bodyJson, cancellationToken);

				await _channel!.BasicAckAsync(ea.DeliveryTag, false);
			};

			string consumerTag = await _channel!.BasicConsumeAsync("queue_example", false, consumer);
		}

		/// <summary>
		/// Responsável por realizar a criação de um channel com o servidor do RabbitMQ
		/// A criação do channel não está no construtor da classe para podermos utilizar o método assíncrono 'CreateChannelAsync',
		/// o qual não seria possível devido sua natureza assíncrona.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async Task SetChannelAsync(CancellationToken cancellationToken)
		{
			if (_channel != null)
				return;

			_logger.LogInformation("{DateTime} - Criando Canal com RabbitMQ", DateTime.UtcNow);

			IConnection connection = await _connection.GetConnectionAsync(cancellationToken);
			_channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

			await _channel.ExchangeDeclareAsync("exchange_example", "direct", true, noWait: true);
			await _channel.QueueDeclareAsync("queue_example", true, false, false, null, noWait: true);
		}

		/// <summary>
		/// Este método será importante para podermos realizar o fechamento do Channel aberto
		/// </summary>
		public void Dispose()
		{
			_channel?.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}

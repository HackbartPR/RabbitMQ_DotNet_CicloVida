using Contratos.Messages;
using Microsservico.B.Services;
using System.Text.Json;

namespace Microsservico.B.Listeners
{
	/// <summary>
	/// Este serviço ficará hospedado na minha API e será responsável por receber as mensagens.
	/// Em caso de ter mais de uma instancia do Microsservicos.B, teremos dois listeners deste abaixo.
	/// </summary>
	public class BrokerListener : BackgroundService
	{
		private readonly ILogger<BrokerListener> _logger;
		private readonly IBrokerListener _listener;

		public BrokerListener(ILogger<BrokerListener> logger, IBrokerListener listener)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_listener = listener ?? throw new ArgumentNullException(nameof(listener));

			_logger.LogInformation("{DateTime} - Iniciando BrokerListener ...", DateTime.UtcNow);
		}

		/// <summary>
		/// Classe que executará algo ao subir este background service.
		/// Não será necessário colocar um while para manter este método, pois o próprio método do Listener do RabbitMQ já realiza isso.
		/// </summary>
		/// <param name="stoppingToken"></param>
		/// <returns></returns>
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				await _listener.ReceiveMessageAsync(TaskToBeExecuted, stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError("{DateTime} - Erro Encontrado: {Erro}", DateTime.UtcNow, ex.Message);
			}
		}

		/// <summary>
		/// Somente um exemplo de uma tarefa que pode ser executada ao receber uma mensagem da fila
		/// </summary>
		/// <param name="message"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private async Task TaskToBeExecuted(string message, CancellationToken cancellationToken)
		{
			await Task.Delay(1000, cancellationToken);

			MessageExample messageExample = JsonSerializer.Deserialize<MessageExample>(message) ?? throw new Exception("Mensagem não reconhecida");

			_logger.LogInformation("{DateTime} - Tarefa {Id} Executada ...", DateTime.UtcNow, messageExample.Id);
		}
	}
}

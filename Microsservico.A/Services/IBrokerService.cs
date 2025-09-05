namespace Microsservico.A.Services
{
	/// <summary>
	/// Responsável por padronizar todos os Messages Brokers que possam surgir RabbitMQ, Kafka, SQS, Service Bus, ....
	/// </summary>
	public interface IBrokerService : IDisposable
	{
		/// <summary>
		/// Responsável por publicar uma mensagem/evento
		/// </summary>
		/// <param name="message"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task SendMessageAsync(string message, CancellationToken cancellationToken = default);
	}
}

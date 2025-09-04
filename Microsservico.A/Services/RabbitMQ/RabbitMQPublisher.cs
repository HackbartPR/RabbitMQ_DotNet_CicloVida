namespace Microsservico.A.Services.RabbitMQ
{
	/// <summary>
	/// Classe responsável por realizar a publicação das mensagens
	/// Essa classe será quem realizará a abertura e fechamento dos channels
	/// </summary>
	public class RabbitMQPublisher : IBrokerService
	{
		/// <summary>
		/// Responsável por publicar uma mensagem/evento
		/// </summary>
		/// <param name="message"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task SendMessageAsync(string message, CancellationToken? cancellationToken = null)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Este método será importante para podermos realizar o fechamento do Channel aberto
		/// </summary>
		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}

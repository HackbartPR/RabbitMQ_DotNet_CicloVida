namespace Microsservico.B.Services
{
	/// <summary>
	/// Responsável por padronizar todos os Messages Brokers que possam surgir RabbitMQ, Kafka, SQS, Service Bus, ....
	/// </summary>
	public interface IBrokerListener : IDisposable
	{
		/// <summary>
		/// Responsável por receber as mensagens que estão na fila configurada.
		/// Para evitar de ter as regras de negócio dentro de um service, será passado um parâmetro do tipo Func (function/método)
		/// Com esse parametro conseguimos ter a regra de negócio dentro da classe responsável e o service tem apenas a responsábilidade de rodá-la ao 
		/// receber uma mensagem
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task ReceiveMessageAsync(Func<string, CancellationToken, Task> callback, CancellationToken cancellationToken = default);
	}
}

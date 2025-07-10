namespace Aplicacao.DTOs
{
    /// <summary>
    /// Representa um pedido enviado do PDV para o Servidor.
    /// </summary>
    public class ComunicacaoRequest
    {
        /// <summary>
        /// O comando que o PDV quer executar.
        /// Ex: "OBTER_PRODUTOS", "REGISTRAR_VENDA"
        /// </summary>
        public string Comando { get; set; }

        /// <summary>
        /// Os dados da requisição, serializados como uma string JSON.
        /// </summary>
        public string PayloadJson { get; set; }
    }

    /// <summary>
    /// Representa uma resposta enviada do Servidor para o PDV.
    /// </summary>
    public class ComunicacaoResponse
    {
        public bool Sucesso { get; set; }
        public string MensagemErro { get; set; }

        /// <summary>
        /// Os dados da resposta, serializados como uma string JSON.
        /// </summary>
        public string PayloadJson { get; set; }
    }
}
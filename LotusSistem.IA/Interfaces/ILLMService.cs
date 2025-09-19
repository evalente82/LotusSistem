/// <summary>
/// Define o contrato para qualquer serviço de Modelo de Linguagem Grande (LLM).
/// Esta interface garante que possamos trocar o modelo de IA subjacente (ex: de Phi-3 para Llama 3)
/// sem alterar o resto do sistema, seguindo o Princípio da Inversão de Dependência.
/// </summary>
namespace LotusSistem.IA.Interfaces
{
    public interface ILLMService
    {
        /// <summary>
        /// Gera uma resposta de texto com base em um prompt fornecido.
        /// </summary>
        /// <param name="prompt">O texto de entrada para o modelo.</param>
        /// <returns>A resposta gerada pelo modelo.</returns>
        Task<string> GerarRespostaAsync(string prompt);
    }
}
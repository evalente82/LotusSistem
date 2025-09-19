using LLama;
using LLama.Common;
using LLama.Sampling; // Adicionado para o SamplingPipeline
using LotusSistem.IA.Interfaces;
using System.Text;

namespace LotusSistem.IA.Services
{
    /// <summary>
    /// Implementação concreta do ILLMService para o modelo Microsoft Phi-3-Mini.
    /// Esta classe é responsável por carregar o modelo, gerenciar seu contexto
    /// e executar a inferência para gerar respostas.
    /// </summary>
    public class Phi3MiniLLMService : ILLMService, IDisposable
    {
        private readonly LLamaWeights _modelWeights;
        private readonly StatelessExecutor _executor;

        public Phi3MiniLLMService()
        {
            var modelPath = @"C:\Users\endri\Desktop\Projetos\ModelosLLM\Phi-3-mini-4k-instruct-q4.gguf";

            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException(
                    "O arquivo do modelo GGUF não foi encontrado. Verifique o caminho em Phi3MiniLLMService.cs.", modelPath);
            }

            // MUDANÇA AQUI: Estes são os parâmetros que o construtor do executor precisa
            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 4096,
                GpuLayerCount = 0
            };

            _modelWeights = LLamaWeights.LoadFromFile(parameters);

            // MUDANÇA AQUI: Passamos os 'parameters' para o construtor do executor
            _executor = new StatelessExecutor(_modelWeights, parameters);
        }

        public async Task<string> GerarRespostaAsync(string prompt)
        {
            var responseBuilder = new StringBuilder();

            // MUDANÇA AQUI: Criamos um pipeline de amostragem para configurar a Temperature
            var pipeline = new DefaultSamplingPipeline
            {
                Temperature = 0.6f
            };

            var inferenceParams = new InferenceParams()
            {
                // E atribuímos o pipeline aqui
                SamplingPipeline = pipeline,
                AntiPrompts = new List<string> { "User:" }
            };

            await foreach (var token in _executor.InferAsync(prompt, inferenceParams))
            {
                responseBuilder.Append(token);
            }

            return responseBuilder.ToString();
        }

        public void Dispose()
        {
            _modelWeights?.Dispose();
        }
    }
}
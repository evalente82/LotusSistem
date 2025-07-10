namespace Dominio.Entidades
{
    // Esta classe é específica para o banco de dados do PDV.
    // Ela tem tudo que uma Venda normal tem, mais a flag de sincronização.
    public class VendaPdv : Venda
    {
        public bool PrecisaSincronizar { get; set; } = true;
    }
}
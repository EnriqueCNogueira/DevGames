public enum TipoItem
{
    KitMedico,
    PocaoAtaque,
    PocaoDefesa,
    Remedio
}

// Catalogo dos itens (GDD 6.2) com os valores decididos no Sprint 2.
// Usar um item gasta a acao do turno e nao carrega a stamina.
public static class Itens
{
    public const int QUANTIDADE = 4;

    public const float CURA_KIT_MEDICO = 0.5f;  // da vida maxima, so no ativo
    public const float BONUS_POCAO = 0.3f;      // +30% no dano causado / -30% no recebido
    public const int RODADAS_POCAO = 3;         // rodadas seguintes a do uso

    public const int ESTOQUE_MAXIMO = 5;        // por item, no perfil
    public const int POR_BATALHA = 3;           // itens que cada jogador leva para a luta

    public static readonly TipoItem[] TODOS =
        { TipoItem.KitMedico, TipoItem.PocaoAtaque, TipoItem.PocaoDefesa, TipoItem.Remedio };

    // o Remedio cura efeitos negativos, que so chegam no Sprint 3
    public static bool Disponivel(TipoItem tipo)
    {
        return tipo != TipoItem.Remedio;
    }

    public static int Preco(TipoItem tipo)
    {
        switch (tipo)
        {
            case TipoItem.KitMedico: return 40;
            case TipoItem.PocaoAtaque: return 30;
            case TipoItem.PocaoDefesa: return 30;
            default: return 30;
        }
    }

    public static string Nome(TipoItem tipo)
    {
        switch (tipo)
        {
            case TipoItem.KitMedico: return "Kit Médico";
            case TipoItem.PocaoAtaque: return "Poção de Ataque";
            case TipoItem.PocaoDefesa: return "Poção de Defesa";
            default: return "Remédio";
        }
    }

    public static string Descricao(TipoItem tipo)
    {
        switch (tipo)
        {
            case TipoItem.KitMedico: return "Cura metade da vida do personagem na arena.";
            case TipoItem.PocaoAtaque: return "Mais 30% de dano causado por 3 rodadas.";
            case TipoItem.PocaoDefesa: return "Menos 30% de dano recebido por 3 rodadas.";
            default: return "Remove efeitos negativos.";
        }
    }
}

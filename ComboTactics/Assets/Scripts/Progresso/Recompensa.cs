// Moedas ao fim de uma batalha (GDD 6.1). Sair pelo botao Menu no meio da luta nao rende nada.
public static class Recompensa
{
    public const int VITORIA = 100;
    public const int DERROTA = 50;

    // No PvP o perfil e um so e sempre ha um vencedor: rende a vitoria.
    // A derrota passa a valer quando houver IA e Historia.
    public const int PARTIDA_PVP = VITORIA;
}

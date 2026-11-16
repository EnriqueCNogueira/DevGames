// Aviso de efetividade de um golpe, pelo saldo de vantagens (GDD 3.3).
// Saldo = vantagens do atacante sobre o alvo menos as do alvo sobre o atacante.
// Com uma vantagem de cada lado o dano fica perto do normal (1,3 x 0,7 = 0,91),
// por isso saldo zero nao tem aviso.
public static class Efetividade
{
    public static int Saldo(DadosPersonagem atacante, DadosPersonagem alvo)
    {
        return CalculadoraDano.ContarVantagens(atacante, alvo) - CalculadoraDano.ContarVantagens(alvo, atacante);
    }

    // null quando nao ha aviso
    public static string Rotulo(int saldo)
    {
        switch (saldo)
        {
            case 2: return "Super efetivo!";
            case 1: return "Efetivo!";
            case -1: return "Pouco efetivo...";
            case -2: return "Muito pouco efetivo...";
            default: return null;
        }
    }
}

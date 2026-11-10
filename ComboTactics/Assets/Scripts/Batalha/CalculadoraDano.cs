using UnityEngine;

// Formula de dano do GDD (secao 3.3):
// Dano = Ataque x Poder x (1 - 0,005 x Defesa) x vantagens do atacante x vantagens do alvo
public static class CalculadoraDano
{
    public const float PODER_BASICO = 1.2f;
    public const float PODER_TECNICA = 0.9f;
    public const float PODER_ESPECIAL = 2.2f;

    // Fogo -> Planta -> Agua -> Fogo
    public static bool TemVantagemDeElemento(Elemento atacante, Elemento alvo)
    {
        return (atacante == Elemento.Fogo && alvo == Elemento.Planta)
            || (atacante == Elemento.Planta && alvo == Elemento.Agua)
            || (atacante == Elemento.Agua && alvo == Elemento.Fogo);
    }

    // Guerreiro -> Mago -> Tanque -> Guerreiro (o Chefe nao tem vantagem de classe)
    public static bool TemVantagemDeClasse(Classe atacante, Classe alvo)
    {
        return (atacante == Classe.Guerreiro && alvo == Classe.Mago)
            || (atacante == Classe.Mago && alvo == Classe.Tanque)
            || (atacante == Classe.Tanque && alvo == Classe.Guerreiro);
    }

    // quantas vantagens (0, 1 ou 2) "a" tem sobre "b"
    public static int ContarVantagens(DadosPersonagem a, DadosPersonagem b)
    {
        int total = 0;
        if (TemVantagemDeElemento(a.elemento, b.elemento))
            total++;
        if (TemVantagemDeClasse(a.classe, b.classe))
            total++;
        return total;
    }

    public static float Poder(TipoGolpe golpe)
    {
        switch (golpe)
        {
            case TipoGolpe.Basico: return PODER_BASICO;
            case TipoGolpe.Tecnica: return PODER_TECNICA;
            default: return PODER_ESPECIAL;
        }
    }

    public static int Calcular(DadosPersonagem atacante, DadosPersonagem alvo, TipoGolpe golpe)
    {
        float dano = atacante.ataque * Poder(golpe);

        // cada ponto de defesa corta 0,5% do dano
        dano *= 1f - 0.005f * alvo.defesa;

        // vantagens do atacante aumentam o dano: +30% com uma, +50% com duas
        int vantagensAtacante = ContarVantagens(atacante, alvo);
        if (vantagensAtacante == 1) dano *= 1.3f;
        else if (vantagensAtacante == 2) dano *= 1.5f;

        // vantagens do alvo diminuem o dano recebido: -30% com uma, -50% com duas
        int vantagensAlvo = ContarVantagens(alvo, atacante);
        if (vantagensAlvo == 1) dano *= 0.7f;
        else if (vantagensAlvo == 2) dano *= 0.5f;

        return Mathf.RoundToInt(dano);
    }
}

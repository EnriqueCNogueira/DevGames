using System.Collections.Generic;

public class Equipe
{
    public const int STAMINA_MAXIMA = 100;

    public List<Lutador> Lutadores = new List<Lutador>();
    public int IndiceAtivo { get; private set; }

    // a barra de stamina e do jogador, nao de cada personagem
    public int Stamina { get; private set; }

    public Lutador Ativo
    {
        get { return Lutadores[IndiceAtivo]; }
    }

    public bool EspecialDisponivel
    {
        get { return Stamina >= STAMINA_MAXIMA; }
    }

    public Equipe(params DadosPersonagem[] personagens)
    {
        foreach (DadosPersonagem p in personagens)
            Lutadores.Add(new Lutador(p));
    }

    public bool TemAlguemVivo()
    {
        foreach (Lutador l in Lutadores)
        {
            if (l.EstaVivo)
                return true;
        }
        return false;
    }

    public bool PodeTrocarPara(int indice)
    {
        if (indice < 0 || indice >= Lutadores.Count)
            return false;
        return indice != IndiceAtivo && Lutadores[indice].EstaVivo;
    }

    public void TrocarAtivo(int indice)
    {
        IndiceAtivo = indice;
    }

    public void GanharStamina(int quantidade)
    {
        Stamina += quantidade;
        if (Stamina > STAMINA_MAXIMA)
            Stamina = STAMINA_MAXIMA;
    }

    public void ZerarStamina()
    {
        Stamina = 0;
    }
}

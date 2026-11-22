// Um personagem durante a batalha. Os dados fixos (nome, golpes, sprites) ficam no
// DadosPersonagem; os atributos ja vem com as evolucoes do perfil aplicadas.
public class Lutador
{
    public DadosPersonagem Dados { get; private set; }
    public Atributos Atributos { get; private set; }
    public int VidaAtual { get; private set; }

    // ultima rodada em que cada pocao ainda vale (0 = sem pocao).
    // O bonus fica no personagem, mesmo no banco, e as rodadas contam do mesmo jeito.
    public int PocaoAtaqueAte { get; private set; }
    public int PocaoDefesaAte { get; private set; }

    public int VidaMaxima
    {
        get { return Atributos.vida; }
    }

    public bool EstaVivo
    {
        get { return VidaAtual > 0; }
    }

    public bool VidaCheia
    {
        get { return VidaAtual >= VidaMaxima; }
    }

    public Lutador(DadosPersonagem dados) : this(dados, Atributos.Base(dados))
    {
    }

    public Lutador(DadosPersonagem dados, Atributos atributos)
    {
        Dados = dados;
        Atributos = atributos;
        VidaAtual = atributos.vida;
    }

    public void ReceberDano(int dano)
    {
        VidaAtual -= dano;
        if (VidaAtual < 0)
            VidaAtual = 0;
    }

    // devolve quanto curou de verdade (nao passa da vida maxima nem revive)
    public int Curar(int quantidade)
    {
        if (!EstaVivo || quantidade <= 0)
            return 0;
        int antes = VidaAtual;
        VidaAtual += quantidade;
        if (VidaAtual > VidaMaxima)
            VidaAtual = VidaMaxima;
        return VidaAtual - antes;
    }

    // usar de novo so renova a duracao, nao acumula
    public void TomarPocao(TipoItem pocao, int ateRodada)
    {
        if (pocao == TipoItem.PocaoAtaque)
            PocaoAtaqueAte = ateRodada;
        else if (pocao == TipoItem.PocaoDefesa)
            PocaoDefesaAte = ateRodada;
    }

    public bool TemPocaoAtaque(int rodada)
    {
        return rodada <= PocaoAtaqueAte;
    }

    public bool TemPocaoDefesa(int rodada)
    {
        return rodada <= PocaoDefesaAte;
    }

    // rodadas em que a pocao ainda vale, contando a atual, para mostrar na tela
    // (0 = sem pocao). Na rodada do uso mostra 3, como as 3 rodadas que ela cobre.
    public int RodadasRestantes(TipoItem pocao, int rodada)
    {
        int ate = pocao == TipoItem.PocaoAtaque ? PocaoAtaqueAte : PocaoDefesaAte;
        if (ate < rodada)
            return 0;
        return System.Math.Min(Itens.RODADAS_POCAO, ate - rodada + 1);
    }
}

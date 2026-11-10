using NUnit.Framework;

public class BatalhaTestes
{
    Batalha batalha;

    // Jogador 1: Guerreiro de Agua (vel 90), Mago de Fogo, Tanque de Planta
    // Jogador 2: Guerreiro de Fogo (vel 77), Mago de Agua, Tanque de Fogo
    [SetUp]
    public void Preparar()
    {
        Equipe jogador1 = new Equipe(Personagens.GuerreiroAgua(), Personagens.MagoFogo(), Personagens.TanquePlanta());
        Equipe jogador2 = new Equipe(Personagens.GuerreiroFogo(), Personagens.MagoAgua(), Personagens.TanqueFogo());
        batalha = new Batalha(jogador1, jogador2);
    }

    // deixa o personagem com 1 de vida, para cair no proximo golpe
    void DeixarComUmDeVida(Lutador lutador)
    {
        lutador.ReceberDano(lutador.VidaAtual - 1);
    }

    [Test]
    public void MaisRapidoComeca()
    {
        Assert.AreEqual(0, batalha.JogadorDaVez);
        Assert.AreEqual(1, batalha.Rodada);
    }

    [Test]
    public void AtaqueTiraVidaDoAlvo()
    {
        Lutador alvo = batalha.Equipes[1].Ativo;
        int danoEsperado = CalculadoraDano.Calcular(batalha.Equipes[0].Ativo.Dados, alvo.Dados, TipoGolpe.Basico);

        batalha.Atacar(TipoGolpe.Basico);

        Assert.AreEqual(620 - danoEsperado, alvo.VidaAtual);
    }

    [Test]
    public void VezPassaEDepoisComecaOutraRodada()
    {
        batalha.Atacar(TipoGolpe.Basico);
        Assert.AreEqual(1, batalha.JogadorDaVez);
        Assert.AreEqual(1, batalha.Rodada);

        batalha.Atacar(TipoGolpe.Basico);
        Assert.AreEqual(0, batalha.JogadorDaVez);
        Assert.AreEqual(2, batalha.Rodada);
    }

    [Test]
    public void TrocarGastaAVez()
    {
        bool trocou = batalha.Trocar(1);

        Assert.IsTrue(trocou);
        Assert.AreEqual("Mago de Fogo", batalha.Equipes[0].Ativo.Dados.nome);
        Assert.AreEqual(1, batalha.JogadorDaVez);
        Assert.AreEqual(620, batalha.Equipes[1].Ativo.VidaAtual);
    }

    [Test]
    public void NaoTrocaParaQuemJaEstaNaArena()
    {
        Assert.IsFalse(batalha.Trocar(0));
        Assert.AreEqual(0, batalha.JogadorDaVez);
    }

    [Test]
    public void NaoTrocaParaPersonagemDerrotado()
    {
        Lutador mago = batalha.Equipes[0].Lutadores[1];
        mago.ReceberDano(mago.VidaAtual);

        Assert.IsFalse(batalha.Trocar(1));
    }

    [Test]
    public void StaminaSobeAoCausarESofrerDano()
    {
        batalha.Atacar(TipoGolpe.Basico);

        Assert.AreEqual(20, batalha.Equipes[0].Stamina);
        Assert.AreEqual(15, batalha.Equipes[1].Stamina);
    }

    [Test]
    public void EspecialPrecisaDaBarraCheia()
    {
        Assert.IsFalse(batalha.Atacar(TipoGolpe.Especial));
        Assert.AreEqual(0, batalha.JogadorDaVez);
    }

    [Test]
    public void EspecialEsvaziaABarra()
    {
        // 3 rodadas com os dois atacando: 35 de stamina por rodada para o jogador 1
        for (int i = 0; i < 3; i++)
        {
            batalha.Atacar(TipoGolpe.Basico);
            batalha.Atacar(TipoGolpe.Basico);
        }
        Assert.AreEqual(100, batalha.Equipes[0].Stamina);

        Assert.IsTrue(batalha.Atacar(TipoGolpe.Especial));
        Assert.AreEqual(0, batalha.Equipes[0].Stamina);
    }

    [Test]
    public void DerrotadoPrecisaDeSubstitutoEPerdeAVez()
    {
        DeixarComUmDeVida(batalha.Equipes[1].Ativo);

        batalha.Atacar(TipoGolpe.Basico);

        Assert.IsTrue(batalha.PrecisaDeSubstituto(1));
        Assert.IsFalse(batalha.Atacar(TipoGolpe.Basico)); // ninguem ataca antes da escolha

        Assert.IsTrue(batalha.EscolherSubstituto(1, 1));
        Assert.AreEqual("Mago de Agua", batalha.Equipes[1].Ativo.Dados.nome);

        // o jogador 2 nao agiu na rodada 1: ja comeca a rodada 2, e o Guerreiro de Agua e mais rapido
        Assert.AreEqual(2, batalha.Rodada);
        Assert.AreEqual(0, batalha.JogadorDaVez);
    }

    [Test]
    public void NaoEscolheSubstitutoDerrotado()
    {
        DeixarComUmDeVida(batalha.Equipes[1].Ativo);
        batalha.Atacar(TipoGolpe.Basico);

        Assert.IsFalse(batalha.EscolherSubstituto(1, 0));
        Assert.IsTrue(batalha.PrecisaDeSubstituto(1));
    }

    [Test]
    public void VenceQuemDerrotaOsTres()
    {
        foreach (Lutador l in batalha.Equipes[1].Lutadores)
            DeixarComUmDeVida(l);

        batalha.Atacar(TipoGolpe.Basico);
        batalha.EscolherSubstituto(1, 1);
        batalha.Atacar(TipoGolpe.Basico);
        batalha.EscolherSubstituto(1, 2);
        Assert.IsFalse(batalha.Terminou);

        batalha.Atacar(TipoGolpe.Basico);

        Assert.IsTrue(batalha.Terminou);
        Assert.AreEqual(0, batalha.Vencedor);
        Assert.IsFalse(batalha.Atacar(TipoGolpe.Basico));
    }
}

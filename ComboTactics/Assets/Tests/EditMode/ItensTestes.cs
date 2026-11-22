using NUnit.Framework;

public class ItensTestes
{
    // Jogador 1: Guerreiro de Agua (vel 90) age primeiro. Jogador 2: Guerreiro de Fogo.
    Batalha CriarBatalha(Mochila mochila1, Mochila mochila2 = null)
    {
        Equipe j1 = new Equipe(new[] { Personagens.GuerreiroAgua(), Personagens.MagoFogo(), Personagens.TanquePlanta() }, null, mochila1);
        Equipe j2 = new Equipe(new[] { Personagens.GuerreiroFogo(), Personagens.MagoAgua(), Personagens.TanqueFogo() }, null, mochila2);
        return new Batalha(j1, j2);
    }

    static Mochila Com(params TipoItem[] itens)
    {
        Mochila m = new Mochila();
        foreach (TipoItem t in itens)
            m.Adicionar(t);
        return m;
    }

    [Test]
    public void MochilaAceitaNoMaximoTresItens()
    {
        Mochila m = Com(TipoItem.KitMedico, TipoItem.KitMedico, TipoItem.PocaoAtaque);

        Assert.IsFalse(m.Adicionar(TipoItem.PocaoDefesa));
        Assert.AreEqual(3, m.Total);
    }

    [Test]
    public void RemedioAindaNaoEntraNaMochila()
    {
        Assert.IsFalse(new Mochila().Adicionar(TipoItem.Remedio));
    }

    [Test]
    public void KitMedicoCuraMetadeDaVidaEGastaAVez()
    {
        Batalha b = CriarBatalha(Com(TipoItem.KitMedico));
        Lutador ativo = b.Equipes[0].Ativo;
        ativo.ReceberDano(400); // 620 - 400 = 220

        Assert.IsTrue(b.UsarItem(TipoItem.KitMedico));

        Assert.AreEqual(220 + 310, ativo.VidaAtual);
        Assert.AreEqual(310, b.UltimoItem.Cura);
        Assert.AreEqual(1, b.JogadorDaVez);
        Assert.AreEqual(0, b.Equipes[0].Mochila.Quantidade(TipoItem.KitMedico));
    }

    [Test]
    public void KitMedicoNaoPassaDaVidaMaxima()
    {
        Batalha b = CriarBatalha(Com(TipoItem.KitMedico));
        Lutador ativo = b.Equipes[0].Ativo;
        ativo.ReceberDano(100);

        b.UsarItem(TipoItem.KitMedico);

        Assert.AreEqual(620, ativo.VidaAtual);
        Assert.AreEqual(100, b.UltimoItem.Cura);
    }

    [Test]
    public void KitMedicoNaoServeComVidaCheia()
    {
        Batalha b = CriarBatalha(Com(TipoItem.KitMedico));

        Assert.IsFalse(b.UsarItem(TipoItem.KitMedico));
        Assert.AreEqual(0, b.JogadorDaVez);
    }

    [Test]
    public void NaoUsaItemQueNaoTrouxe()
    {
        Batalha b = CriarBatalha(Com(TipoItem.PocaoAtaque));

        Assert.IsFalse(b.UsarItem(TipoItem.PocaoDefesa));
    }

    [Test]
    public void ItemNaoCarregaStamina()
    {
        Batalha b = CriarBatalha(Com(TipoItem.PocaoAtaque));

        b.UsarItem(TipoItem.PocaoAtaque);

        Assert.AreEqual(0, b.Equipes[0].Stamina);
    }

    [Test]
    public void PocaoDeAtaqueAumentaODanoEmTrintaPorCento()
    {
        Batalha b = CriarBatalha(Com(TipoItem.PocaoAtaque));
        Lutador atacante = b.Equipes[0].Ativo;
        Lutador alvo = b.Equipes[1].Ativo;
        int semPocao = CalculadoraDano.Calcular(atacante, alvo, TipoGolpe.Basico, b.Rodada);

        b.UsarItem(TipoItem.PocaoAtaque);
        int comPocao = CalculadoraDano.Calcular(atacante, alvo, TipoGolpe.Basico, b.Rodada);

        Assert.AreEqual(semPocao * 1.3f, comPocao, 1f);
    }

    [Test]
    public void PocaoDeDefesaReduzODanoRecebidoEmTrintaPorCento()
    {
        Batalha b = CriarBatalha(Com(TipoItem.PocaoDefesa));
        Lutador defensor = b.Equipes[0].Ativo;
        Lutador atacante = b.Equipes[1].Ativo;
        int semPocao = CalculadoraDano.Calcular(atacante, defensor, TipoGolpe.Basico, b.Rodada);

        b.UsarItem(TipoItem.PocaoDefesa);
        int vidaAntes = defensor.VidaAtual;
        b.Atacar(TipoGolpe.Basico); // o Jogador 2 ataca quem tomou a pocao

        Assert.AreEqual(semPocao * 0.7f, vidaAntes - defensor.VidaAtual, 1f);
    }

    [Test]
    public void PocaoValeNaRodadaDoUsoENasTresSeguintes()
    {
        Batalha b = CriarBatalha(Com(TipoItem.PocaoAtaque));
        Lutador ativo = b.Equipes[0].Ativo;

        b.UsarItem(TipoItem.PocaoAtaque); // rodada 1

        Assert.IsTrue(ativo.TemPocaoAtaque(1));
        Assert.IsTrue(ativo.TemPocaoAtaque(4));
        Assert.IsFalse(ativo.TemPocaoAtaque(5));
        Assert.AreEqual(3, ativo.RodadasRestantes(TipoItem.PocaoAtaque, 1));
        Assert.AreEqual(1, ativo.RodadasRestantes(TipoItem.PocaoAtaque, 4));
        Assert.AreEqual(0, ativo.RodadasRestantes(TipoItem.PocaoAtaque, 5));
    }

    [Test]
    public void PocaoFicaNoPersonagemQuandoEleVaiParaOBanco()
    {
        Batalha b = CriarBatalha(Com(TipoItem.PocaoAtaque));
        Lutador tomou = b.Equipes[0].Ativo;
        b.UsarItem(TipoItem.PocaoAtaque);
        b.Atacar(TipoGolpe.Basico); // J2, fim da rodada 1

        b.Trocar(1); // J1 manda o Mago para a arena

        Assert.AreNotSame(tomou, b.Equipes[0].Ativo);
        Assert.IsTrue(tomou.TemPocaoAtaque(b.Rodada));
        Assert.IsFalse(b.Equipes[0].Ativo.TemPocaoAtaque(b.Rodada));
    }

    [Test]
    public void PocaoRepetidaRenovaSemAcumular()
    {
        Batalha b = CriarBatalha(Com(TipoItem.PocaoAtaque, TipoItem.PocaoAtaque));
        Lutador ativo = b.Equipes[0].Ativo;
        b.UsarItem(TipoItem.PocaoAtaque);      // rodada 1, vale ate a 4
        b.Atacar(TipoGolpe.Basico);            // J2
        b.UsarItem(TipoItem.PocaoAtaque);      // rodada 2, vale ate a 5
        int dano = CalculadoraDano.Calcular(ativo, b.Equipes[1].Ativo, TipoGolpe.Basico, b.Rodada);
        int danoBase = CalculadoraDano.Calcular(ativo.Dados, b.Equipes[1].Ativo.Dados, TipoGolpe.Basico);

        Assert.AreEqual(5, ativo.PocaoAtaqueAte);
        Assert.AreEqual(danoBase * 1.3f, dano, 1f);
    }

    [Test]
    public void SelecaoNaoDeixaSepararOQueOOutroJaSeparou()
    {
        int[] estoque = { 2, 1, 0, 0 };
        SelecaoItens j1 = new SelecaoItens(estoque, null);
        j1.Adicionar(TipoItem.KitMedico);
        j1.Adicionar(TipoItem.PocaoAtaque);

        SelecaoItens j2 = new SelecaoItens(estoque, j1.Mochila);

        Assert.AreEqual(1, j2.Disponivel(TipoItem.KitMedico));
        Assert.AreEqual(0, j2.Disponivel(TipoItem.PocaoAtaque));
        Assert.IsFalse(j2.Adicionar(TipoItem.PocaoAtaque));
        Assert.IsTrue(j2.Adicionar(TipoItem.KitMedico));
        Assert.IsFalse(j2.Adicionar(TipoItem.KitMedico));
    }

    [Test]
    public void SelecaoAjustaAMochilaQuandoOEstoqueDiminuiu()
    {
        Mochila antiga = Com(TipoItem.KitMedico, TipoItem.KitMedico);
        int[] estoqueAgora = { 1, 0, 0, 0 };

        SelecaoItens s = new SelecaoItens(estoqueAgora, null, antiga);

        Assert.AreEqual(1, s.Mochila.Quantidade(TipoItem.KitMedico));
    }

    [Test]
    public void SemEstoqueNaoHaNadaParaEscolher()
    {
        Assert.IsFalse(new SelecaoItens(new int[4], null).TemAlgoParaEscolher);
    }
}

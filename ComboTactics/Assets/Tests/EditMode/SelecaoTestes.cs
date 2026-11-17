using NUnit.Framework;

public class SelecaoEquipeTestes
{
    SelecaoEquipe equipe;
    DadosPersonagem guerreiro, mago, tanque, outro;

    [SetUp]
    public void Preparar()
    {
        equipe = new SelecaoEquipe();
        guerreiro = Personagens.GuerreiroAgua();
        mago = Personagens.MagoFogo();
        tanque = Personagens.TanquePlanta();
        outro = Personagens.TanqueFogo();
    }

    void EscolherTres()
    {
        equipe.Adicionar(guerreiro);
        equipe.Adicionar(mago);
        equipe.Adicionar(tanque);
    }

    [Test]
    public void ComecaVazia()
    {
        Assert.AreEqual(0, equipe.Quantidade);
        Assert.IsNull(equipe.Titular);
        Assert.IsFalse(equipe.Completa);
        Assert.IsNull(equipe.ParaArray());
    }

    [Test]
    public void PrimeiroEscolhidoEOTitular()
    {
        EscolherTres();

        Assert.AreSame(guerreiro, equipe.Titular);
        CollectionAssert.AreEqual(new[] { guerreiro, mago, tanque }, equipe.ParaArray());
    }

    [Test]
    public void NaoRepetePersonagem()
    {
        equipe.Adicionar(guerreiro);

        Assert.IsFalse(equipe.Adicionar(guerreiro));
        Assert.AreEqual(1, equipe.Quantidade);
    }

    [Test]
    public void NaoPassaDeTres()
    {
        EscolherTres();

        Assert.IsTrue(equipe.Completa);
        Assert.IsFalse(equipe.Adicionar(outro));
        Assert.AreEqual(3, equipe.Quantidade);
    }

    [Test]
    public void RecusaVagaNula()
    {
        Assert.IsFalse(equipe.Adicionar(null));
        Assert.AreEqual(0, equipe.Quantidade);
    }

    [Test]
    public void RemoverOTitularPromoveOSegundo()
    {
        EscolherTres();

        Assert.IsTrue(equipe.Remover(0));

        Assert.AreSame(mago, equipe.Titular);
        Assert.AreSame(tanque, equipe.Em(1));
        Assert.IsNull(equipe.Em(2));
        Assert.IsFalse(equipe.Completa);
    }

    [Test]
    public void RemoverVagaVaziaNaoFazNada()
    {
        equipe.Adicionar(guerreiro);

        Assert.IsFalse(equipe.Remover(1));
        Assert.IsFalse(equipe.Remover(-1));
        Assert.AreEqual(1, equipe.Quantidade);
    }

    [Test]
    public void PodeEscolherDeNovoQuemFoiRemovido()
    {
        EscolherTres();
        equipe.Remover(1);

        Assert.IsTrue(equipe.Adicionar(mago));
        CollectionAssert.AreEqual(new[] { guerreiro, tanque, mago }, equipe.ParaArray());
    }
}

public class SelecaoPartidaTestes
{
    SelecaoPartida selecao;
    DadosPersonagem a, b, c, d;

    [SetUp]
    public void Preparar()
    {
        selecao = new SelecaoPartida();
        a = Personagens.GuerreiroAgua();
        b = Personagens.MagoFogo();
        c = Personagens.TanquePlanta();
        d = Personagens.TanqueFogo();
    }

    void EscolherTres()
    {
        selecao.Escolher(a);
        selecao.Escolher(b);
        selecao.Escolher(c);
    }

    [Test]
    public void Jogador1EscolhePrimeiro()
    {
        Assert.AreEqual(0, selecao.JogadorDaVez);
        Assert.IsFalse(selecao.Pronta);
    }

    [Test]
    public void SoConfirmaComEquipeCompleta()
    {
        selecao.Escolher(a);
        selecao.Escolher(b);

        Assert.IsFalse(selecao.PodeConfirmar);
        Assert.IsFalse(selecao.Confirmar());
        Assert.AreEqual(0, selecao.JogadorDaVez);
    }

    [Test]
    public void ConfirmarPassaAVezParaOJogador2()
    {
        EscolherTres();

        Assert.IsTrue(selecao.Confirmar());
        Assert.AreEqual(1, selecao.JogadorDaVez);
        Assert.IsFalse(selecao.Pronta);
    }

    [Test]
    public void Jogador2PodeEspelharAEquipeDoJogador1()
    {
        EscolherTres();
        selecao.Confirmar();

        EscolherTres();

        Assert.IsTrue(selecao.Equipes[1].Completa);
        CollectionAssert.AreEqual(selecao.Equipes[0].ParaArray(), selecao.Equipes[1].ParaArray());
    }

    [Test]
    public void EscolhaDoJogador2NaoMexeNaEquipeDoJogador1()
    {
        EscolherTres();
        selecao.Confirmar();

        selecao.Escolher(d);
        selecao.Remover(0);

        CollectionAssert.AreEqual(new[] { a, b, c }, selecao.Equipes[0].ParaArray());
        Assert.AreEqual(0, selecao.Equipes[1].Quantidade);
    }

    [Test]
    public void ConfirmacaoDoJogador2DeixaAPartidaPronta()
    {
        EscolherTres();
        selecao.Confirmar();
        EscolherTres();

        Assert.IsTrue(selecao.Confirmar());
        Assert.IsTrue(selecao.Pronta);
        Assert.IsFalse(selecao.Escolher(d));
        Assert.IsFalse(selecao.Confirmar());
    }

    [Test]
    public void VoltarDoJogador2DevolveAVezMantendoAsEscolhas()
    {
        EscolherTres();
        selecao.Confirmar();
        selecao.Escolher(d);

        Assert.IsFalse(selecao.Voltar());

        Assert.AreEqual(0, selecao.JogadorDaVez);
        Assert.IsTrue(selecao.Equipes[0].Completa);
        Assert.AreSame(d, selecao.Equipes[1].Titular);
    }

    [Test]
    public void VoltarDoJogador1SaiParaOMenu()
    {
        selecao.Escolher(a);

        Assert.IsTrue(selecao.Voltar());
    }
}

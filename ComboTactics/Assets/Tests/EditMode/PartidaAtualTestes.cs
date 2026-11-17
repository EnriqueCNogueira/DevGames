using System;
using NUnit.Framework;

public class PartidaAtualTestes
{
    DadosPersonagem[] equipe1;
    DadosPersonagem[] equipe2;

    // a classe e estatica: cada teste comeca com ela limpa
    [SetUp]
    public void Preparar()
    {
        PartidaAtual.Limpar();
        equipe1 = new[] { Personagens.GuerreiroAgua(), Personagens.MagoFogo(), Personagens.TanquePlanta() };
        equipe2 = new[] { Personagens.GuerreiroFogo(), Personagens.MagoAgua(), Personagens.TanqueFogo() };
    }

    [TearDown]
    public void Encerrar()
    {
        PartidaAtual.Limpar();
    }

    [Test]
    public void ComecaVazia()
    {
        Assert.IsFalse(PartidaAtual.TemEquipes);
        Assert.IsNull(PartidaAtual.Equipe(0));
        Assert.IsNull(PartidaAtual.Equipe(1));
    }

    [Test]
    public void GuardaAsDuasEquipesNaOrdem()
    {
        PartidaAtual.Guardar(equipe1, equipe2);

        Assert.IsTrue(PartidaAtual.TemEquipes);
        CollectionAssert.AreEqual(equipe1, PartidaAtual.Equipe(0));
        CollectionAssert.AreEqual(equipe2, PartidaAtual.Equipe(1));
    }

    [Test]
    public void AceitaConfrontoEspelhado()
    {
        PartidaAtual.Guardar(equipe1, equipe1);

        CollectionAssert.AreEqual(PartidaAtual.Equipe(0), PartidaAtual.Equipe(1));
    }

    [Test]
    public void LimparEsvazia()
    {
        PartidaAtual.Guardar(equipe1, equipe2);
        PartidaAtual.Limpar();

        Assert.IsFalse(PartidaAtual.TemEquipes);
        Assert.IsNull(PartidaAtual.Equipe(0));
        Assert.IsNull(PartidaAtual.Equipe(1));
    }

    [Test]
    public void MudarOArrayOriginalNaoMudaAPartida()
    {
        PartidaAtual.Guardar(equipe1, equipe2);
        DadosPersonagem titular = equipe1[0];
        DadosPersonagem segundo = equipe1[1];

        // muda o array que a Selecao passou e a copia que a Batalha recebeu
        equipe1[0] = Personagens.TanqueAgua();
        PartidaAtual.Equipe(0)[1] = Personagens.TanqueAgua();

        Assert.AreSame(titular, PartidaAtual.Equipe(0)[0]);
        Assert.AreSame(segundo, PartidaAtual.Equipe(0)[1]);
    }

    [Test]
    public void RecusaEquipeIncompleta()
    {
        DadosPersonagem[] comVagaVazia = { Personagens.GuerreiroAgua(), null, Personagens.TanquePlanta() };
        DadosPersonagem[] comDois = { Personagens.GuerreiroAgua(), Personagens.MagoFogo() };

        Assert.Throws<ArgumentException>(() => PartidaAtual.Guardar(comVagaVazia, equipe2));
        Assert.Throws<ArgumentException>(() => PartidaAtual.Guardar(equipe1, comDois));
        Assert.Throws<ArgumentException>(() => PartidaAtual.Guardar(null, equipe2));
        Assert.IsFalse(PartidaAtual.TemEquipes);
    }
}

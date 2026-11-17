using NUnit.Framework;

// A revanche recarrega a cena e monta uma Batalha nova com as equipes da PartidaAtual
public class RevancheTestes
{
    DadosPersonagem[] equipe1;
    DadosPersonagem[] equipe2;

    [SetUp]
    public void Preparar()
    {
        PartidaAtual.Limpar();
        equipe1 = new[] { Personagens.GuerreiroAgua(), Personagens.MagoFogo(), Personagens.TanquePlanta() };
        equipe2 = new[] { Personagens.GuerreiroFogo(), Personagens.MagoAgua(), Personagens.TanqueFogo() };
        PartidaAtual.Guardar(equipe1, equipe2);
    }

    [TearDown]
    public void Encerrar()
    {
        PartidaAtual.Limpar();
    }

    Batalha NovaBatalha()
    {
        return new Batalha(new Equipe(PartidaAtual.Equipe(0)), new Equipe(PartidaAtual.Equipe(1)));
    }

    [Test]
    public void RevancheComecaComVidaCheiaEStaminaZerada()
    {
        // primeira luta: alguns golpes e uma troca
        Batalha primeira = NovaBatalha();
        primeira.Atacar(TipoGolpe.Basico);
        primeira.Atacar(TipoGolpe.Basico);
        primeira.Trocar(1);
        Assert.Greater(primeira.Equipes[0].Stamina, 0);

        Batalha revanche = NovaBatalha();

        for (int j = 0; j < 2; j++)
        {
            Assert.AreEqual(0, revanche.Equipes[j].Stamina);
            Assert.AreEqual(0, revanche.Equipes[j].IndiceAtivo);
            foreach (Lutador l in revanche.Equipes[j].Lutadores)
                Assert.AreEqual(l.Dados.vida, l.VidaAtual);
        }
        Assert.AreEqual(1, revanche.Rodada);
        Assert.IsFalse(revanche.Terminou);
        Assert.IsEmpty(revanche.Registro);
    }

    [Test]
    public void RevancheMantemAsMesmasEquipesNaMesmaOrdem()
    {
        Batalha primeira = NovaBatalha();
        primeira.Atacar(TipoGolpe.Basico);

        Batalha revanche = NovaBatalha();

        for (int i = 0; i < 3; i++)
        {
            Assert.AreSame(equipe1[i], revanche.Equipes[0].Lutadores[i].Dados);
            Assert.AreSame(equipe2[i], revanche.Equipes[1].Lutadores[i].Dados);
        }
    }
}

using NUnit.Framework;
using UnityEngine;

public class PerfilTestes
{
    DadosPersonagem guerreiro;
    DadosPersonagem tanque;

    [SetUp]
    public void Preparar()
    {
        guerreiro = Personagens.GuerreiroFogo(); // 620, 130, 45, 77
        guerreiro.name = "GuerreiroFogo";
        tanque = Personagens.TanquePlanta();     // 880, 63, 95, 23
        tanque.name = "TanquePlanta";
    }

    [Test]
    public void PerfilNovoComecaComTrezentasMoedas()
    {
        Perfil p = new Perfil();

        Assert.AreEqual(300, p.moedas);
        Assert.AreEqual(0, p.Estoque(TipoItem.KitMedico));
    }

    [Test]
    public void ComprarItemGastaMoedasEAumentaOEstoque()
    {
        Perfil p = new Perfil();

        Assert.IsTrue(p.Comprar(TipoItem.KitMedico));

        Assert.AreEqual(260, p.moedas);
        Assert.AreEqual(1, p.Estoque(TipoItem.KitMedico));
    }

    [Test]
    public void EstoqueTemTetoDeCinco()
    {
        Perfil p = new Perfil { moedas = 1000 };
        for (int i = 0; i < 5; i++)
            p.Comprar(TipoItem.PocaoAtaque);

        Assert.IsFalse(p.Comprar(TipoItem.PocaoAtaque));
        Assert.AreEqual(5, p.Estoque(TipoItem.PocaoAtaque));
        Assert.AreEqual(1000 - 5 * 30, p.moedas);
    }

    [Test]
    public void NaoCompraSemMoedasSuficientes()
    {
        Perfil p = new Perfil { moedas = 39 };

        Assert.IsFalse(p.Comprar(TipoItem.KitMedico));
        Assert.AreEqual(39, p.moedas);
    }

    [Test]
    public void RemedioNaoEstaAVenda()
    {
        Assert.IsFalse(new Perfil().Comprar(TipoItem.Remedio));
    }

    [Test]
    public void ConsumirTiraDoEstoque()
    {
        Perfil p = new Perfil();
        p.Comprar(TipoItem.PocaoDefesa);

        Assert.IsTrue(p.Consumir(TipoItem.PocaoDefesa));
        Assert.IsFalse(p.Consumir(TipoItem.PocaoDefesa));
    }

    [Test]
    public void PrecoDaEvolucaoSobeComONivel()
    {
        Assert.AreEqual(50, Evolucao.Preco(0));
        Assert.AreEqual(250, Evolucao.Preco(4));
        Assert.AreEqual(0, Evolucao.Preco(5));
    }

    [Test]
    public void EvoluirSomaTresPorCentoDoBase()
    {
        Perfil p = new Perfil { moedas = 1000 };

        p.Evoluir(guerreiro, Atributo.Ataque);
        p.Evoluir(guerreiro, Atributo.Ataque);

        Assert.AreEqual(Mathf.RoundToInt(130 * 1.06f), p.AtributosDe(guerreiro).ataque);
        Assert.AreEqual(1000 - 50 - 100, p.moedas);
        Assert.AreEqual(620, p.AtributosDe(guerreiro).vida);
    }

    [Test]
    public void EvolucaoTemNoMaximoCincoNiveis()
    {
        Perfil p = new Perfil { moedas = 5000 };
        for (int i = 0; i < 5; i++)
            Assert.IsTrue(p.Evoluir(guerreiro, Atributo.Vida));

        Assert.IsFalse(p.Evoluir(guerreiro, Atributo.Vida));
        Assert.AreEqual(Mathf.RoundToInt(620 * 1.15f), p.AtributosDe(guerreiro).vida);
        Assert.AreEqual(5000 - 750, p.moedas);
    }

    [Test]
    public void EvolucaoEIndividualPorPersonagem()
    {
        Perfil p = new Perfil();
        p.Evoluir(guerreiro, Atributo.Velocidade);

        Assert.AreEqual(0, p.Nivel(tanque, Atributo.Velocidade));
        Assert.AreEqual(1, p.Nivel(guerreiro, Atributo.Velocidade));
    }

    [Test]
    public void TodoNivelSobePeloMenosUmPonto()
    {
        // Defesa 25 do Sapo: 3% e 0,75 ponto, e o arredondamento repetiria 26
        int anterior = 25;
        for (int nivel = 1; nivel <= Evolucao.NIVEL_MAXIMO; nivel++)
        {
            int valor = Evolucao.Valor(25, nivel, Atributo.Defesa);
            Assert.Greater(valor, anterior, "nivel " + nivel);
            anterior = valor;
        }
        Assert.AreEqual(30, anterior);
    }

    [Test]
    public void DefesaNaoPassaDeCem()
    {
        Assert.AreEqual(100, Evolucao.Valor(95, 5, Atributo.Defesa));
        Assert.AreEqual(109, Evolucao.Valor(95, 5, Atributo.Ataque));
    }

    [Test]
    public void NaoVendeEvolucaoDeDefesaAcimaDeCem()
    {
        // Arvore: 95 > 98 > 101 (vira 100) > parado em 100
        Perfil p = new Perfil { moedas = 5000 };

        Assert.IsTrue(p.Evoluir(tanque, Atributo.Defesa));
        Assert.IsTrue(p.Evoluir(tanque, Atributo.Defesa));
        Assert.IsFalse(p.Evoluir(tanque, Atributo.Defesa));
        Assert.AreEqual(100, p.AtributosDe(tanque).defesa);
        Assert.AreEqual(5000 - 50 - 100, p.moedas);
    }

    [Test]
    public void PerfilSobreviveAoJson()
    {
        Perfil p = new Perfil { moedas = 500 };
        p.Comprar(TipoItem.KitMedico);
        p.Evoluir(tanque, Atributo.Defesa);

        Perfil lido = JsonUtility.FromJson<Perfil>(JsonUtility.ToJson(p));

        Assert.AreEqual(p.moedas, lido.moedas);
        Assert.AreEqual(1, lido.Estoque(TipoItem.KitMedico));
        Assert.AreEqual(1, lido.Nivel(tanque, Atributo.Defesa));
    }

    [Test]
    public void BatalhaUsaOsAtributosEvoluidos()
    {
        Perfil p = new Perfil { moedas = 5000 };
        for (int i = 0; i < 5; i++)
            p.Evoluir(tanque, Atributo.Vida);

        Equipe e = new Equipe(new[] { tanque, guerreiro, Personagens.MagoAgua() }, p, null);

        Assert.AreEqual(1012, e.Ativo.VidaMaxima);
        Assert.AreEqual(1012, e.Ativo.VidaAtual);
    }

    [Test]
    public void VelocidadeEvoluidaDecideQuemComeca()
    {
        // Curupira (73) evoluido ao maximo chega a 84 e passa a Mula (77)
        DadosPersonagem curupira = Personagens.GuerreiroAgua();
        curupira.name = "Curupira";
        curupira.velocidade = 73;
        Perfil p = new Perfil { moedas = 5000 };
        for (int i = 0; i < 5; i++)
            p.Evoluir(curupira, Atributo.Velocidade);

        Equipe j1 = new Equipe(new[] { guerreiro, Personagens.MagoAgua(), Personagens.TanqueFogo() }, p, null);
        Equipe j2 = new Equipe(new[] { curupira, Personagens.MagoFogo(), tanque }, p, null);

        Assert.AreEqual(1, new Batalha(j1, j2).JogadorDaVez);
    }
}

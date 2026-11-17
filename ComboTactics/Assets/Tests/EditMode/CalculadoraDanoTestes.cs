using NUnit.Framework;

public class CalculadoraDanoTestes
{
    [Test]
    public void CicloDeElementos()
    {
        Assert.IsTrue(CalculadoraDano.TemVantagemDeElemento(Elemento.Fogo, Elemento.Planta));
        Assert.IsTrue(CalculadoraDano.TemVantagemDeElemento(Elemento.Planta, Elemento.Agua));
        Assert.IsTrue(CalculadoraDano.TemVantagemDeElemento(Elemento.Agua, Elemento.Fogo));

        Assert.IsFalse(CalculadoraDano.TemVantagemDeElemento(Elemento.Planta, Elemento.Fogo));
        Assert.IsFalse(CalculadoraDano.TemVantagemDeElemento(Elemento.Fogo, Elemento.Fogo));
    }

    [Test]
    public void CicloDeClasses()
    {
        Assert.IsTrue(CalculadoraDano.TemVantagemDeClasse(Classe.Guerreiro, Classe.Mago));
        Assert.IsTrue(CalculadoraDano.TemVantagemDeClasse(Classe.Mago, Classe.Tanque));
        Assert.IsTrue(CalculadoraDano.TemVantagemDeClasse(Classe.Tanque, Classe.Guerreiro));

        Assert.IsFalse(CalculadoraDano.TemVantagemDeClasse(Classe.Mago, Classe.Guerreiro));
        Assert.IsFalse(CalculadoraDano.TemVantagemDeClasse(Classe.Chefe, Classe.Mago));
        Assert.IsFalse(CalculadoraDano.TemVantagemDeClasse(Classe.Guerreiro, Classe.Chefe));
    }

    [Test]
    public void SemVantagens()
    {
        // 130 x 1,2 x (1 - 0,005 x 45) = 120,9
        int dano = CalculadoraDano.Calcular(Personagens.GuerreiroFogo(), Personagens.GuerreiroFogo(), TipoGolpe.Basico);
        Assert.AreEqual(121, dano);
    }

    [Test]
    public void DuasVantagensAtacando()
    {
        // exemplo do GDD: Mago de Fogo contra Tanque de Planta tem vantagem de classe e de elemento
        // 171 x 1,2 x (1 - 0,005 x 95) x 1,5 = 161,6
        int dano = CalculadoraDano.Calcular(Personagens.MagoFogo(), Personagens.TanquePlanta(), TipoGolpe.Basico);
        Assert.AreEqual(162, dano);
    }

    [Test]
    public void DuasVantagensDefendendo()
    {
        // o contrario: o Tanque de Planta recebe so metade do dano normal
        // 63 x 1,2 x (1 - 0,005 x 15) x 0,5 = 34,97
        int dano = CalculadoraDano.Calcular(Personagens.TanquePlanta(), Personagens.MagoFogo(), TipoGolpe.Basico);
        Assert.AreEqual(35, dano);

        // o GDD fala em uns 16 golpes basicos para derrubar o Mago de Fogo
        int golpes = (560 + dano - 1) / dano;
        Assert.AreEqual(16, golpes);
    }

    [Test]
    public void UmaVantagemDeCadaLado()
    {
        // Guerreiro de Fogo tem vantagem de classe, mas o Mago de Agua tem vantagem de elemento
        // 130 x 1,2 x (1 - 0,005 x 25) x 1,3 x 0,7 = 124,2
        int dano = CalculadoraDano.Calcular(Personagens.GuerreiroFogo(), Personagens.MagoAgua(), TipoGolpe.Basico);
        Assert.AreEqual(124, dano);
    }

    [Test]
    public void ChefeSoTemVantagemDeElemento()
    {
        // Boitata (Fogo) contra Tanque de Agua: so vale a vantagem de elemento do tanque
        // 150 x 1,2 x (1 - 0,005 x 85) x 0,7 = 72,45
        int dano = CalculadoraDano.Calcular(Personagens.Boitata(), Personagens.TanqueAgua(), TipoGolpe.Basico);
        Assert.AreEqual(72, dano);
    }

    [Test]
    public void PoderDosGolpes()
    {
        DadosPersonagem mago = Personagens.MagoFogo();
        DadosPersonagem tanque = Personagens.TanquePlanta();

        Assert.AreEqual(121, CalculadoraDano.Calcular(mago, tanque, TipoGolpe.Tecnica));  // 0,9
        Assert.AreEqual(296, CalculadoraDano.Calcular(mago, tanque, TipoGolpe.Especial)); // 2,2
    }
}

using NUnit.Framework;

public class EfetividadeTestes
{
    [Test]
    public void DuasVantagensESuperEfetivo()
    {
        // Agua vence Fogo e Guerreiro vence Mago
        int saldo = Efetividade.Saldo(Personagens.GuerreiroAgua(), Personagens.MagoFogo());

        Assert.AreEqual(2, saldo);
        Assert.AreEqual("Super efetivo!", Efetividade.Rotulo(saldo));
    }

    [Test]
    public void UmaVantagemEEfetivo()
    {
        // so a classe: Guerreiro vence Mago, mesmo elemento
        int saldo = Efetividade.Saldo(Personagens.GuerreiroFogo(), Personagens.MagoFogo());

        Assert.AreEqual(1, saldo);
        Assert.AreEqual("Efetivo!", Efetividade.Rotulo(saldo));
    }

    [Test]
    public void UmaDesvantagemEPoucoEfetivo()
    {
        int saldo = Efetividade.Saldo(Personagens.MagoFogo(), Personagens.GuerreiroFogo());

        Assert.AreEqual(-1, saldo);
        Assert.AreEqual("Pouco efetivo...", Efetividade.Rotulo(saldo));
    }

    [Test]
    public void DuasDesvantagensEMuitoPoucoEfetivo()
    {
        int saldo = Efetividade.Saldo(Personagens.MagoFogo(), Personagens.GuerreiroAgua());

        Assert.AreEqual(-2, saldo);
        Assert.AreEqual("Muito pouco efetivo...", Efetividade.Rotulo(saldo));
    }

    [Test]
    public void SemVantagensNaoTemAviso()
    {
        int saldo = Efetividade.Saldo(Personagens.GuerreiroFogo(), Personagens.GuerreiroFogo());

        Assert.AreEqual(0, saldo);
        Assert.IsNull(Efetividade.Rotulo(saldo));
    }

    [Test]
    public void UmaVantagemDeCadaLadoNaoTemAviso()
    {
        // Agua vence Fogo, mas Guerreiro vence Mago: 1,3 x 0,7 = 0,91 do dano
        int saldo = Efetividade.Saldo(Personagens.MagoAgua(), Personagens.GuerreiroFogo());

        Assert.AreEqual(0, saldo);
        Assert.IsNull(Efetividade.Rotulo(saldo));
    }

    [Test]
    public void BatalhaGuardaOUltimoAtaqueEAvisaNoRegistro()
    {
        // Guerreiro de Agua (vel 90) ataca o Mago de Fogo: super efetivo
        var batalha = new Batalha(
            new Equipe(Personagens.GuerreiroAgua(), Personagens.TanqueAgua(), Personagens.TanquePlanta()),
            new Equipe(Personagens.MagoFogo(), Personagens.TanqueFogo(), Personagens.GuerreiroFogo()));
        Lutador alvo = batalha.Equipes[1].Ativo;

        batalha.Atacar(TipoGolpe.Basico);

        ResultadoAtaque r = batalha.UltimoAtaque;
        Assert.AreEqual(0, r.Atacante);
        Assert.AreEqual(TipoGolpe.Basico, r.Golpe);
        Assert.AreEqual(alvo.Dados.vida - alvo.VidaAtual, r.Dano);
        Assert.AreEqual(2, r.Saldo);
        Assert.IsFalse(r.Derrotou);
        StringAssert.EndsWith("Super efetivo!", batalha.Registro[batalha.Registro.Count - 1]);
    }
}

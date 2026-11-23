using System.Collections.Generic;
using UnityEngine;

// Regras da luta, sem nada de tela. A cena chama Atacar, Trocar e EscolherSubstituto.
//
// Como funciona uma rodada:
// - o personagem ativo mais rapido age primeiro, depois o outro
// - cada jogador ataca, usa um item ou troca de personagem (item e troca gastam a vez)
// - se um personagem cai, o dono escolhe quem entra (de graca) e, se ele ainda
//   nao tinha agido, perde a vez naquela rodada
public class Batalha
{
    public const int STAMINA_AO_CAUSAR_DANO = 20;
    public const int STAMINA_AO_SOFRER_DANO = 15;

    public Equipe[] Equipes = new Equipe[2];
    public int JogadorDaVez { get; private set; }
    public int Rodada { get; private set; }
    public int Vencedor { get; private set; }

    // mensagens da luta, para mostrar na tela
    public List<string> Registro = new List<string>();

    // o ultimo golpe que acertou (dano, efetividade, se derrubou o alvo)
    public ResultadoAtaque UltimoAtaque { get; private set; }

    // o ultimo item usado (quem usou, qual, quanto curou)
    public ResultadoItem UltimoItem { get; private set; }

    bool[] jaAgiu = new bool[2];

    public bool Terminou
    {
        get { return Vencedor != -1; }
    }

    public Batalha(Equipe jogador1, Equipe jogador2)
    {
        Equipes[0] = jogador1;
        Equipes[1] = jogador2;
        Vencedor = -1;
        ComecarRodada();
    }

    public bool PrecisaDeSubstituto(int jogador)
    {
        return !Terminou && !Equipes[jogador].Ativo.EstaVivo;
    }

    // enquanto alguem precisa escolher substituto, ninguem ataca
    public bool AguardandoSubstituto()
    {
        return PrecisaDeSubstituto(0) || PrecisaDeSubstituto(1);
    }

    public bool PodeAgir()
    {
        return !Terminou && !AguardandoSubstituto();
    }

    public bool Atacar(TipoGolpe golpe)
    {
        if (!PodeAgir())
            return false;

        Equipe atacante = Equipes[JogadorDaVez];
        Equipe defensor = Equipes[Outro(JogadorDaVez)];

        if (golpe == TipoGolpe.Especial && !atacante.EspecialDisponivel)
            return false;

        int dano = CalculadoraDano.Calcular(atacante.Ativo, defensor.Ativo, golpe, Rodada);
        int saldo = Efetividade.Saldo(atacante.Ativo.Dados, defensor.Ativo.Dados);
        defensor.Ativo.ReceberDano(dano);

        string aviso = Efetividade.Rotulo(saldo);
        Registro.Add(atacante.Ativo.Dados.nome + " usou " + atacante.Ativo.Dados.NomeDoGolpe(golpe)
            + " e causou " + dano + " de dano." + (aviso != null ? " " + aviso : ""));

        UltimoAtaque = new ResultadoAtaque
        {
            Atacante = JogadorDaVez,
            Golpe = golpe,
            Dano = dano,
            Saldo = saldo,
            Derrotou = !defensor.Ativo.EstaVivo,
        };

        // usar o especial esvazia a barra
        if (golpe == TipoGolpe.Especial)
            atacante.ZerarStamina();
        else
            atacante.GanharStamina(STAMINA_AO_CAUSAR_DANO);
        defensor.GanharStamina(STAMINA_AO_SOFRER_DANO);

        if (!defensor.Ativo.EstaVivo)
        {
            Registro.Add(defensor.Ativo.Dados.nome + " foi derrotado!");

            if (!defensor.TemAlguemVivo())
            {
                Vencedor = JogadorDaVez;
                Registro.Add("Jogador " + (Vencedor + 1) + " venceu!");
                return true;
            }

            // quem caiu perde a vez nesta rodada
            jaAgiu[Outro(JogadorDaVez)] = true;
        }

        FimDaAcao();
        return true;
    }

    // pode usar o item agora? (o Kit Medico nao serve com a vida cheia)
    public bool PodeUsarItem(TipoItem tipo)
    {
        if (!PodeAgir() || !Itens.Disponivel(tipo))
            return false;

        Equipe equipe = Equipes[JogadorDaVez];
        if (!equipe.Mochila.Tem(tipo))
            return false;
        return tipo != TipoItem.KitMedico || !equipe.Ativo.VidaCheia;
    }

    // usar um item gasta a vez e nao carrega a stamina
    public bool UsarItem(TipoItem tipo)
    {
        if (!PodeUsarItem(tipo))
            return false;

        Equipe equipe = Equipes[JogadorDaVez];
        Lutador ativo = equipe.Ativo;
        equipe.Mochila.Tirar(tipo);

        int cura = 0;
        if (tipo == TipoItem.KitMedico)
        {
            cura = ativo.Curar(UnityEngine.Mathf.RoundToInt(ativo.VidaMaxima * Itens.CURA_KIT_MEDICO));
            Registro.Add(ativo.Dados.nome + " usou " + Itens.Nome(tipo) + " e recuperou " + cura + " de vida.");
        }
        else
        {
            // vale nesta rodada e nas 3 seguintes
            ativo.TomarPocao(tipo, Rodada + Itens.RODADAS_POCAO);
            Registro.Add(ativo.Dados.nome + " tomou " + Itens.Nome(tipo) + ".");
        }

        UltimoItem = new ResultadoItem { Jogador = JogadorDaVez, Tipo = tipo, Cura = cura };

        FimDaAcao();
        return true;
    }

    public bool Trocar(int indice)
    {
        if (!PodeAgir())
            return false;

        Equipe equipe = Equipes[JogadorDaVez];
        if (!equipe.PodeTrocarPara(indice))
            return false;

        string saiu = equipe.Ativo.Dados.nome;
        equipe.TrocarAtivo(indice);
        Registro.Add(saiu + " saiu e " + equipe.Ativo.Dados.nome + " entrou.");

        FimDaAcao();
        return true;
    }

    public bool EscolherSubstituto(int jogador, int indice)
    {
        if (!PrecisaDeSubstituto(jogador))
            return false;

        Equipe equipe = Equipes[jogador];
        if (!equipe.PodeTrocarPara(indice))
            return false;

        equipe.TrocarAtivo(indice);
        Registro.Add(equipe.Ativo.Dados.nome + " entrou na arena.");

        // a rodada ja tinha acabado, so faltava o substituto
        if (!AguardandoSubstituto())
            ComecarRodada();
        return true;
    }

    void FimDaAcao()
    {
        jaAgiu[JogadorDaVez] = true;

        if (jaAgiu[0] && jaAgiu[1])
        {
            // se alguem caiu, a proxima rodada so comeca depois da escolha do substituto
            if (!AguardandoSubstituto())
                ComecarRodada();
        }
        else
        {
            JogadorDaVez = Outro(JogadorDaVez);
        }
    }

    void ComecarRodada()
    {
        Rodada++;
        jaAgiu[0] = false;
        jaAgiu[1] = false;
        JogadorDaVez = QuemComeca();
    }

    int QuemComeca()
    {
        int vel1 = Equipes[0].Ativo.Atributos.velocidade;
        int vel2 = Equipes[1].Ativo.Atributos.velocidade;

        if (vel1 > vel2) return 0;
        if (vel2 > vel1) return 1;
        return Random.Range(0, 2); // empate: sorteia
    }

    int Outro(int jogador)
    {
        return 1 - jogador;
    }
}

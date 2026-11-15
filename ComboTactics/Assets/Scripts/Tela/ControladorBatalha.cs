using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Liga a cena com a classe Batalha: le os botoes e atualiza a tela.
// As regras ficam todas na Batalha.
public class ControladorBatalha : MonoBehaviour
{
    [Header("Equipes (3 personagens cada)")]
    public DadosPersonagem[] equipeJogador1 = new DadosPersonagem[3];
    public DadosPersonagem[] equipeJogador2 = new DadosPersonagem[3];

    [Header("Arena")]
    public PersonagemNaArena personagemJogador1;
    public PersonagemNaArena personagemJogador2;

    [Header("Paineis de cima")]
    public PainelJogador painelJogador1;
    public PainelJogador painelJogador2;

    [Header("Acoes")]
    public TMP_Text textoVez;
    public Button[] botoesGolpe = new Button[3]; // basico, tecnica, especial
    public Button[] botoesTroca = new Button[3];
    public TMP_Text textoMensagens;

    Batalha batalha;

    void Start()
    {
        if (!EquipeCompleta(equipeJogador1) || !EquipeCompleta(equipeJogador2))
        {
            Debug.LogError("Coloque 3 personagens em cada equipe no Inspector do ControladorBatalha.");
            return;
        }

        batalha = new Batalha(new Equipe(equipeJogador1), new Equipe(equipeJogador2));

        botoesGolpe[0].onClick.AddListener(() => UsarGolpe(TipoGolpe.Basico));
        botoesGolpe[1].onClick.AddListener(() => UsarGolpe(TipoGolpe.Tecnica));
        botoesGolpe[2].onClick.AddListener(() => UsarGolpe(TipoGolpe.Especial));

        for (int i = 0; i < botoesTroca.Length; i++)
        {
            int indice = i; // copia para o lambda nao pegar o valor final do i
            botoesTroca[i].onClick.AddListener(() => TrocarPersonagem(indice));
        }

        personagemJogador1.Mostrar(batalha.Equipes[0].Ativo.Dados);
        personagemJogador2.Mostrar(batalha.Equipes[1].Ativo.Dados);
        AtualizarTela();
    }

    bool EquipeCompleta(DadosPersonagem[] equipe)
    {
        if (equipe == null || equipe.Length != 3)
            return false;
        foreach (DadosPersonagem p in equipe)
        {
            if (p == null)
                return false;
        }
        return true;
    }

    PersonagemNaArena Arena(int jogador)
    {
        return jogador == 0 ? personagemJogador1 : personagemJogador2;
    }

    void UsarGolpe(TipoGolpe golpe)
    {
        int atacante = batalha.JogadorDaVez;
        int defensor = 1 - atacante;

        if (!batalha.Atacar(golpe))
            return;

        Arena(atacante).Atacar();
        if (batalha.Equipes[defensor].Ativo.EstaVivo)
            Arena(defensor).LevarDano();
        else
            Arena(defensor).Derrotado();

        AtualizarTela();
    }

    void TrocarPersonagem(int indice)
    {
        // depois de uma derrota, o botao de troca serve para escolher quem entra
        if (batalha.AguardandoSubstituto())
        {
            int jogador = batalha.PrecisaDeSubstituto(0) ? 0 : 1;
            if (batalha.EscolherSubstituto(jogador, indice))
                Arena(jogador).Mostrar(batalha.Equipes[jogador].Ativo.Dados);
        }
        else
        {
            int jogador = batalha.JogadorDaVez;
            if (batalha.Trocar(indice))
                Arena(jogador).Mostrar(batalha.Equipes[jogador].Ativo.Dados);
        }

        AtualizarTela();
    }

    void AtualizarTela()
    {
        painelJogador1.Atualizar(batalha.Equipes[0]);
        painelJogador2.Atualizar(batalha.Equipes[1]);
        MostrarUltimasMensagens(4);

        if (batalha.Terminou)
        {
            textoVez.text = "Jogador " + (batalha.Vencedor + 1) + " venceu!";
            foreach (Button b in botoesGolpe) b.interactable = false;
            foreach (Button b in botoesTroca) b.interactable = false;
            return;
        }

        int jogador;
        bool escolhendoSubstituto = batalha.AguardandoSubstituto();
        if (escolhendoSubstituto)
        {
            jogador = batalha.PrecisaDeSubstituto(0) ? 0 : 1;
            textoVez.text = "Jogador " + (jogador + 1) + ": escolha quem entra";
        }
        else
        {
            jogador = batalha.JogadorDaVez;
            textoVez.text = "Vez do Jogador " + (jogador + 1);
        }

        Equipe equipe = batalha.Equipes[jogador];

        // golpes do personagem ativo
        DadosPersonagem ativo = equipe.Ativo.Dados;
        MudarTexto(botoesGolpe[0], ativo.golpeBasico);
        MudarTexto(botoesGolpe[1], ativo.tecnica);
        MudarTexto(botoesGolpe[2], ativo.especial);
        botoesGolpe[0].interactable = !escolhendoSubstituto;
        botoesGolpe[1].interactable = !escolhendoSubstituto;
        botoesGolpe[2].interactable = !escolhendoSubstituto && equipe.EspecialDisponivel;

        // um botao para cada personagem da equipe
        for (int i = 0; i < botoesTroca.Length; i++)
        {
            Lutador l = equipe.Lutadores[i];
            string texto = l.Dados.nome;
            if (!l.EstaVivo) texto += " (derrotado)";
            else if (i == equipe.IndiceAtivo) texto += " (na arena)";

            MudarTexto(botoesTroca[i], texto);
            botoesTroca[i].interactable = equipe.PodeTrocarPara(i);
        }
    }

    void MostrarUltimasMensagens(int quantidade)
    {
        List<string> registro = batalha.Registro;
        int inicio = Mathf.Max(0, registro.Count - quantidade);
        textoMensagens.text = string.Join("\n", registro.GetRange(inicio, registro.Count - inicio));
    }

    void MudarTexto(Button botao, string texto)
    {
        TMP_Text t = botao.GetComponentInChildren<TMP_Text>();
        if (t != null)
            t.text = texto;
    }
}

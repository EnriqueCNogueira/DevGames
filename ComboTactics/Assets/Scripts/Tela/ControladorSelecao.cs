using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Liga a tela de Selecao com a SelecaoPartida. As regras ficam na SelecaoPartida.
public class ControladorSelecao : MonoBehaviour
{
    [Header("Elenco, na ordem da grade")]
    public DadosPersonagem[] elenco = new DadosPersonagem[9];
    public CartaoPersonagem[] cartoes = new CartaoPersonagem[9];
    public IconesTipo icones;

    [Header("Equipes")]
    public PainelEquipe[] paineis = new PainelEquipe[2];

    [Header("Topo e botoes")]
    public TMP_Text textoVez;
    public Button botaoConfirmar;
    public Button botaoVoltar;
    public Color[] coresJogador = { new Color(0.93f, 0.52f, 0.14f), new Color(0.27f, 0.65f, 0.78f) };

    SelecaoPartida selecao;

    void Start()
    {
        selecao = new SelecaoPartida();

        // as barras usam o maior valor do elenco como barra cheia
        int[] maximos = new int[4];
        foreach (DadosPersonagem p in elenco)
        {
            int[] valores = Atributos(p);
            for (int i = 0; i < 4; i++)
                maximos[i] = Mathf.Max(maximos[i], valores[i]);
        }

        for (int i = 0; i < cartoes.Length; i++)
        {
            DadosPersonagem p = elenco[i];
            int[] valores = Atributos(p);
            float[] proporcoes = new float[4];
            for (int a = 0; a < 4; a++)
                proporcoes[a] = maximos[a] > 0 ? (float)valores[a] / maximos[a] : 0f;

            cartoes[i].Mostrar(p, icones, proporcoes);
            cartoes[i].botao.onClick.AddListener(() => Escolher(p));
        }

        for (int j = 0; j < paineis.Length; j++)
        {
            int jogador = j;
            foreach (SlotEquipe slot in paineis[j].slots)
            {
                int indice = slot.indice;
                slot.botao.onClick.AddListener(() => Remover(jogador, indice));
            }
        }

        botaoConfirmar.onClick.AddListener(Confirmar);
        botaoVoltar.onClick.AddListener(Voltar);
        Atualizar();
    }

    static int[] Atributos(DadosPersonagem p)
    {
        return new[] { p.vida, p.ataque, p.defesa, p.velocidade };
    }

    void Escolher(DadosPersonagem personagem)
    {
        selecao.Escolher(personagem);
        Atualizar();
    }

    void Remover(int jogador, int indice)
    {
        if (jogador == selecao.JogadorDaVez)
            selecao.Remover(indice);
        Atualizar();
    }

    void Confirmar()
    {
        if (!selecao.Confirmar())
            return;

        if (selecao.Pronta)
        {
            PartidaAtual.Guardar(selecao.Equipes[0].ParaArray(), selecao.Equipes[1].ParaArray());
            SceneManager.LoadScene(Cenas.BATALHA);
            return;
        }
        Atualizar();
    }

    void Voltar()
    {
        if (selecao.Voltar())
        {
            SceneManager.LoadScene(Cenas.MENU);
            return;
        }
        Atualizar();
    }

    void Atualizar()
    {
        int vez = selecao.JogadorDaVez;
        SelecaoEquipe equipe = selecao.EquipeDaVez;

        textoVez.text = "Jogador " + (vez + 1) + ", escolha 3 personagens";
        textoVez.color = coresJogador[vez];

        for (int i = 0; i < cartoes.Length; i++)
        {
            DadosPersonagem p = cartoes[i].Dados;
            cartoes[i].MostrarSelos(selecao.Equipes[0].Contem(p), selecao.Equipes[1].Contem(p));
            // quem ja esta na equipe da vez nao pode ser escolhido de novo
            cartoes[i].botao.interactable = !equipe.Contem(p);
        }

        for (int j = 0; j < paineis.Length; j++)
        {
            bool escolhendo = j == vez;
            bool confirmada = j < vez; // o Jogador 1 ja confirmou quando e a vez do 2
            paineis[j].Mostrar(selecao.Equipes[j], escolhendo, confirmada);
        }

        botaoConfirmar.interactable = selecao.PodeConfirmar;
    }
}

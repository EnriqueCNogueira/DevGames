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

    // Depois de confirmar a equipe, cada jogador sempre passa pela escolha de
    // ate 3 itens do estoque do perfil, mesmo que nao tenha nada para levar.
    [Header("Itens para a batalha")]
    public GameObject painelItens;
    public TMP_Text textoItens;
    public TMP_Text textoDicaItens; // regra do estoque, ou o aviso de estoque vazio
    public BotaoItem[] botoesItem = new BotaoItem[4]; // na ordem de Itens.TODOS
    public Button[] vagasItem = new Button[3];        // clicar tira o item da mochila
    public Image[] iconesVagaItem = new Image[3];
    public Button botaoConfirmarItens;
    public Button botaoVoltarItens;

    SelecaoPartida selecao;
    Perfil perfil;
    Mochila[] mochilas = new Mochila[2];
    SelecaoItens selecaoItens; // != null enquanto a escolha de itens esta aberta

    void Start()
    {
        selecao = new SelecaoPartida();
        perfil = PerfilSalvo.Atual;

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

        foreach (BotaoItem b in botoesItem)
        {
            TipoItem tipo = b.tipo;
            b.botao.onClick.AddListener(() => AdicionarItem(tipo));
        }
        for (int i = 0; i < vagasItem.Length; i++)
        {
            int vaga = i;
            vagasItem[i].onClick.AddListener(() => TirarItem(vaga));
        }
        botaoConfirmarItens.onClick.AddListener(ConfirmarItens);
        botaoVoltarItens.onClick.AddListener(FecharItens);
        painelItens.SetActive(false);

        Atualizar();
    }

    // atributos com as evolucoes do perfil (valem para os dois jogadores)
    int[] Atributos(DadosPersonagem p)
    {
        Atributos a = perfil.AtributosDe(p);
        return new[] { a.vida, a.ataque, a.defesa, a.velocidade };
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
        if (!selecao.PodeConfirmar)
            return;

        // o Jogador 1 escolhe primeiro; o Jogador 2 so ve o que sobrou no estoque
        int vez = selecao.JogadorDaVez;
        Mochila doOutro = vez == 1 ? mochilas[0] : null;
        selecaoItens = new SelecaoItens(perfil.estoque, doOutro, mochilas[vez]);
        painelItens.SetActive(true);
        AtualizarItens();
    }

    void AdicionarItem(TipoItem tipo)
    {
        if (selecaoItens != null && selecaoItens.Adicionar(tipo))
            AtualizarItens();
    }

    // a vaga mostra os itens na ordem do catalogo; tira um do tipo daquela vaga
    void TirarItem(int vaga)
    {
        if (selecaoItens == null)
            return;
        TipoItem? tipo = ItemNaVaga(selecaoItens.Mochila, vaga);
        if (tipo.HasValue && selecaoItens.Tirar(tipo.Value))
            AtualizarItens();
    }

    static TipoItem? ItemNaVaga(Mochila mochila, int vaga)
    {
        int contador = 0;
        foreach (TipoItem tipo in Itens.TODOS)
        {
            int q = mochila.Quantidade(tipo);
            if (vaga < contador + q)
                return tipo;
            contador += q;
        }
        return null;
    }

    void ConfirmarItens()
    {
        int vez = selecao.JogadorDaVez;
        mochilas[vez] = selecaoItens.Mochila.Copia();
        selecaoItens = null;
        painelItens.SetActive(false);

        if (!selecao.Confirmar())
            return;

        if (selecao.Pronta)
        {
            PartidaAtual.Guardar(selecao.Equipes[0].ParaArray(), selecao.Equipes[1].ParaArray());
            PartidaAtual.GuardarMochilas(mochilas[0], mochilas[1]);
            SceneManager.LoadScene(Cenas.BATALHA);
            return;
        }
        Atualizar();
    }

    // volta para a equipe guardando o que ja foi separado
    void FecharItens()
    {
        if (selecaoItens != null)
            mochilas[selecao.JogadorDaVez] = selecaoItens.Mochila.Copia();
        selecaoItens = null;
        painelItens.SetActive(false);
        Atualizar();
    }

    void AtualizarItens()
    {
        int vez = selecao.JogadorDaVez;
        Mochila mochila = selecaoItens.Mochila;

        textoItens.text = "Jogador " + (vez + 1) + ", escolha até " + Itens.POR_BATALHA + " itens";
        textoItens.color = coresJogador[vez];
        textoDicaItens.text = selecaoItens.TemAlgoParaEscolher
            ? "Os itens só saem do estoque quando forem usados na batalha."
            : "Sem itens disponíveis no estoque. Compre na Loja, pelo Menu.";

        foreach (BotaoItem b in botoesItem)
        {
            string detalhe = !Itens.Disponivel(b.tipo) ? "Em breve"
                : "Disponível: " + selecaoItens.Disponivel(b.tipo);
            b.Mostrar(Itens.Nome(b.tipo), detalhe, selecaoItens.PodeAdicionar(b.tipo));
        }

        for (int i = 0; i < vagasItem.Length; i++)
        {
            TipoItem? tipo = ItemNaVaga(mochila, i);
            iconesVagaItem[i].enabled = tipo.HasValue;
            if (tipo.HasValue)
                iconesVagaItem[i].sprite = botoesItem[(int)tipo.Value].icone.sprite;
            vagasItem[i].interactable = tipo.HasValue;
        }
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

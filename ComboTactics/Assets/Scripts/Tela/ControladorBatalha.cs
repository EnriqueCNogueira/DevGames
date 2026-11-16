using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Liga a cena com a classe Batalha: le os botoes e atualiza a tela.
// As regras ficam todas na Batalha.
public class ControladorBatalha : MonoBehaviour
{
    // usadas so quando a cena abre direto no editor, sem passar pela Selecao
    [Header("Equipes de teste (3 personagens cada)")]
    public DadosPersonagem[] equipeJogador1 = new DadosPersonagem[3];
    public DadosPersonagem[] equipeJogador2 = new DadosPersonagem[3];

    [Header("Arena")]
    public PersonagemNaArena personagemJogador1;
    public PersonagemNaArena personagemJogador2;
    public IconesTipo icones;
    public DadosPersonagem[] elenco; // os 9 personagens: o maior atributo enche a barra

    [Header("Paineis de cima")]
    public PainelJogador painelJogador1;
    public PainelJogador painelJogador2;
    public TMP_Text textoRodada;

    [Header("Menu de acoes")]
    public RectTransform painelMenu;
    public TMP_Text textoVez;
    public GameObject grupoPrincipal;
    public GameObject grupoHabilidade;
    public GameObject grupoTrocar;
    public Button botaoHabilidade;
    public Button botaoItens;
    public Button botaoTrocar;
    public Button botaoVoltar;
    public Vector2 tamanhoPrincipal = new Vector2(720, 216);
    public Vector2 tamanhoHabilidade = new Vector2(720, 420);
    public Vector2 tamanhoTrocar = new Vector2(1002, 444);
    public BotaoAcao[] acoesGolpe = new BotaoAcao[3]; // basico, tecnica, especial
    public CartaoTroca[] cartoesTroca = new CartaoTroca[3];

    [Header("Registro")]
    public Button botaoRegistro;
    public GameObject painelRegistro;
    public TMP_Text textoMensagens;
    public int linhasNoRegistro = 8;

    [Header("Destaque e numeros")]
    public RectTransform areaCanvas;
    public RectTransform indicadorVez;       // seta sobre quem joga
    public TMP_Text modeloTextoFlutuante;    // copiado para cada dano e aviso
    public Color[] coresJogador = { new Color(0.93f, 0.52f, 0.14f), new Color(0.27f, 0.65f, 0.78f) };

    [Header("Sair no meio da luta")]
    public Button botaoMenu;
    public GameObject confirmacaoMenu;
    public Button botaoConfirmarSaida;
    public Button botaoCancelarSaida;

    [Header("Resultado")]
    public GameObject painelResultado;
    public TMP_Text textoVencedor;
    public AnimacaoUI[] retratosVencedor = new AnimacaoUI[3];
    public Button botaoRevanche;
    public Button botaoTrocarPersonagens;
    public Button botaoMenuResultado;

    enum Submenu { Principal, Habilidade, Trocar }

    // altura da cabeca acima do pe, em unidades do mundo (sprite de 128 texels, PPU 96)
    const float ALTURA_CABECA = 1.2f;

    static readonly string[] NOME_DO_TIPO = { "Básico", "Técnica", "Especial" };

    Batalha batalha;
    bool animando; // enquanto o golpe aparece na tela, os botoes ficam travados
    Submenu submenu = Submenu.Principal;
    int[] maximos = new int[3]; // maior ataque, defesa e velocidade do elenco

    void Start()
    {
        DadosPersonagem[] equipe1 = equipeJogador1;
        DadosPersonagem[] equipe2 = equipeJogador2;
        // vindo da Selecao, as equipes escolhidas tem prioridade sobre as de teste
        if (PartidaAtual.TemEquipes)
        {
            equipe1 = PartidaAtual.Equipe(0);
            equipe2 = PartidaAtual.Equipe(1);
        }

        if (!EquipeCompleta(equipe1) || !EquipeCompleta(equipe2))
        {
            Debug.LogError("Coloque 3 personagens em cada equipe no Inspector do ControladorBatalha.");
            return;
        }

        batalha = new Batalha(new Equipe(equipe1), new Equipe(equipe2));

        foreach (DadosPersonagem p in elenco)
        {
            maximos[0] = Mathf.Max(maximos[0], p.ataque);
            maximos[1] = Mathf.Max(maximos[1], p.defesa);
            maximos[2] = Mathf.Max(maximos[2], p.velocidade);
        }

        botaoHabilidade.onClick.AddListener(() => AbrirSubmenu(Submenu.Habilidade));
        botaoTrocar.onClick.AddListener(() => AbrirSubmenu(Submenu.Trocar));
        botaoVoltar.onClick.AddListener(() => AbrirSubmenu(Submenu.Principal));
        acoesGolpe[0].botao.onClick.AddListener(() => UsarGolpe(TipoGolpe.Basico));
        acoesGolpe[1].botao.onClick.AddListener(() => UsarGolpe(TipoGolpe.Tecnica));
        acoesGolpe[2].botao.onClick.AddListener(() => UsarGolpe(TipoGolpe.Especial));
        for (int i = 0; i < cartoesTroca.Length; i++)
        {
            int indice = i; // copia para o lambda nao pegar o valor final do i
            cartoesTroca[i].botao.onClick.AddListener(() => TrocarPersonagem(indice));
        }

        botaoRegistro.onClick.AddListener(() => painelRegistro.SetActive(!painelRegistro.activeSelf));
        botaoMenu.onClick.AddListener(() => confirmacaoMenu.SetActive(true));
        botaoCancelarSaida.onClick.AddListener(() => confirmacaoMenu.SetActive(false));
        botaoConfirmarSaida.onClick.AddListener(() => SceneManager.LoadScene(Cenas.MENU));
        // revanche: recarrega a cena, que monta uma Batalha nova com as mesmas equipes
        botaoRevanche.onClick.AddListener(() => SceneManager.LoadScene(Cenas.BATALHA));
        botaoTrocarPersonagens.onClick.AddListener(() => SceneManager.LoadScene(Cenas.SELECAO));
        botaoMenuResultado.onClick.AddListener(() => SceneManager.LoadScene(Cenas.MENU));

        painelRegistro.SetActive(false);
        confirmacaoMenu.SetActive(false);
        painelResultado.SetActive(false);
        modeloTextoFlutuante.gameObject.SetActive(false);

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

    // quem precisa agir agora: o dono do derrotado ou o jogador da vez
    int JogadorAgindo()
    {
        if (batalha.AguardandoSubstituto())
            return batalha.PrecisaDeSubstituto(0) ? 0 : 1;
        return batalha.JogadorDaVez;
    }

    void AbrirSubmenu(Submenu novo)
    {
        if (animando)
            return;
        submenu = novo;
        AtualizarTela();
    }

    void UsarGolpe(TipoGolpe golpe)
    {
        if (animando || batalha == null)
            return;

        if (!batalha.Atacar(golpe))
            return;

        submenu = Submenu.Principal;
        StartCoroutine(MostrarGolpe(batalha.UltimoAtaque));
        AtualizarTela(); // fecha o submenu ja travado enquanto o golpe anima
    }

    // A regra ja foi aplicada pela Batalha. Aqui so mostra na tela:
    // o atacante comeca o golpe, e o alvo reage quando o golpe "acerta".
    IEnumerator MostrarGolpe(ResultadoAtaque resultado)
    {
        int atacante = resultado.Atacante;
        int defensor = 1 - atacante;
        bool especial = resultado.Golpe == TipoGolpe.Especial;

        animando = true;
        TravarBotoes();
        Arena(atacante).Atacar(especial);

        yield return new WaitForSeconds(especial ? 0.5f : 0.3f);

        if (resultado.Derrotou)
            Arena(defensor).Derrotado();
        else
            Arena(defensor).LevarDano();

        // o numero nasce na altura da cabeca e sobe pouco, para nao entrar no painel de cima
        Vector2 cabeca = PontoNoCanvas(Arena(defensor).transform.position + Vector3.up * (ALTURA_CABECA - 0.2f));
        StartCoroutine(Flutuar("-" + resultado.Dano, cabeca, 64, new Color(1f, 0.42f, 0.29f), 0f));
        string aviso = Efetividade.Rotulo(resultado.Saldo);
        if (aviso != null)
        {
            Color cor = resultado.Saldo > 0 ? new Color(0.97f, 0.75f, 0.38f) : new Color(0.6f, 0.65f, 0.75f);
            StartCoroutine(Flutuar(aviso, cabeca + new Vector2(0, 60), 40, cor, 0.1f));
        }
        AtualizarTela(); // a barra de vida desce na hora do acerto

        yield return new WaitForSeconds(0.5f);
        animando = false;
        AtualizarTela();

        if (batalha.Terminou)
            StartCoroutine(MostrarResultado());
    }

    void TravarBotoes()
    {
        botaoHabilidade.interactable = false;
        botaoItens.interactable = false;
        botaoTrocar.interactable = false;
        botaoVoltar.interactable = false;
        foreach (BotaoAcao b in acoesGolpe) b.botao.interactable = false;
        foreach (CartaoTroca c in cartoesTroca) c.botao.interactable = false;
    }

    void TrocarPersonagem(int indice)
    {
        if (animando || batalha == null)
            return;

        // depois de uma derrota, a troca serve para escolher quem entra
        int jogador = JogadorAgindo();
        bool feito = batalha.AguardandoSubstituto()
            ? batalha.EscolherSubstituto(jogador, indice)
            : batalha.Trocar(indice);

        if (feito)
        {
            Arena(jogador).Mostrar(batalha.Equipes[jogador].Ativo.Dados);
            submenu = Submenu.Principal;
        }
        AtualizarTela();
    }

    void AtualizarTela()
    {
        int agindo = batalha.Terminou ? -1 : JogadorAgindo();
        painelJogador1.Atualizar(batalha.Equipes[0], agindo == 0, icones);
        painelJogador2.Atualizar(batalha.Equipes[1], agindo == 1, icones);
        textoRodada.text = "Rodada " + batalha.Rodada;
        MostrarUltimasMensagens(linhasNoRegistro);

        if (batalha.Terminou)
        {
            textoVez.text = "Jogador " + (batalha.Vencedor + 1) + " venceu!";
            textoVez.color = coresJogador[batalha.Vencedor];
            MostrarSubmenu(Submenu.Principal, false);
            TravarBotoes();
            return;
        }

        // na escolha de substituto o Trocar abre direto, sem Voltar
        bool escolhendoSubstituto = batalha.AguardandoSubstituto();
        Submenu visivel = escolhendoSubstituto ? Submenu.Trocar : submenu;
        MostrarSubmenu(visivel, !escolhendoSubstituto && visivel != Submenu.Principal);

        textoVez.text = escolhendoSubstituto
            ? "Jogador " + (agindo + 1) + ", escolha quem entra"
            : "Vez do Jogador " + (agindo + 1);
        textoVez.color = coresJogador[agindo];

        Equipe equipe = batalha.Equipes[agindo];

        botaoHabilidade.interactable = true;
        botaoItens.interactable = false; // itens chegam no Sprint 3
        bool podeTrocar = false;
        for (int i = 0; i < equipe.Lutadores.Count; i++)
            podeTrocar |= equipe.PodeTrocarPara(i);
        botaoTrocar.interactable = podeTrocar;
        botaoVoltar.interactable = true;

        // golpes do personagem ativo, com o tipo embaixo do nome
        DadosPersonagem ativo = equipe.Ativo.Dados;
        TipoGolpe[] tipos = { TipoGolpe.Basico, TipoGolpe.Tecnica, TipoGolpe.Especial };
        for (int i = 0; i < 3; i++)
        {
            bool pode = !escolhendoSubstituto;
            string detalhe = NOME_DO_TIPO[i];
            if (tipos[i] == TipoGolpe.Especial)
            {
                pode = pode && equipe.EspecialDisponivel;
                if (!equipe.EspecialDisponivel)
                    detalhe = "Especial (carregando)";
            }
            acoesGolpe[i].Mostrar(ativo.NomeDoGolpe(tipos[i]), detalhe, pode);
        }

        // a equipe de quem age, com vida e atributos; so troca para vivo fora da arena
        for (int i = 0; i < cartoesTroca.Length; i++)
        {
            Lutador l = equipe.Lutadores[i];
            string estado = null;
            if (!l.EstaVivo) estado = "Derrotado";
            else if (i == equipe.IndiceAtivo) estado = "Na arena";
            cartoesTroca[i].Mostrar(l, maximos, icones, estado, equipe.PodeTrocarPara(i));
        }

        if (animando)
            TravarBotoes();
    }

    void MostrarSubmenu(Submenu qual, bool comVoltar)
    {
        grupoPrincipal.SetActive(qual == Submenu.Principal);
        grupoHabilidade.SetActive(qual == Submenu.Habilidade);
        grupoTrocar.SetActive(qual == Submenu.Trocar);
        botaoVoltar.gameObject.SetActive(comVoltar);
        painelMenu.sizeDelta = qual == Submenu.Principal ? tamanhoPrincipal
            : qual == Submenu.Habilidade ? tamanhoHabilidade : tamanhoTrocar;
    }

    void MostrarUltimasMensagens(int quantidade)
    {
        List<string> registro = batalha.Registro;
        int inicio = Mathf.Max(0, registro.Count - quantidade);
        textoMensagens.text = string.Join("\n", registro.GetRange(inicio, registro.Count - inicio));
    }

    void LateUpdate()
    {
        if (batalha == null)
            return;

        // seta balancando sobre a cabeca de quem joga
        bool mostrar = !batalha.Terminou && !animando && !confirmacaoMenu.activeSelf;
        indicadorVez.gameObject.SetActive(mostrar);
        if (!mostrar)
            return;

        Vector3 cabeca = Arena(JogadorAgindo()).transform.position + Vector3.up * (ALTURA_CABECA + 0.15f);
        float balanco = Mathf.Round(Mathf.Sin(Time.time * 5f) * 3f) * 3f; // passos de 1 texel
        indicadorVez.anchoredPosition = PontoNoCanvas(cabeca) + new Vector2(0, balanco);
    }

    // posicao na area da tela (origem no centro) de um ponto do mundo
    Vector2 PontoNoCanvas(Vector3 mundo)
    {
        Vector3 tela = Camera.main.WorldToScreenPoint(mundo);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(areaCanvas, tela, null, out Vector2 local);
        return local;
    }

    // numero ou aviso que sobe e some sobre o alvo
    IEnumerator Flutuar(string texto, Vector2 inicio, float tamanho, Color cor, float atraso)
    {
        if (atraso > 0f)
            yield return new WaitForSeconds(atraso);

        TMP_Text t = Instantiate(modeloTextoFlutuante, modeloTextoFlutuante.transform.parent);
        t.gameObject.SetActive(true);
        t.text = texto;
        t.fontSize = tamanho;
        RectTransform rt = t.rectTransform;

        const float duracao = 1.1f;
        for (float tempo = 0f; tempo < duracao; tempo += Time.deltaTime)
        {
            float p = tempo / duracao;
            rt.anchoredPosition = inicio + new Vector2(0, Mathf.Round(p * 60f / 3f) * 3f);
            t.color = new Color(cor.r, cor.g, cor.b, p < 0.6f ? 1f : 1f - (p - 0.6f) / 0.4f);
            yield return null;
        }
        Destroy(t.gameObject);
    }

    IEnumerator MostrarResultado()
    {
        // deixa a animacao de derrota terminar antes de cobrir a arena
        yield return new WaitForSeconds(1.2f);

        int vencedor = batalha.Vencedor;
        textoVencedor.text = "Vitória do Jogador " + (vencedor + 1) + "!";
        textoVencedor.color = coresJogador[vencedor];

        List<Lutador> lutadores = batalha.Equipes[vencedor].Lutadores;
        for (int i = 0; i < retratosVencedor.Length; i++)
        {
            Lutador l = lutadores[i];
            Image imagem = retratosVencedor[i].GetComponent<Image>();
            if (l.EstaVivo)
            {
                retratosVencedor[i].Tocar(l.Dados.spritesParado);
                imagem.color = Color.white;
            }
            else
            {
                // caido: fica parado e apagado
                retratosVencedor[i].Tocar(new[] { l.Dados.spritesParado[0] });
                imagem.color = new Color(0.35f, 0.35f, 0.4f, 0.8f);
            }
        }
        painelResultado.SetActive(true);
    }
}

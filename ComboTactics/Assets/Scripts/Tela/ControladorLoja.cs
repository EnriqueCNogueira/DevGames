using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Loja: compra de itens e evolucao de atributos com as moedas do perfil.
// As regras ficam no Perfil; aqui so liga os botoes e atualiza a tela.
public class ControladorLoja : MonoBehaviour
{
    [Header("Topo")]
    public TMP_Text textoSaldo;
    public Button botaoVoltar;

    [Header("Abas")]
    public Button abaItens;
    public Button abaEvolucoes;
    public GameObject conteudoItens;
    public GameObject conteudoEvolucoes;
    public Sprite abaNormal;
    public Sprite abaAtiva; // bisel invertido: a aba aberta parece apertada

    [Header("Itens")]
    public CartaoItemLoja[] cartoesItem = new CartaoItemLoja[4];

    [Header("Evolucoes")]
    public DadosPersonagem[] elenco = new DadosPersonagem[9];
    public CartaoPersonagem[] cartoes = new CartaoPersonagem[9];
    public IconesTipo icones;
    public Sprite cartaoNormal;
    public Sprite cartaoEscolhido;
    public TMP_Text textoPersonagem;
    public ColunaEvolucao[] colunas = new ColunaEvolucao[4];

    Perfil perfil;
    int escolhido;

    void Start()
    {
        perfil = PerfilSalvo.Atual;

        botaoVoltar.onClick.AddListener(() => SceneManager.LoadScene(Cenas.MENU));
        abaItens.onClick.AddListener(() => AbrirAba(true));
        abaEvolucoes.onClick.AddListener(() => AbrirAba(false));

        foreach (CartaoItemLoja c in cartoesItem)
        {
            TipoItem tipo = c.tipo;
            c.botaoComprar.onClick.AddListener(() => Comprar(tipo));
        }

        for (int i = 0; i < cartoes.Length; i++)
        {
            int indice = i;
            cartoes[i].botao.onClick.AddListener(() => Escolher(indice));
            cartoes[i].MostrarSelos(false, false);
        }

        foreach (ColunaEvolucao c in colunas)
        {
            Atributo atributo = c.atributo;
            c.botaoEvoluir.onClick.AddListener(() => Evoluir(atributo));
        }

        AbrirAba(true);
    }

    void AbrirAba(bool itens)
    {
        conteudoItens.SetActive(itens);
        conteudoEvolucoes.SetActive(!itens);
        abaItens.image.sprite = itens ? abaAtiva : abaNormal;
        abaEvolucoes.image.sprite = itens ? abaNormal : abaAtiva;
        Atualizar();
    }

    void Comprar(TipoItem tipo)
    {
        if (perfil.Comprar(tipo))
            PerfilSalvo.Salvar();
        Atualizar();
    }

    void Escolher(int indice)
    {
        escolhido = indice;
        Atualizar();
    }

    void Evoluir(Atributo atributo)
    {
        if (perfil.Evoluir(elenco[escolhido], atributo))
            PerfilSalvo.Salvar();
        Atualizar();
    }

    void Atualizar()
    {
        textoSaldo.text = perfil.moedas.ToString();

        foreach (CartaoItemLoja c in cartoesItem)
            c.Mostrar(perfil);

        // barras com os atributos ja evoluidos; o maior valor do elenco enche a barra
        int[] maximos = new int[Atributos.QUANTIDADE];
        foreach (DadosPersonagem p in elenco)
        {
            Atributos a = perfil.AtributosDe(p);
            for (int i = 0; i < maximos.Length; i++)
                maximos[i] = Mathf.Max(maximos[i], a.Valor((Atributo)i));
        }

        for (int i = 0; i < cartoes.Length; i++)
        {
            DadosPersonagem p = elenco[i];
            Atributos a = perfil.AtributosDe(p);
            float[] proporcoes = new float[maximos.Length];
            for (int j = 0; j < maximos.Length; j++)
                proporcoes[j] = maximos[j] > 0 ? (float)a.Valor((Atributo)j) / maximos[j] : 0f;

            // so reinicia a animacao quando o personagem do cartao muda
            if (cartoes[i].Dados != p)
                cartoes[i].Mostrar(p, icones, proporcoes);
            else
                cartoes[i].MostrarBarras(proporcoes);
            cartoes[i].botao.image.sprite = i == escolhido ? cartaoEscolhido : cartaoNormal;
        }

        DadosPersonagem personagem = elenco[escolhido];
        textoPersonagem.text = personagem.nome;
        foreach (ColunaEvolucao c in colunas)
            c.Mostrar(perfil, personagem);
    }
}

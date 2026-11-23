using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Um personagem na grade da Selecao: idle, nome, icones, barras e selos J1/J2
public class CartaoPersonagem : MonoBehaviour
{
    public Button botao;
    public AnimacaoUI retrato;
    public TMP_Text textoNome;
    public Image iconeClasse;
    public Image iconeElemento;
    public Image[] barras = new Image[4]; // vida, ataque, defesa, velocidade (preenchimento)
    public GameObject seloJogador1;
    public GameObject seloJogador2;

    public DadosPersonagem Dados { get; private set; }

    // proporcoes: valor do atributo / maior valor do elenco, de 0 a 1
    public void Mostrar(DadosPersonagem dados, IconesTipo icones, float[] proporcoes)
    {
        Dados = dados;
        textoNome.text = dados.nome;
        retrato.Tocar(dados.spritesParado);
        iconeClasse.sprite = icones.DaClasse(dados.classe);
        iconeElemento.sprite = icones.DoElemento(dados.elemento);
        MostrarBarras(proporcoes);
    }

    public void MostrarBarras(float[] proporcoes)
    {
        for (int i = 0; i < barras.Length; i++)
            barras[i].fillAmount = proporcoes[i];
    }

    public void MostrarSelos(bool jogador1, bool jogador2)
    {
        seloJogador1.SetActive(jogador1);
        seloJogador2.SetActive(jogador2);
    }
}

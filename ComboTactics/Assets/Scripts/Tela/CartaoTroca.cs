using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Um personagem da equipe no submenu Trocar: idle, vida atual e atributos
public class CartaoTroca : MonoBehaviour
{
    public Button botao;
    public AnimacaoUI retrato;
    public TMP_Text textoNome;
    public Image iconeClasse;
    public Image iconeElemento;
    public GameObject faixaEstado;   // "Na arena" ou "Derrotado" sobre o retrato
    public TMP_Text textoEstado;
    public Image barraVida;          // vida atual / vida maxima do personagem
    public Image[] barrasAtributo = new Image[3]; // ataque, defesa, velocidade (relativos ao elenco)

    DadosPersonagem mostrado;
    Image imagemRetrato;

    // maximos: maior ataque, defesa e velocidade do elenco (barra cheia)
    public void Mostrar(Lutador lutador, int[] maximos, IconesTipo icones, string estado, bool interagivel)
    {
        DadosPersonagem d = lutador.Dados;
        if (d != mostrado)
        {
            // so reinicia a animacao quando troca o personagem do cartao
            mostrado = d;
            retrato.Tocar(d.spritesParado);
            textoNome.text = d.nome;
            iconeClasse.sprite = icones.DaClasse(d.classe);
            iconeElemento.sprite = icones.DoElemento(d.elemento);
            int[] valores = { d.ataque, d.defesa, d.velocidade };
            for (int i = 0; i < barrasAtributo.Length; i++)
                barrasAtributo[i].fillAmount = maximos[i] > 0 ? (float)valores[i] / maximos[i] : 0f;
        }

        float fracao = (float)lutador.VidaAtual / d.vida;
        barraVida.fillAmount = fracao;
        barraVida.color = PainelJogador.CorDaVida(fracao);

        faixaEstado.SetActive(estado != null);
        textoEstado.text = estado ?? "";
        if (imagemRetrato == null)
            imagemRetrato = retrato.GetComponent<Image>();
        imagemRetrato.color =lutador.EstaVivo ? Color.white : new Color(0.35f, 0.35f, 0.4f, 0.8f);
        botao.interactable = interagivel;
    }
}

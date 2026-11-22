using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Painel de cima de um jogador: personagem ativo, icones, vida e stamina.
// A moldura fica dourada quando e a vez desse jogador.
public class PainelJogador : MonoBehaviour
{
    public Image fundo;
    public Sprite fundoNormal;
    public Sprite fundoDaVez;
    public TMP_Text textoNome;
    public TMP_Text textoVida;
    public Image iconeClasse;
    public Image iconeElemento;
    public Image barraVida;     // Image Filled
    public Image barraStamina;  // Image Filled
    public TMP_Text textoStamina;
    // pocoes ativas no personagem da arena: icone com as rodadas que faltam
    public GameObject efeitoAtaque;
    public TMP_Text textoEfeitoAtaque;
    public GameObject efeitoDefesa;
    public TMP_Text textoEfeitoDefesa;
    public Color corStamina = new Color(0.91f, 0.78f, 0.44f);
    public Color corStaminaCheia = new Color(0.97f, 0.75f, 0.38f);

    static readonly Color VIDA_ALTA = new Color(0.48f, 0.67f, 0.27f);
    static readonly Color VIDA_MEDIA = new Color(0.91f, 0.78f, 0.44f);
    static readonly Color VIDA_BAIXA = new Color(0.76f, 0.25f, 0.11f);

    // verde acima de 50%, ouro ate 25%, vermelho abaixo
    public static Color CorDaVida(float fracao)
    {
        if (fracao > 0.5f) return VIDA_ALTA;
        if (fracao > 0.25f) return VIDA_MEDIA;
        return VIDA_BAIXA;
    }

    public void Atualizar(Equipe equipe, bool daVez, IconesTipo icones, int rodada)
    {
        Lutador ativo = equipe.Ativo;
        fundo.sprite = daVez ? fundoDaVez : fundoNormal;

        textoNome.text = ativo.Dados.nome;
        textoVida.text = ativo.VidaAtual + " / " + ativo.VidaMaxima;
        iconeClasse.sprite = icones.DaClasse(ativo.Dados.classe);
        iconeElemento.sprite = icones.DoElemento(ativo.Dados.elemento);

        float fracao = (float)ativo.VidaAtual / ativo.VidaMaxima;
        barraVida.fillAmount = fracao;
        barraVida.color = CorDaVida(fracao);

        barraStamina.fillAmount = (float)equipe.Stamina / Equipe.STAMINA_MAXIMA;
        barraStamina.color = equipe.EspecialDisponivel ? corStaminaCheia : corStamina;
        textoStamina.text = equipe.EspecialDisponivel ? "ESPECIAL PRONTO!" : "ESPECIAL";

        MostrarEfeito(efeitoAtaque, textoEfeitoAtaque, ativo.RodadasRestantes(TipoItem.PocaoAtaque, rodada));
        MostrarEfeito(efeitoDefesa, textoEfeitoDefesa, ativo.RodadasRestantes(TipoItem.PocaoDefesa, rodada));
    }

    static void MostrarEfeito(GameObject efeito, TMP_Text texto, int rodadas)
    {
        if (efeito == null)
            return;
        efeito.SetActive(rodadas > 0);
        texto.text = rodadas.ToString();
    }
}

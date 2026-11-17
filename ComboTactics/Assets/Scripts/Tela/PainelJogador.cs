using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Nome, vida e stamina de um jogador na parte de cima da tela
public class PainelJogador : MonoBehaviour
{
    public TMP_Text textoNome;
    public TMP_Text textoVida;
    public Slider barraVida;
    public Slider barraStamina;

    public void Atualizar(Equipe equipe)
    {
        Lutador ativo = equipe.Ativo;

        textoNome.text = ativo.Dados.nome;
        textoVida.text = ativo.VidaAtual + " / " + ativo.Dados.vida;

        barraVida.maxValue = ativo.Dados.vida;
        barraVida.value = ativo.VidaAtual;

        barraStamina.maxValue = Equipe.STAMINA_MAXIMA;
        barraStamina.value = equipe.Stamina;
    }
}

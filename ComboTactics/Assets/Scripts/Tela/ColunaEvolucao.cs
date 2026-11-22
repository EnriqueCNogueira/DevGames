using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Um atributo do personagem escolhido na aba Evolucoes: valor atual e o
// proximo, os 5 niveis e o botao com o preco da proxima evolucao
public class ColunaEvolucao : MonoBehaviour
{
    public Atributo atributo;
    public TMP_Text textoNome;
    public TMP_Text textoValor;
    public Image[] niveis = new Image[Evolucao.NIVEL_MAXIMO];
    public Sprite nivelVazio;
    public Sprite nivelCheio;
    public Button botaoEvoluir;
    public TMP_Text textoBotao;
    public GameObject moedaBotao; // some quando o botao nao mostra preco

    public void Mostrar(Perfil perfil, DadosPersonagem personagem)
    {
        int nivel = perfil.Nivel(personagem, atributo);
        int valorBase = Atributos.ValorBase(personagem, atributo);
        int atual = Evolucao.Valor(valorBase, nivel, atributo);

        textoNome.text = Atributos.Nome(atributo);
        for (int i = 0; i < niveis.Length; i++)
            niveis[i].sprite = i < nivel ? nivelCheio : nivelVazio;

        if (nivel >= Evolucao.NIVEL_MAXIMO)
        {
            textoValor.text = atual.ToString();
            textoBotao.text = "Máximo";
            moedaBotao.SetActive(false);
            botaoEvoluir.interactable = false;
            return;
        }

        int proximo = Evolucao.Valor(valorBase, nivel + 1, atributo);
        // a Defesa para em 100: evoluir alem disso nao muda nada
        if (proximo == atual)
        {
            textoValor.text = atual.ToString();
            textoBotao.text = "No limite";
            moedaBotao.SetActive(false);
            botaoEvoluir.interactable = false;
            return;
        }

        textoValor.text = atual + "  >  <color=#e8c870>" + proximo + "</color>";
        textoBotao.text = "Evoluir  " + Evolucao.Preco(nivel);
        moedaBotao.SetActive(true);
        botaoEvoluir.interactable = perfil.PodeEvoluir(personagem, atributo);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Um item na aba Itens da Loja: icone, nome, efeito, estoque e botao de compra
public class CartaoItemLoja : MonoBehaviour
{
    public TipoItem tipo;
    public Image icone;
    public TMP_Text textoNome;
    public TMP_Text textoDescricao;
    public TMP_Text textoEstoque;
    public Button botaoComprar;
    public TMP_Text textoBotao;
    public GameObject moedaBotao; // some quando o botao nao mostra preco
    public GameObject vinha;      // "Em breve" do Remedio

    public void Mostrar(Perfil perfil)
    {
        bool disponivel = Itens.Disponivel(tipo);
        textoNome.text = Itens.Nome(tipo);
        textoDescricao.text = Itens.Descricao(tipo);
        vinha.SetActive(!disponivel);

        if (!disponivel)
        {
            textoEstoque.text = "Em breve";
            textoBotao.text = "Indisponível";
            moedaBotao.SetActive(false);
            botaoComprar.interactable = false;
            return;
        }

        int estoque = perfil.Estoque(tipo);
        bool cheio = estoque >= Itens.ESTOQUE_MAXIMO;
        textoEstoque.text = "No estoque: " + estoque + " de " + Itens.ESTOQUE_MAXIMO;
        textoBotao.text = cheio ? "Estoque cheio" : "Comprar  " + Itens.Preco(tipo);
        moedaBotao.SetActive(!cheio);
        botaoComprar.interactable = perfil.PodeComprar(tipo);
    }
}

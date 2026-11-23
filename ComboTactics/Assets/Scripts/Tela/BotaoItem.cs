using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Botao de um item: icone, nome com a quantidade e, embaixo, o efeito.
// Usado no submenu Itens da Batalha e na escolha de itens da Selecao.
public class BotaoItem : MonoBehaviour
{
    public TipoItem tipo;
    public Button botao;
    public Image icone;
    public TMP_Text titulo;
    public TMP_Text detalhe;
    public GameObject vinha; // sobreposicao de "Em breve" (Remedio)

    public void Mostrar(string textoTitulo, string textoDetalhe, bool ativo)
    {
        titulo.text = textoTitulo;
        detalhe.text = textoDetalhe;
        botao.interactable = ativo;
        if (vinha != null)
            vinha.SetActive(!Itens.Disponivel(tipo));
    }
}

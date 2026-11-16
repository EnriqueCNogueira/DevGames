using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Botao de golpe na Batalha: nome do golpe e, embaixo, o tipo
public class BotaoAcao : MonoBehaviour
{
    public Button botao;
    public TMP_Text titulo;
    public TMP_Text detalhe;

    public void Mostrar(string textoTitulo, string textoDetalhe, bool ativo)
    {
        titulo.text = textoTitulo;
        detalhe.text = textoDetalhe;
        botao.interactable = ativo;
    }
}

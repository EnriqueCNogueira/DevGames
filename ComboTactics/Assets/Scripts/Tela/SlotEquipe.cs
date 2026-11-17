using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Uma vaga da equipe na Selecao. Clicar na vaga tira o personagem dela.
public class SlotEquipe : MonoBehaviour
{
    public int indice; // posicao na ordem de escolha: 0 e o titular
    public Button botao;
    public AnimacaoUI retrato;
    public TMP_Text textoNome;
    public GameObject faixaNome;
    public GameObject marcaVazio;

    public void Mostrar(DadosPersonagem dados)
    {
        bool ocupado = dados != null;
        retrato.Tocar(ocupado ? dados.spritesParado : null);
        textoNome.text = ocupado ? dados.nome : "";
        faixaNome.SetActive(ocupado);
        marcaVazio.SetActive(!ocupado);
    }
}

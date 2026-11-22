using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Tela inicial. PvP e Loja funcionam; Historia aparece desabilitada ate o Sprint 5.
public class ControladorMenu : MonoBehaviour
{
    public Button botaoPvP;
    public Button botaoHistoria;
    public Button botaoLoja;
    public TMP_Text textoSaldo;

    void Awake()
    {
        // voltar ao Menu descarta a partida anterior
        PartidaAtual.Limpar();
    }

    void Start()
    {
        botaoPvP.onClick.AddListener(AbrirSelecao);
        botaoLoja.onClick.AddListener(() => SceneManager.LoadScene(Cenas.LOJA));
        botaoHistoria.interactable = false;
        textoSaldo.text = PerfilSalvo.Atual.moedas.ToString();
    }

    void AbrirSelecao()
    {
        SceneManager.LoadScene(Cenas.SELECAO);
    }
}

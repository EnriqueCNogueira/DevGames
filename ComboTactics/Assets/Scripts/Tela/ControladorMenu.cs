using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Tela inicial. So o PvP funciona no Sprint 1; Historia e Loja aparecem
// desabilitados ate os Sprints 5 e 2.
public class ControladorMenu : MonoBehaviour
{
    public Button botaoPvP;
    public Button botaoHistoria;
    public Button botaoLoja;

    void Awake()
    {
        // voltar ao Menu descarta a partida anterior
        PartidaAtual.Limpar();
    }

    void Start()
    {
        botaoPvP.onClick.AddListener(AbrirSelecao);
        botaoHistoria.interactable = false;
        botaoLoja.interactable = false;
    }

    void AbrirSelecao()
    {
        SceneManager.LoadScene(Cenas.SELECAO);
    }
}

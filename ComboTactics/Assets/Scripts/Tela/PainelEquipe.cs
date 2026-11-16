using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Painel de baixo com as 3 vagas de um jogador
public class PainelEquipe : MonoBehaviour
{
    public Image fundo;
    public Sprite fundoNormal;
    public Sprite fundoAtivo; // moldura de ouro para quem esta escolhendo
    public TMP_Text textoEstado;
    public SlotEquipe[] slots = new SlotEquipe[3];

    public void Mostrar(SelecaoEquipe equipe, bool escolhendo, bool confirmada)
    {
        fundo.sprite = escolhendo ? fundoAtivo : fundoNormal;

        // sem contador "2/3": as vagas ja mostram quanto falta
        if (escolhendo)
            textoEstado.text = equipe.Completa ? "Confirme" : "Escolhendo";
        else if (confirmada)
            textoEstado.text = "Pronto";
        else
            textoEstado.text = "Aguardando";

        foreach (SlotEquipe slot in slots)
        {
            slot.Mostrar(equipe.Em(slot.indice));
            // so quem esta escolhendo tira personagem da propria equipe
            slot.botao.interactable = escolhendo && equipe.Em(slot.indice) != null;
        }
    }
}

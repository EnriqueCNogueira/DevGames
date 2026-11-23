using UnityEditor;
using UnityEngine;

// Atalhos do editor para testar a Loja sem jogar varias partidas.
// Mexem so no PlayerPrefs do editor; o jogo no navegador tem o seu proprio.
public static class MenuProgresso
{
    [MenuItem("Combo Tactics/Progresso/Apagar progresso")]
    static void Apagar()
    {
        if (!EditorUtility.DisplayDialog("Apagar progresso",
                "Volta ao perfil novo: " + Perfil.MOEDAS_INICIAIS + " moedas, sem itens e sem evolucoes.",
                "Apagar", "Cancelar"))
            return;
        PerfilSalvo.Apagar();
        Debug.Log("Progresso apagado.");
    }

    [MenuItem("Combo Tactics/Progresso/Ganhar 1000 moedas")]
    static void GanharMoedas()
    {
        PerfilSalvo.Atual.Ganhar(1000);
        PerfilSalvo.Salvar();
        Debug.Log("Moedas: " + PerfilSalvo.Atual.moedas);
    }
}

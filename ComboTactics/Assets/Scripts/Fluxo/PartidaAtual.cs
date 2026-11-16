using System;
using UnityEngine;

// Guarda as equipes escolhidas para a proxima batalha, entre uma cena e outra.
// E estatica e C# puro: nao depende de nenhum objeto na cena.
//
// O projeto roda com o domain reload desligado (Enter Play Mode Options), entao
// valores estaticos sobrevivem de um Play para o outro no editor. Por isso ela
// e limpa no inicio de todo Play e sempre que o Menu abre.
public static class PartidaAtual
{
    public const int TAMANHO_EQUIPE = 3;

    static DadosPersonagem[] equipe1;
    static DadosPersonagem[] equipe2;

    // true quando a Selecao ja guardou as duas equipes
    public static bool TemEquipes
    {
        get { return equipe1 != null && equipe2 != null; }
    }

    // devolve uma copia, para quem le nao alterar a equipe guardada
    public static DadosPersonagem[] Equipe(int jogador)
    {
        DadosPersonagem[] equipe = jogador == 0 ? equipe1 : equipe2;
        return equipe == null ? null : (DadosPersonagem[])equipe.Clone();
    }

    public static void Guardar(DadosPersonagem[] jogador1, DadosPersonagem[] jogador2)
    {
        Validar(jogador1, "jogador1");
        Validar(jogador2, "jogador2");

        // guarda copias: mudar o array original depois nao muda a partida
        equipe1 = (DadosPersonagem[])jogador1.Clone();
        equipe2 = (DadosPersonagem[])jogador2.Clone();
    }

    public static void Limpar()
    {
        equipe1 = null;
        equipe2 = null;
    }

    static void Validar(DadosPersonagem[] equipe, string nome)
    {
        if (equipe == null || equipe.Length != TAMANHO_EQUIPE)
            throw new ArgumentException("A equipe precisa ter " + TAMANHO_EQUIPE + " personagens.", nome);
        foreach (DadosPersonagem p in equipe)
        {
            if (p == null)
                throw new ArgumentException("A equipe tem uma vaga vazia.", nome);
        }
    }

    // roda antes de qualquer cena carregar, em todo Play
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void LimparAoIniciarPlay()
    {
        Limpar();
    }
}

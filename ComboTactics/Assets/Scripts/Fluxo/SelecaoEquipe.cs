using System.Collections.Generic;

// Escolha de uma equipe na tela de Selecao, sem nada de tela.
// Regras: 3 personagens diferentes; a ordem de escolha define quem entra
// primeiro (o primeiro escolhido e o titular).
public class SelecaoEquipe
{
    public const int TAMANHO = PartidaAtual.TAMANHO_EQUIPE;

    readonly List<DadosPersonagem> escolhidos = new List<DadosPersonagem>();

    public int Quantidade
    {
        get { return escolhidos.Count; }
    }

    public bool Completa
    {
        get { return escolhidos.Count == TAMANHO; }
    }

    // null enquanto ninguem foi escolhido
    public DadosPersonagem Titular
    {
        get { return Em(0); }
    }

    // personagem na posicao de escolha (0 = titular), ou null se a vaga esta vazia
    public DadosPersonagem Em(int indice)
    {
        return indice >= 0 && indice < escolhidos.Count ? escolhidos[indice] : null;
    }

    public bool Contem(DadosPersonagem personagem)
    {
        return escolhidos.Contains(personagem);
    }

    public bool Adicionar(DadosPersonagem personagem)
    {
        if (personagem == null || Completa || Contem(personagem))
            return false;

        escolhidos.Add(personagem);
        return true;
    }

    // tirar alguem puxa os seguintes uma posicao para frente:
    // se o titular sai, o segundo escolhido vira o titular
    public bool Remover(int indice)
    {
        if (indice < 0 || indice >= escolhidos.Count)
            return false;

        escolhidos.RemoveAt(indice);
        return true;
    }

    // a equipe na ordem de escolha; null se ainda nao esta completa
    public DadosPersonagem[] ParaArray()
    {
        return Completa ? escolhidos.ToArray() : null;
    }
}

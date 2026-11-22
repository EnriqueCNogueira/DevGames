// Escolha dos itens que um jogador leva para a batalha, na Selecao.
// Os dois jogadores separam itens do mesmo estoque: o que o outro ja separou
// nao esta disponivel. Nada sai do estoque aqui; so o uso na batalha consome.
public class SelecaoItens
{
    readonly int[] estoque = new int[Itens.QUANTIDADE];
    readonly Mochila doOutro;

    public Mochila Mochila { get; private set; }

    // estoque: quantidade de cada item no perfil; doOutro: o que o outro jogador ja separou (pode ser null)
    public SelecaoItens(int[] estoque, Mochila doOutro, Mochila atual = null)
    {
        for (int i = 0; i < Itens.QUANTIDADE && i < estoque.Length; i++)
            this.estoque[i] = estoque[i];
        this.doOutro = doOutro;
        Mochila = atual != null ? atual.Copia() : new Mochila();

        // o estoque pode ter mudado desde a ultima escolha (ex.: revanche depois de gastar)
        foreach (TipoItem tipo in Itens.TODOS)
        {
            while (Mochila.Quantidade(tipo) > 0 && Livres(tipo) < 0)
                Mochila.Tirar(tipo);
        }
    }

    // quantos ainda podem ser separados por este jogador
    public int Disponivel(TipoItem tipo)
    {
        return Itens.Disponivel(tipo) ? System.Math.Max(0, Livres(tipo)) : 0;
    }

    int Livres(TipoItem tipo)
    {
        int doOutroJogador = doOutro != null ? doOutro.Quantidade(tipo) : 0;
        return estoque[(int)tipo] - doOutroJogador - Mochila.Quantidade(tipo);
    }

    public bool PodeAdicionar(TipoItem tipo)
    {
        return !Mochila.Cheia && Disponivel(tipo) > 0;
    }

    public bool Adicionar(TipoItem tipo)
    {
        return PodeAdicionar(tipo) && Mochila.Adicionar(tipo);
    }

    public bool Tirar(TipoItem tipo)
    {
        return Mochila.Tirar(tipo);
    }

    // false quando nao ha nada que este jogador possa levar: a tela avisa que o estoque esta vazio
    public bool TemAlgoParaEscolher
    {
        get
        {
            if (Mochila.Total > 0)
                return true;
            foreach (TipoItem tipo in Itens.TODOS)
            {
                if (Disponivel(tipo) > 0)
                    return true;
            }
            return false;
        }
    }
}

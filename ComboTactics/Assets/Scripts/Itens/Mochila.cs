// Itens que um jogador leva para uma batalha (ate Itens.POR_BATALHA no total).
// Os itens sao separados do estoque na Selecao, mas so saem do estoque quando usados.
public class Mochila
{
    readonly int[] quantidades = new int[Itens.QUANTIDADE];

    public int Total
    {
        get
        {
            int total = 0;
            foreach (int q in quantidades)
                total += q;
            return total;
        }
    }

    public bool Cheia
    {
        get { return Total >= Itens.POR_BATALHA; }
    }

    public int Quantidade(TipoItem tipo)
    {
        return quantidades[(int)tipo];
    }

    public bool Tem(TipoItem tipo)
    {
        return Quantidade(tipo) > 0;
    }

    public bool Adicionar(TipoItem tipo)
    {
        if (Cheia || !Itens.Disponivel(tipo))
            return false;
        quantidades[(int)tipo]++;
        return true;
    }

    public bool Tirar(TipoItem tipo)
    {
        if (!Tem(tipo))
            return false;
        quantidades[(int)tipo]--;
        return true;
    }

    public Mochila Copia()
    {
        Mochila copia = new Mochila();
        quantidades.CopyTo(copia.quantidades, 0);
        return copia;
    }
}

// Um personagem durante a batalha (os dados fixos ficam no DadosPersonagem)
public class Lutador
{
    public DadosPersonagem Dados { get; private set; }
    public int VidaAtual { get; private set; }

    public bool EstaVivo
    {
        get { return VidaAtual > 0; }
    }

    public Lutador(DadosPersonagem dados)
    {
        Dados = dados;
        VidaAtual = dados.vida;
    }

    public void ReceberDano(int dano)
    {
        VidaAtual -= dano;
        if (VidaAtual < 0)
            VidaAtual = 0;
    }
}

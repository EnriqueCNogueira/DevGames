// Ordem da tela de Selecao: o Jogador 1 escolhe e confirma, depois o Jogador 2.
// Tudo fica visivel, e os dois podem escolher o mesmo personagem.
public class SelecaoPartida
{
    public SelecaoEquipe[] Equipes = { new SelecaoEquipe(), new SelecaoEquipe() };

    public int JogadorDaVez { get; private set; }

    // true depois que o Jogador 2 confirma: a partida pode comecar
    public bool Pronta { get; private set; }

    public SelecaoEquipe EquipeDaVez
    {
        get { return Equipes[JogadorDaVez]; }
    }

    public bool Escolher(DadosPersonagem personagem)
    {
        return !Pronta && EquipeDaVez.Adicionar(personagem);
    }

    public bool Remover(int indice)
    {
        return !Pronta && EquipeDaVez.Remover(indice);
    }

    public bool PodeConfirmar
    {
        get { return !Pronta && EquipeDaVez.Completa; }
    }

    public bool Confirmar()
    {
        if (!PodeConfirmar)
            return false;

        if (JogadorDaVez == 0)
            JogadorDaVez = 1;
        else
            Pronta = true;
        return true;
    }

    // Voltar um passo: o Jogador 2 devolve a vez ao Jogador 1 (as escolhas dos
    // dois ficam). Devolve true quando a tela deve voltar ao Menu.
    public bool Voltar()
    {
        if (Pronta)
            return false;

        if (JogadorDaVez == 1)
        {
            JogadorDaVez = 0;
            return false;
        }
        return true;
    }
}

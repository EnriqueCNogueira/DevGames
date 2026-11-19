# Game Design Document — Combo Tactics

## 1. Visão geral

- **Gênero:** RPG tático por turnos, com gerenciamento de equipe e duelos 1v1 alternados.
- **Plataforma:** Navegador em PC, sem instalação; requer internet.
- **Público-alvo:** Fãs de RPG tático, otimização de equipes e batalhas competitivas.
- **Classificação indicativa:** 14 anos.
- **Modelo de negócio:** Gratuito, com pacotes opcionais de moedas; ver seção 10.
- **Fantasia do jogador:** Um novo duelista monta uma equipe de três lutadores e enfrenta desafios para alcançar a glória.
- **Pilares de experiência:**
  1. Montar uma equipe e explorar combinações de classe e elemento.
  2. Tomar decisões táticas em batalhas curtas de equipes 3 contra 3, com apenas um lutador ativo de cada lado.
  3. Reagir no momento certo e adaptar-se às mudanças de arena.

## 2. Modos de jogo e fluxo

### 2.1 Modos

- **História:** Campanha de quatro batalhas em sequência: três guardiões elementais e o confronto final contra o antigo guardião. Os adversários são liberados ao vencer a etapa anterior.
- **PvP local:** Dois jogadores disputam uma batalha no mesmo aparelho e revezam os turnos. A equipe, a arena e seus efeitos são compartilhados na mesma tela.
- **Contra a IA:** Batalhas fora da campanha contra a IA, com escolha de dificuldade.
- **Torneio PvP:** O caderno associa uma recompensa de 500 moedas à vitória no torneio. A estrutura do torneio ainda precisa ser definida.

### 2.2 Fluxo principal

1. No menu, escolher História, Multiplayer ou Loja.
2. Selecionar uma equipe de três personagens e os itens para a batalha.
3. Na História, escolher a fase disponível; nos outros modos, escolher o oponente ou a dificuldade.
4. Lutar usando ataques, itens, troca de personagem e ações defensivas por tempo.
5. Ao fim da batalha, receber moedas e retornar ao menu.

O fluxo segue o diagrama: **Menu → Selecionar equipe e itens → Escolher fase → Ações de batalha → Estratégias de batalha → Derrotar o oponente**.

## 3. História e mundo

### 3.1 Premissa

Fogo, Água e Planta eram mantidos em equilíbrio pelo Cristal Elemental, protegido no Santuário da Origem. O antigo guardião do santuário, acreditando que o cristal limitava o potencial dos elementos, tentou concentrar seu poder em si. A tentativa partiu o cristal em três fragmentos e provocou um desequilíbrio que ameaça o mundo.

Os fragmentos foram parar em regiões distintas, sob a guarda de três elementais. A energia dos fragmentos ampliou seus poderes e distorceu suas personalidades; convencidos de que protegem o mundo, eles impedem que qualquer pessoa se aproxime.

Um novo duelista parte para recuperar os fragmentos. Ao longo da jornada, descobre a história do cristal e as motivações do antigo guardião. Depois de reunir os três fragmentos, segue para a Fortaleza, construída sobre as ruínas do santuário. O guardião oferece ao duelista a chance de se unir a ele; após a recusa, uma batalha final decide o destino das forças elementais. O objetivo é devolver o cristal ao santuário e restabelecer o equilíbrio, embora as marcas da ruptura permaneçam.

### 3.2 Estrutura da campanha

| Etapa | Desafio | Objetivo narrativo |
| --- | --- | --- |
| 1–3 | Um guardião de cada elemento | Recuperar os três fragmentos e atravessar as regiões afetadas |
| 4 | Antigo guardião, na Fortaleza | Recuperar o cristal e restaurar o equilíbrio |

Cada um dos três primeiros adversários está associado a um ambiente exclusivo. A ambientação da fase deve refletir sua região; durante o combate, o sorteio de arena descrito na seção 5.7 pode alterar temporariamente o ambiente ativo.

O caderno descreve os três primeiros adversários como equipes de um único elemento e o chefe final como um duelista veterano com um personagem exclusivo e mais forte. A composição exata das equipes dos guardiões e os detalhes da batalha final devem permanecer consistentes com essa proposta.

Os rivais são competidores, não necessariamente vilões. A ameaça central da campanha é o desequilíbrio causado pela ruptura do cristal e a decisão do antigo guardião.

### 3.3 Duelista do jogador

O protagonista é um novato desconhecido que entra nos desafios por vontade própria. Ele representa o jogador, não é uma unidade controlável em combate e não aparece na arte do jogo.

## 4. Personagens

Há nove personagens jogáveis, cobrindo todas as combinações de três classes e três elementos. O jogador leva três para cada batalha. Os nove personagens estão disponíveis desde o início; a progressão da campanha libera adversários, não personagens jogáveis.

### 4.1 Guerreiros

| Personagem | Tipo | HP | Ataque | Defesa | Velocidade | Habilidades |
| --- | --- | ---: | ---: | ---: | ---: | --- |
| Mula sem cabeça | Guerreiro de Fogo | 620 | 130 | 45 | 77 | Corte Flamejante, Investida Vulcânica, Brado de Calor |
| Tubarão | Guerreiro de Água | 620 | 119 | 55 | 90 | Lâmina de Correnteza, Impacto Geiser, Defesa Fluida |
| Curupira | Guerreiro de Planta | 620 | 112 | 65 | 73 | Esmagamento com Raízes, Escudo de Madeira, Golpe Espinhoso |

### 4.2 Magos

| Personagem | Tipo | HP | Ataque | Defesa | Velocidade | Habilidades |
| --- | --- | ---: | ---: | ---: | ---: | --- |
| Fênix | Mago de Fogo | 560 | 171 | 15 | 52 | Bola de Fogo, Explosão Solar, Chuva de Meteoros |
| Sapo Feiticeiro | Mago de Água | 560 | 160 | 25 | 65 | Jato de Pressão, Prisão Gélida, Nevasca |
| Cogumelo | Mago de Planta | 560 | 153 | 35 | 48 | Chicote de Vinhas, Esporos Tóxicos, Rajada de Folhas Cortantes |

### 4.3 Tanques

| Personagem | Tipo | HP | Ataque | Defesa | Velocidade | Habilidades |
| --- | --- | ---: | ---: | ---: | ---: | --- |
| Dragão | Tanque de Fogo | 880 | 73 | 75 | 27 | Escudo de Magma, Provocação Ardente, Impacto de Cinzas |
| Poseidon | Tanque de Água | 880 | 68 | 85 | 40 | Barreira Hidráulica, Carapaça de Gelo, Maremoto Punitivo |
| Árvore guerreira | Tanque de Planta | 880 | 63 | 95 | 23 | Muralha de Troncos, Absorção Vital, Casca Grossa |

Os valores são atributos-base. O equilíbrio comparativo entre personagens ainda deve ser validado em testes; os atributos diferentes não permitem concluir, por si só, que os confrontos são equivalentes.

## 5. Batalha e regras

### 5.1 Equipes, turnos e vitória

- Cada lado leva três personagens e mantém um lutador ativo na arena por vez.
- A batalha é composta por duelos 1v1 alternados. Vence quem derrotar todos os personagens da equipe adversária.
- Em seu turno, o jogador escolhe uma ação: atacar, usar um item ou trocar o personagem ativo.
- Cada personagem tem três ataques: dois disponíveis normalmente e um ataque especial, liberado com a barra de stamina cheia.
- A Velocidade determina quem age primeiro no turno; ela não entra diretamente no cálculo de dano.

### 5.2 Vantagens de elemento e classe

Os dois ciclos de vantagem são:

| Categoria | Ciclo |
| --- | --- |
| Elementos | Fogo → Planta → Água → Fogo |
| Classes | Guerreiro → Mago → Tanque → Guerreiro |

| Elemento | Forte contra | Fraco contra |
| --- | --- | --- |
| Fogo | Planta | Água |
| Planta | Água | Fogo |
| Água | Fogo | Planta |

| Classe | Forte contra | Fraco contra |
| --- | --- | --- |
| Guerreiro | Mago | Tanque |
| Mago | Tanque | Guerreiro |
| Tanque | Guerreiro | Mago |

A vantagem de elemento e a de classe podem se acumular. A regra documentada atribui +30% de dano a uma vantagem e +50% a duas. Também associa uma vantagem a −30% de dano recebido e duas a −50%.

### 5.3 Atributos e cálculo de dano

- **Ataque:** determina o dano causado; a fórmula completa não está especificada.
- **Defesa:** cada ponto reduz em 0,5% o dano recebido, até o máximo de 50% com Defesa 100.
- **Velocidade:** define a ordem de ação, sem alterar o dano.

| Defesa | Redução de dano |
| ---: | ---: |
| 20 | 10% |
| 50 | 25% |
| 100 | 50% |

### 5.4 Stamina e ataque especial

Cada jogador tem uma barra de stamina. Ela carrega ao causar dano, sofrer dano e defender com sucesso. Quando cheia, permite usar o ataque especial do personagem ativo.

### 5.5 Desvio e contra-ataque

Ao ser alvo de um ataque, o jogador defensor recebe uma janela de tempo para clicar e tentar uma resposta. Um acerto resulta em desvio, que anula o dano, ou contra-ataque, que devolve o ataque recebido.

### 5.6 Efeitos de status

Ataques especiais podem aplicar efeitos positivos ou negativos a outros personagens. Os efeitos duram três turnos e acompanham o personagem mesmo quando ele sai da arena.

- **Negativos:** Chamas, Afogamento e Veneno causam dano contínuo ao fim de cada turno e podem ser removidos com Remédio.
- **Positivos:** regeneração restaura HP a cada turno; outros efeitos podem aumentar atributos temporariamente.

### 5.7 Arena e mudança de ambiente

A câmera observa uma arena 2D fixa. A cada três rodadas, um sorteio muda o ambiente; o efeito é aplicado apenas na rodada sorteada. O modificador documentado é de ±10% no dano.

| Ambiente | Favorece | Prejudica |
| --- | --- | --- |
| Mar | Personagens de Água | Personagens de Fogo |
| Vulcão | Personagens de Fogo | Personagens de Planta |
| Floresta | Personagens de Planta | Personagens de Água |
| Lua | Personagens de baixa Velocidade | Personagens de alta Velocidade |

## 6. Loja, itens e progressão

### 6.1 Moedas e recompensas

| Resultado | Recompensa |
| --- | ---: |
| Derrota | 50 moedas |
| Vitória em batalha | 100 moedas |
| Vitória no torneio | 500 moedas |

As moedas podem ser gastas em itens e melhorias permanentes de atributos. A campanha libera a próxima etapa após a vitória na etapa atual.

### 6.2 Itens

Os itens são consumíveis e ocupam a ação do turno:

| Item | Efeito documentado |
| --- | --- |
| Kit Médico | Recupera HP; quantidade a definir |
| Remédio | Remove efeitos negativos |
| Poção de Ataque | Aumenta Ataque; valor e duração a definir |
| Poção de Defesa | Aumenta Defesa em 10%; duração a definir |

### 6.3 Melhorias de atributos

Na loja, o jogador pode evoluir individualmente HP, Ataque, Defesa ou Velocidade de cada personagem. Cada compra aumenta o atributo em 3% do valor-base; o limite é de cinco melhorias por atributo, ou +15%. As melhorias são permanentes.

Os preços dos itens e das melhorias não estão definidos.

## 7. Direção de arte, áudio e apresentação

- **Arte:** estilo cartoon 2D estilizado para os nove lutadores e as arenas.
- **Câmera:** lateral e fixa; aproxima o foco do personagem durante o ataque especial.
- **Ambientes visuais:** Mar, Vulcão, Floresta e Lua.
- **Música:** animada, leve e energética.
- **Efeitos sonoros:** vozes e rugidos próprios para dar personalidade aos lutadores.
- **Tom emocional:** competição eletrizante, vínculo com a equipe e sensação de risco criada pelo sorteio da arena. Horror e sobrevivência não fazem parte da proposta.

## 8. Escopo e conteúdo

- Não há exploração de mapa, fases bônus ou hazards; o dano vem das batalhas e dos efeitos de status.
- Não há editor de personagens, arenas ou regras.
- O personagem que representa o jogador não é controlado durante o combate.
- Os nove lutadores jogáveis estão disponíveis desde o início; não são desbloqueados ao vencer adversários.

## 9. Multiplayer e plataforma

O PvP é local, para dois jogadores no mesmo aparelho. Não há divisão de tela: os jogadores revezam o dispositivo e compartilham a visualização da batalha. O jogo é pensado para navegador de PC, teclado e mouse, com conexão à internet.

## 10. Modelo de negócio

O jogo é gratuito no navegador, sem anúncios e sem cobrança para acessar a campanha, o multiplayer ou os nove personagens. Pacotes de moedas podem ser comprados para adquirir itens e melhorias mais rapidamente. Como melhorias de atributos e itens também afetam o combate, essa compra pode conceder vantagem indireta no PvP.
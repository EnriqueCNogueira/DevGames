# Combo Tactics

Jogo feito para a disciplina de Desenvolvimento de Games da UNIFESP.

É um RPG de turno em pixel art: cada jogador monta uma equipe de 3 personagens e as lutas são 1 contra 1, com troca de personagem durante a batalha. Os personagens têm classe (Guerreiro, Mago ou Tanque) e elemento (Fogo, Água ou Planta), e a vantagem entre eles muda bastante o dano.

## Grupo

- Enrique Campos Nogueira
- João Victor dos Santos Silva

## Como abrir o projeto

1. Instale o Unity **6000.3.28f1** pelo Unity Hub, com o módulo **Web Build Support**.
2. No Hub, clique em *Add project from disk* e escolha a pasta `ComboTactics`.
3. A primeira abertura demora um pouco porque o Unity monta a pasta `Library`.

## Build WebGL

O jogo roda no navegador, em 1280x720. O build sai em `ComboTactics/Builds/WebGL` (pasta fora do git).

- **Pelo Editor**: menu *Combo Tactics > Build WebGL*.
- **Pela linha de comando** (com o Editor fechado):

```bash
unity build ComboTactics --target WebGL --execute-method ConstruirWebGL.Construir
```

O build usa compressão Gzip com *Decompression Fallback*: o próprio loader descompacta os arquivos, porque o GitHub Pages não envia o header `Content-Encoding`. Para testar localmente, sirva a pasta por HTTP (abrir o `index.html` direto do disco não funciona):

```bash
python -m http.server 8000 --directory ComboTactics/Builds/WebGL
```

E abra `http://localhost:8000`.

## Publicação no GitHub Pages

A publicação é manual, numa branch `gh-pages` separada do código (sem histórico em comum, para o build não pesar na `main`). Na raiz do repositório:

1. Na primeira vez, crie a branch órfã numa pasta ao lado:

```bash
git worktree add --orphan -b gh-pages ../DevGames-pages
```

2. A cada publicação, copie o build, commite e envie:

```bash
rm -rf ../DevGames-pages/* && cp -r ComboTactics/Builds/WebGL/. ../DevGames-pages/ && touch ../DevGames-pages/.nojekyll
```

```bash
git -C ../DevGames-pages add -A && git -C ../DevGames-pages commit -m "build: publica a versao WebGL" && git -C ../DevGames-pages push -u origin gh-pages
```

3. Só na primeira vez: no GitHub, em *Settings > Pages*, escolha *Deploy from a branch*, branch `gh-pages`, pasta `/ (root)`.

O jogo fica em `https://enriquecnogueira.github.io/DevGames/`.

## Pastas

- `ComboTactics/` - projeto do Unity
- `arte/` - sprites dos personagens (PNG + JSON de cada animação)
- `imagens/` - imagens das arenas, logo e menu
- `GDD.md` - regras do jogo, status dos personagens e fórmula de dano
- `caderno.md` - história, telas e mundo do jogo
- `SPRITES.md` - lista dos personagens e animações
- `leveldesign.md`, `telas.excalidraw`, `fluxograma.excalidraw` - level design e rascunhos das telas

## Situação

Em desenvolvimento. O Sprint 1 entrega o jogo jogável do início ao fim em PvP local: menu, seleção das duas equipes, batalha 3x3 (golpes, troca de personagem e vantagens de elemento e classe) e tela de resultado com revanche.

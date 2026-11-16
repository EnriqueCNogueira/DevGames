"""Gera os sprites de UI em pixel art do Combo Tactics.

Tudo e desenhado pixel a pixel com a paleta das sheets dos personagens e do
logo (menu.jpeg). Rodar a partir da pasta ComboTactics:

    python Ferramentas/gerar_sprites_ui.py

Saida em Assets/UI/Sprites. Botoes e paineis sao 9-slice (borda no Unity); no
Unity cada texel vale 3 unidades do canvas (PPU 33.333), a mesma densidade dos
personagens com a Pixel Perfect Camera em 640x360.
"""

import os
import random

from PIL import Image

SAIDA = os.path.join("Assets", "UI", "Sprites")

# paleta (sheets dos personagens + pedra e bronze do logo)
CONTORNO = "#0d0b10"
ESCURO = "#2a2220"
BRONZE_SOMBRA2 = "#3c2014"
BRONZE_SOMBRA = "#5a3418"
BRONZE_ESCURO = "#603c24"
BRONZE = "#966c24"
BRONZE_CLARO = "#d4a640"
OURO = "#e8c870"
OURO_BRILHO = "#f6e6a8"
PEDRA_FUNDO = "#342118"
PEDRA_SOMBRA = "#4c392d"
PEDRA = "#625448"
PEDRA_CLARA = "#6e6458"
PEDRA_LUZ = "#887665"
FERRO_SOMBRA = "#2a2c3a"
FERRO_ESCURO = "#3a3c48"
FERRO = "#4e5262"
FERRO_CLARO = "#686266"
FERRO_LUZ = "#868c9c"
PEDRA_FRIA_SOMBRA = "#2e2a2c"
PEDRA_FRIA = "#464246"
PEDRA_FRIA_CLARA = "#524e50"
BRASA_ESCURA = "#7a1c18"
BRASA = "#c2401c"
BRASA_CLARA = "#ee8424"
BRASA_BRILHO = "#f8c060"
FOLHA_ESCURA = "#244a2e"
FOLHA = "#3f7838"
FOLHA_CLARA = "#7aaa44"
FOLHA_BRILHO = "#cadd78"
AGUA_SOMBRA = "#1c3f66"
AGUA = "#2a6e9c"
AGUA_CLARA = "#44a6c8"
AGUA_BRILHO = "#a8dcec"
ACO_BRILHO = "#c4cad6"
CABO = "#5a3418"
MAGIA_ESCURA = "#3a1e5a"
MAGIA_SOMBRA = "#6a3aa0"
MAGIA = "#b46adc"
MAGIA_BRILHO = "#e8b8f4"
FUNDO_PAINEL = "#1e1719"
FUNDO_PAINEL_CLARO = "#261d1e"
FUNDO_SLOT = "#141012"

LARGURA = 128
ALTURA = 40
CHANFRO = 2  # cantos cortados na diagonal


def rgba(cor, alfa=255):
    cor = cor.lstrip("#")
    return (int(cor[0:2], 16), int(cor[2:4], 16), int(cor[4:6], 16), alfa)


def anel(x, y, w, h):
    """Distancia do pixel ate a borda, contando o chanfro dos cantos."""
    d = min(x, y, w - 1 - x, h - 1 - y)
    # nos cantos, o chanfro diagonal empurra o anel para dentro
    dx = min(x, w - 1 - x)
    dy = min(y, h - 1 - y)
    diag = dx + dy - CHANFRO
    return min(d, diag)


def lado(x, y, w, h):
    """Qual lado da moldura o pixel pertence: 'cima', 'baixo', 'esq' ou 'dir'."""
    dist = {"cima": y, "baixo": h - 1 - y, "esq": x, "dir": w - 1 - x}
    return min(dist, key=dist.get)


def textura_pedra(img, x0, y0, x1, y1, cores, semente):
    """Pedra com manchas suaves e poucas rachaduras, sem padrao repetido visivel."""
    base, clara, sombra, luz, fundo = cores
    rnd = random.Random(semente)
    for y in range(y0, y1):
        for x in range(x0, x1):
            img.putpixel((x, y), rgba(base))
    # manchas em grupos de 2 a 4 pixels, poucas, para nao competir com o texto
    for _ in range((x1 - x0) * (y1 - y0) // 60):
        x = rnd.randrange(x0 + 1, x1 - 3)
        y = rnd.randrange(y0 + 2, y1 - 3)
        cor = clara if rnd.random() < 0.6 else sombra
        for dx, dy in rnd.choice(([(0, 0), (1, 0)], [(0, 0), (1, 0), (2, 0)], [(0, 0), (1, 0), (1, 1)], [(0, 0)])):
            img.putpixel((x + dx, y + dy), rgba(cor))
    # rachaduras curtas, so perto das bordas laterais
    for cx in (x0 + 3, x1 - 12):
        x = cx + rnd.randrange(0, 4)
        y = rnd.randrange(y0 + 3, y1 - 5)
        for _ in range(rnd.randint(4, 7)):
            img.putpixel((x, y), rgba(fundo))
            img.putpixel((x, y + 1), rgba(luz))
            x += 1
            y = min(y1 - 4, max(y0 + 2, y + rnd.choice((-1, 0, 1))))
    # bisel interno: luz em cima, sombra solida embaixo
    for x in range(x0, x1):
        img.putpixel((x, y0), rgba(luz))
        img.putpixel((x, y1 - 1), rgba(fundo))
        img.putpixel((x, y1 - 2), rgba(sombra))
    for y in range(y0 + 1, y1 - 2):
        img.putpixel((x0, y), rgba(clara))
        img.putpixel((x1 - 1, y), rgba(sombra))


# placa octogonal do canto, com uma gema lapidada no meio. O contorno,
# B metal (o sombreado e calculado pela vizinhanca), W/G/g gema (brilho, base, escura)
PLACA = [
    "..OOOOOO..",
    ".OBBBBBBO.",
    "OBBBOOBBBO",
    "OBBOWGOBBO",
    "OBOWGGgOBO",
    "OBOGGggOBO",
    "OBBOGgOBBO",
    "OBBBOOBBBO",
    ".OBBBBBBO.",
    "..OOOOOO..",
]
BORDA = 10  # borda do 9-slice: cobre moldura e placa


def placa_canto(img, espelho_x, espelho_y, cores):
    contorno, luz, base, escura, sombra, gema_escura, gema, gema_brilho = cores
    w, h = img.size
    n = len(PLACA)

    def em(x, y):
        return PLACA[y][x] if 0 <= x < n and 0 <= y < n else "."

    for y in range(n):
        for x in range(n):
            # le o desenho ja espelhado, para a luz vir sempre de cima-esquerda
            sx = n - 1 - x if espelho_x else x
            sy = n - 1 - y if espelho_y else y
            c = PLACA[sy][sx]
            if c == ".":
                continue
            if c == "O":
                cor = contorno
            elif c in "WGg":
                cor = {"W": gema_brilho, "G": gema, "g": gema_escura}[c]
            else:
                vx = -1 if espelho_x else 1
                vy = -1 if espelho_y else 1
                if em(sx, sy - vy) == "O" or em(sx - vx, sy) == "O":
                    cor = luz
                elif em(sx, sy + vy) == "O":
                    cor = sombra
                elif em(sx + vx, sy) == "O":
                    cor = escura
                else:
                    cor = base
            px = w - n + x if espelho_x else x
            py = h - n + y if espelho_y else y
            img.putpixel((px, py), rgba(cor))


def botao(estilo):
    w, h = LARGURA, ALTURA
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    m = estilo["moldura"]  # (luz, clara, base, escura, sombra)
    afundado = estilo.get("afundado", False)

    for y in range(h):
        for x in range(w):
            a = anel(x, y, w, h)
            if a < 0:
                continue
            l = lado(x, y, w, h)
            de_cima = l in ("cima", "esq")
            if afundado:
                de_cima = not de_cima
            if a == 0:
                cor = CONTORNO
            elif a == 1:
                cor = m[0] if de_cima and l == "cima" else (m[1] if de_cima else m[4])
            elif a == 2:
                cor = m[1] if de_cima else m[3]
            elif a == 3:
                cor = m[2]
            elif a == 4:
                # friso escuro gravado no meio da moldura
                cor = m[3]
            elif a == 5:
                cor = m[2] if de_cima else m[3]
            elif a == 6:
                cor = ESCURO
            else:
                continue
            img.putpixel((x, y), rgba(cor))

    # face de pedra
    textura_pedra(img, 7, 7, w - 7, h - 7, estilo["pedra"], estilo.get("semente", 7))
    if afundado:
        # sombra da moldura caindo sobre a pedra
        for x in range(7, w - 7):
            img.putpixel((x, 7), rgba(estilo["pedra"][4]))
            img.putpixel((x, 8), rgba(estilo["pedra"][2]))

    # placas ornamentadas nos quatro cantos
    for ex in (False, True):
        for ey in (False, True):
            placa_canto(img, ex, ey, estilo["placa"])

    return img


FOLHA_BASE = [
    "..GG",
    ".LGG",
    "LGGD",
    "GGD.",
]


def trepadeira():
    """Vinha sobreposta aos botoes 'Em breve': cresce pelos cantos da placa."""
    w, h = LARGURA, ALTURA
    planta = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    rnd = random.Random(11)
    cores = {"L": FOLHA_BRILHO, "G": FOLHA_CLARA, "D": FOLHA}

    folha_img = Image.new("RGBA", (4, 4), (0, 0, 0, 0))
    for y, linha in enumerate(FOLHA_BASE):
        for x, c in enumerate(linha):
            if c != ".":
                folha_img.putpixel((x, y), rgba(cores[c]))

    def folha(x, y, virar_x, virar_y):
        f = folha_img
        if virar_x:
            f = f.transpose(Image.FLIP_LEFT_RIGHT)
        if virar_y:
            f = f.transpose(Image.FLIP_TOP_BOTTOM)
        ox = x - 3 if virar_x else x + 1
        oy = y + 1 if virar_y else y - 3
        planta.alpha_composite(f, (max(0, min(w - 4, ox)), max(0, min(h - 4, oy))))

    def caule(caminho):
        for i, (x, y) in enumerate(caminho):
            planta.putpixel((x, y), rgba(FOLHA_ESCURA))
        for i, (x, y) in enumerate(caminho):
            if i % 5 == 3:
                vertical = i + 1 < len(caminho) and caminho[i + 1][0] == x
                lado = (i // 5) % 2 == 0
                if vertical:
                    folha(x, y, not lado, False)
                else:
                    folha(x, y, False, not lado)

    def andar(x, y, dx, dy, passos, faixa):
        pts = []
        for _ in range(passos):
            pts.append((x, y))
            x += dx
            y += dy
            if rnd.random() < 0.3:
                if dx == 0:
                    x = max(faixa[0], min(faixa[1], x + rnd.choice((-1, 1))))
                else:
                    y = max(faixa[0], min(faixa[1], y + rnd.choice((-1, 1))))
        return pts, x, y

    # canto inferior esquerdo: sobe pela lateral e entra no topo
    sobe, x, y = andar(3, h - 2, 0, -1, h - 6, (2, 5))
    vai, _, _ = andar(x, y, 1, 0, 34, (2, 5))
    caule(sobe + vai)
    # canto superior direito: vem pelo topo e desce pela lateral direita
    vem, x, y = andar(w - 28, 3, 1, 0, 25, (2, 5))
    desce, _, _ = andar(x, y, 0, 1, 18, (w - 6, w - 3))
    caule(vem + desce)

    # contorno escuro em volta de tudo, para a vinha ler bem sobre a pedra
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    for y in range(h):
        for x in range(w):
            if planta.getpixel((x, y))[3]:
                continue
            for ox, oy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                qx, qy = x + ox, y + oy
                if 0 <= qx < w and 0 <= qy < h and planta.getpixel((qx, qy))[3]:
                    img.putpixel((x, y), rgba(CONTORNO))
                    break
    img.alpha_composite(planta)
    return img


ESTILOS = {
    "Botao_Normal": {
        "moldura": (OURO, BRONZE_CLARO, BRONZE, BRONZE_ESCURO, BRONZE_SOMBRA),
        "pedra": (PEDRA, PEDRA_CLARA, PEDRA_SOMBRA, PEDRA_LUZ, PEDRA_FUNDO),
        "placa": (CONTORNO, OURO, BRONZE_CLARO, BRONZE, BRONZE_ESCURO, BRASA_ESCURA, BRASA, BRASA_CLARA),
    },
    "Botao_Hover": {
        "moldura": (OURO_BRILHO, OURO, BRONZE_CLARO, BRONZE, BRONZE_ESCURO),
        "pedra": (PEDRA_CLARA, PEDRA_LUZ, PEDRA, PEDRA_LUZ, PEDRA_SOMBRA),
        "placa": (CONTORNO, OURO_BRILHO, OURO, BRONZE_CLARO, BRONZE, BRASA, BRASA_CLARA, BRASA_BRILHO),
    },
    "Botao_Pressionado": {
        "moldura": (BRONZE, BRONZE_ESCURO, BRONZE_ESCURO, BRONZE_SOMBRA, BRONZE_SOMBRA2),
        "pedra": (PEDRA_SOMBRA, PEDRA, PEDRA_FUNDO, PEDRA, ESCURO),
        "placa": (CONTORNO, BRONZE, BRONZE_ESCURO, BRONZE_SOMBRA, BRONZE_SOMBRA2, BRASA_ESCURA, BRASA_ESCURA, BRASA),
        "afundado": True,
    },
    "Botao_Desabilitado": {
        "moldura": (FERRO_LUZ, FERRO_CLARO, FERRO, FERRO_ESCURO, FERRO_SOMBRA),
        "pedra": (PEDRA_FRIA, PEDRA_FRIA_CLARA, PEDRA_FRIA_SOMBRA, PEDRA_FRIA_CLARA, FERRO_SOMBRA),
        "placa": (CONTORNO, FERRO_LUZ, FERRO_CLARO, FERRO, FERRO_SOMBRA, FERRO_SOMBRA, FERRO_ESCURO, FERRO),
    },
}


# ---------------------------------------------------------------- paineis

def moldura_fina(w, h, moldura, fundo, fundo_claro, semente, cantos=None):
    """Painel 9-slice: moldura de 4 texels e interior escuro quase liso,
    para repetir sem emenda visivel em qualquer tamanho."""
    luz, clara, base, escura, sombra = moldura
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    rnd = random.Random(semente)
    for y in range(h):
        for x in range(w):
            a = anel(x, y, w, h)
            if a < 0:
                continue
            l = lado(x, y, w, h)
            de_cima = l in ("cima", "esq")
            if a == 0:
                cor = CONTORNO
            elif a == 1:
                cor = (luz if l == "cima" else clara) if de_cima else sombra
            elif a == 2:
                cor = base
            elif a == 3:
                cor = escura
            elif a == 4:
                cor = CONTORNO
            else:
                cor = fundo_claro if rnd.random() < 0.06 else fundo
            img.putpixel((x, y), rgba(cor))
    # sombra interna embaixo da moldura de cima
    for x in range(5, w - 5):
        img.putpixel((x, 5), rgba(CONTORNO))
    if cantos:
        for ex in (False, True):
            for ey in (False, True):
                placa_canto(img, ex, ey, cantos)
    return img


PAINEIS = {
    # cartao e paineis comuns: bronze escuro; o ativo fica em ouro
    "Painel_Normal": ((BRONZE_CLARO, BRONZE, BRONZE_ESCURO, BRONZE_SOMBRA, BRONZE_SOMBRA2), FUNDO_PAINEL, FUNDO_PAINEL_CLARO),
    "Painel_Hover": ((OURO_BRILHO, OURO, BRONZE_CLARO, BRONZE, BRONZE_ESCURO), FUNDO_PAINEL_CLARO, "#30252a"),
    "Painel_Pressionado": ((BRONZE, BRONZE_ESCURO, BRONZE_SOMBRA, BRONZE_SOMBRA2, CONTORNO), FUNDO_SLOT, FUNDO_PAINEL),
    "Painel_Desabilitado": ((FERRO_LUZ, FERRO_CLARO, FERRO, FERRO_ESCURO, FERRO_SOMBRA), "#17151a", "#1d1b20"),
}


def slot(titular):
    """Nicho da equipe. O do titular tem moldura de ouro e gemas nos cantos."""
    w = h = 40
    if titular:
        moldura = (OURO_BRILHO, OURO, BRONZE_CLARO, BRONZE, BRONZE_ESCURO)
        cantos = (CONTORNO, OURO_BRILHO, OURO, BRONZE_CLARO, BRONZE, BRASA, BRASA_CLARA, BRASA_BRILHO)
    else:
        moldura = (FERRO_LUZ, FERRO_CLARO, FERRO, FERRO_ESCURO, FERRO_SOMBRA)
        cantos = None
    img = moldura_fina(w, h, moldura, FUNDO_SLOT, "#191416", 5)
    # o nicho e fundo: sombra mais grossa em cima e na esquerda
    for x in range(5, w - 5):
        img.putpixel((x, 6), rgba("#0f0c0e"))
    for y in range(5, h - 5):
        img.putpixel((5, y), rgba(CONTORNO))
    if cantos:
        for ex in (False, True):
            for ey in (False, True):
                placa_canto(img, ex, ey, cantos)
    return img


def barra_fundo():
    """Trilho da barra de atributo: 9-slice com borda de 2 texels."""
    w, h = 12, 6
    img = Image.new("RGBA", (w, h), rgba(CONTORNO))
    for y in range(1, h - 1):
        for x in range(1, w - 1):
            img.putpixel((x, y), rgba(FUNDO_SLOT))
    for x in range(1, w - 1):
        img.putpixel((x, h - 2), rgba("#2a2226"))
    return img


def barra_preenchimento():
    """Preenchimento em tons de cinza, tingido pela cor do atributo no Unity."""
    w, h = 8, 4
    tons = ["#ffffff", "#e6e6e6", "#c8c8c8", "#9a9a9a"]
    img = Image.new("RGBA", (w, h))
    for y in range(h):
        for x in range(w):
            img.putpixel((x, y), rgba(tons[y]))
    return img


def selo():
    """Plaquinha clara 9-slice (borda 3) para os selos J1 e J2, tingida no Unity."""
    w, h = 12, 10
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    for y in range(h):
        for x in range(w):
            a = anel(x, y, w, h)
            if a < 0:
                continue
            if a == 0:
                cor = CONTORNO
            elif a == 1:
                cor = "#ffffff" if y < h // 2 else "#a0a0a0"
            else:
                cor = "#d8d8d8"
            img.putpixel((x, y), rgba(cor))
    return img


def seta():
    """Seta para baixo dos ciclos de vantagem, em bronze."""
    desenho = [
        "..OOOO..",
        "..OLBO..",
        "..OLBO..",
        "OOOLBOOO",
        "OLLLBBDO",
        ".OLBBDO.",
        "..OBDO..",
        "...OO...",
    ]
    return de_texto(desenho, {"O": CONTORNO, "L": OURO, "B": BRONZE_CLARO, "D": BRONZE})


def seta_ciclo():
    """Seta longa dos triangulos de vantagem, apontando para a direita."""
    desenho = [
        "..............OO....",
        "..............OLO...",
        "OOOOOOOOOOOOOOOLLO..",
        "OLLLLLLLLLLLLLLBBLO.",
        "OBBBBBBBBBBBBBBBBBLO",
        "ODDDDDDDDDDDDDDBBDO.",
        "OOOOOOOOOOOOOOODDO..",
        "..............ODO...",
        "..............OO....",
    ]
    return de_texto(desenho, {"O": CONTORNO, "L": OURO_BRILHO, "B": BRONZE_CLARO, "D": BRONZE})


# ----------------------------------------------------------------- icones

def de_texto(linhas, cores):
    h, w = len(linhas), max(len(l) for l in linhas)
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    for y, linha in enumerate(linhas):
        for x, c in enumerate(linha):
            if c != ".":
                img.putpixel((x, y), rgba(cores[c]))
    return img


def contornar(img):
    """Contorno escuro de 1 texel em volta da forma (4 vizinhos)."""
    w, h = img.size
    out = img.copy()
    for y in range(h):
        for x in range(w):
            if img.getpixel((x, y))[3]:
                continue
            for ox, oy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                qx, qy = x + ox, y + oy
                if 0 <= qx < w and 0 <= qy < h and img.getpixel((qx, qy))[3]:
                    out.putpixel((x, y), rgba(CONTORNO))
                    break
    return out


def centralizar(img, tam):
    forma = img.crop(img.getbbox())
    out = Image.new("RGBA", (tam, tam), (0, 0, 0, 0))
    out.alpha_composite(forma, ((tam - forma.width) // 2, (tam - forma.height) // 2))
    return out


def icone_fogo():
    d = [
        "......r.......",
        "......rr......",
        ".....rYr...r..",
        "....rrYYr.rr..",
        "....rYYYr.rYr.",
        ".r..rYWYYrrYr.",
        ".rr.rYWWYYYYr.",
        "rYrrYYWWWYYYr.",
        "rYYYYWWWWWYYr.",
        "rYYYWWWWWWYYr.",
        "RrYYWWWWWYYrR.",
        ".RrYYYYYYYrR..",
        "..RRrrrrrRR...",
    ]
    return d, {"R": BRASA_ESCURA, "r": BRASA, "Y": BRASA_CLARA, "W": BRASA_BRILHO}


def icone_agua():
    d = [
        "......b.......",
        ".....bbb......",
        ".....bbb......",
        "....bcbbb.....",
        "...bcwbbbb....",
        "...bcwcbbbb...",
        "..bcwccbbbbB..",
        "..bcwcbbbbbB..",
        "..bccbbbbbbB..",
        "..bcbbbbbbBB..",
        "...bbbbbbBB...",
        "....BBBBBB....",
    ]
    return d, {"B": AGUA_SOMBRA, "b": AGUA, "c": AGUA_CLARA, "w": AGUA_BRILHO}


def icone_planta():
    d = [
        "..........gll.",
        "........gglll.",
        "......gglllyl.",
        ".....ggllllyl.",
        "....ggllllyll.",
        "...gglllyylll.",
        "...gglllylllg.",
        "...gllyylllgg.",
        "...glyllllgg..",
        "...gylllggg...",
        "...ylgggg.....",
        "..G...........",
        ".G............",
    ]
    return d, {"G": FOLHA_ESCURA, "g": FOLHA, "l": FOLHA_CLARA, "y": FOLHA_BRILHO}


def icone_guerreiro():
    d = [
        "...........ww",
        "..........wsS",
        ".........wsS.",
        "........wsS..",
        ".......wsS...",
        "......wsS....",
        ".....wsS.....",
        ".g..wsS......",
        ".gg.sS.......",
        "..gggD.......",
        "...cgg.......",
        "..c..gg......",
        "gc.....g.....",
        "gg...........",
    ]
    return d, {"w": ACO_BRILHO, "s": FERRO_LUZ, "S": FERRO, "g": BRONZE_CLARO, "D": BRONZE, "c": CABO}


def icone_mago():
    d = [
        "........m...",
        ".......mP...",
        "......mmP...",
        "......mWP...",
        ".....mmPP...",
        ".....mmPPP..",
        "....mmmPPP..",
        "....mmPPPPP.",
        "...GGGGGGGD.",
        "..mmmmPPPPPP",
        "mmmmmmPPPPPP",
        ".EEEEEEEEEE.",
    ]
    return d, {"m": MAGIA, "P": MAGIA_SOMBRA, "W": MAGIA_BRILHO, "G": BRONZE_CLARO, "D": BRONZE, "E": MAGIA_ESCURA}


def icone_tanque():
    d = [
        "gggggggggggg",
        "gLLLLLsSSSSd",
        "gLwLLLsSSSSd",
        "gLLLLLsSSSSd",
        "gLLLLLsSSSSd",
        "gLLLLggSSSSd",
        "gLLLggggSSSd",
        ".gLLLggSSSd.",
        ".gLLLLsSSSd.",
        "..gLLLsSSd..",
        "...gLLsSd...",
        "....gLSd....",
        ".....dd.....",
    ]
    return d, {"g": BRONZE_CLARO, "d": BRONZE, "L": FERRO_LUZ, "w": ACO_BRILHO, "s": FERRO, "S": FERRO_ESCURO}


ICONES = {
    "Icone_Fogo": icone_fogo,
    "Icone_Agua": icone_agua,
    "Icone_Planta": icone_planta,
    "Icone_Guerreiro": icone_guerreiro,
    "Icone_Mago": icone_mago,
    "Icone_Tanque": icone_tanque,
}


def icone(nome):
    desenho, cores = ICONES[nome]()
    return centralizar(contornar(centralizar(de_texto(desenho, cores), 18)), 16)


def main():
    os.makedirs(SAIDA, exist_ok=True)
    for nome, estilo in ESTILOS.items():
        botao(estilo).save(os.path.join(SAIDA, nome + ".png"))
    trepadeira().save(os.path.join(SAIDA, "Trepadeira.png"))
    for nome, (moldura, fundo, claro) in PAINEIS.items():
        moldura_fina(48, 48, moldura, fundo, claro, 3).save(os.path.join(SAIDA, nome + ".png"))
    slot(False).save(os.path.join(SAIDA, "Slot.png"))
    slot(True).save(os.path.join(SAIDA, "Slot_Titular.png"))
    barra_fundo().save(os.path.join(SAIDA, "Barra_Fundo.png"))
    barra_preenchimento().save(os.path.join(SAIDA, "Barra_Preenchimento.png"))
    selo().save(os.path.join(SAIDA, "Selo.png"))
    seta().save(os.path.join(SAIDA, "Seta.png"))
    seta_ciclo().save(os.path.join(SAIDA, "Seta_Ciclo.png"))
    os.makedirs(os.path.join(SAIDA, "Icones"), exist_ok=True)
    for nome in ICONES:
        icone(nome).save(os.path.join(SAIDA, "Icones", nome + ".png"))
    print("sprites gerados em", SAIDA)


if __name__ == "__main__":
    main()

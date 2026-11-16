using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Build WebGL do jogo, para rodar no navegador e publicar no GitHub Pages.
// Pelo menu: Combo Tactics > Build WebGL. Pela linha de comando:
//   unity build ComboTactics --target WebGL --execute-method ConstruirWebGL.Construir
public static class ConstruirWebGL
{
    public const string SAIDA = "Builds/WebGL";

    [MenuItem("Combo Tactics/Build WebGL")]
    public static void Construir()
    {
        Configurar();

        string[] cenas = EditorBuildSettings.scenes.Where(c => c.enabled).Select(c => c.path).ToArray();
        BuildReport relatorio = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = cenas,
            locationPathName = SAIDA,
            target = BuildTarget.WebGL,
            options = BuildOptions.None,
        });

        BuildSummary resumo = relatorio.summary;
        Debug.Log("Build WebGL: " + resumo.result + ", " + resumo.totalSize / (1024 * 1024) + " MB, " + resumo.totalTime);
        if (resumo.result != BuildResult.Succeeded && Application.isBatchMode)
            EditorApplication.Exit(1);
    }

    // As opcoes ficam no ProjectSettings; aplicar de novo aqui garante que todo build sai igual.
    public static void Configurar()
    {
        PlayerSettings.productName = "Combo Tactics";

        // tela fixa 1280x720: a Pixel Perfect Camera (640x360) fica em escala inteira 2x
        PlayerSettings.defaultWebScreenWidth = 1280;
        PlayerSettings.defaultWebScreenHeight = 720;

        // o GitHub Pages nao envia o header Content-Encoding, entao o proprio
        // loader descompacta os arquivos (Decompression Fallback)
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.decompressionFallback = true;
        PlayerSettings.WebGL.dataCaching = true;
        PlayerSettings.WebGL.template = "APPLICATION:Default";
    }
}

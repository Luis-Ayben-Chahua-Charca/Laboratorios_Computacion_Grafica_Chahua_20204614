// FIX: antes esto era una clase estática con constantes de tipo string
// (MisionIds.RecogerHoz = "recoger_hoz"). Eso protegía typos en el código
// SOLO si te acordabas de usar la constante en vez de escribir el string a
// mano (y de hecho, CampoAvena y ManoJugador no la usaban). Un enum protege
// en ambos lados: en código, el compilador no te deja comparar contra un
// valor que no existe; y en el Inspector (ej. el campo "id" de un
// ObjetivoInicial en un StageData), Unity lo muestra como un desplegable en
// vez de una caja de texto libre — ya no se puede tipear "cortar_pareho".
public enum MisionId
{
    RecogerHoz,
    CortarAvena,
    CortarParejo,
    EntregarPasto,
    BuscarCuerda,
    RepararCerca,
    IrCocina,
    DejarHoz,
    // ... vas sumando acá cada nuevo eslabón, a medida que armés la escena
}
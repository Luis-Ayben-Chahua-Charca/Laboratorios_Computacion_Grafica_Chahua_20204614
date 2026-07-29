// Separado de MisionId a propósito: "qué objeto es esto" y "qué misión
// completa al recogerse" son dos cosas distintas. Antes reutilizábamos
// MisionId para identificar objetos agarrables en StageData.objetosEnMano,
// lo que hacía que el desplegable del Inspector mostrara las 8 misiones del
// juego en vez de los objetos que el jugador puede tener en mano.
public enum ObjetoId
{
    Hoz,
    Cuerda,
    // ... sumá acá cada objeto agarrable nuevo, siempre al final de la lista
}
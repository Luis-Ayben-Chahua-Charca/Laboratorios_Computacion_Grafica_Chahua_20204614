// Cualquier sistema de escena (CampoAvena, y a futuro CampoCerca,
// EscenaCocina, EscenaLampara...) que necesite un "salto de estado" para
// testing implementa esta interfaz. StageLoader busca automáticamente
// cualquier componente que la implemente y le pasa la etapa completa —
// así StageLoader nunca necesita conocer el nombre de la clase concreta,
// y sumar una escena nueva no requiere tocar StageLoader.
public interface IEstadoDebug
{
    void AplicarEstadoDebug(StageData etapa);
}
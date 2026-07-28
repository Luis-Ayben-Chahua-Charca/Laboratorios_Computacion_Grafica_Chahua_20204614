// Mismo patrón que MisionId: un enum en vez de strings sueltos para los
// tipos de recurso del inventario ("Avena", y a futuro Leche, Huevos, Leña,
// etc.). Protege contra typos en código y convierte los campos del
// Inspector (IconoRecurso.tipo, ItemInicial.tipo en StageData) en
// desplegables en vez de cajas de texto libre.
public enum TipoRecurso
{
    Avena,
    Cuerda,
    Encendedor,
    // ... vas sumando acá cada nuevo recurso, a medida que armés la escena
}

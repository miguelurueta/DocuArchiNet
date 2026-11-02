# DOC-111: consulta y exportación locales, 2026-10-28

HEAD `b2e93b726f221868cf2b82d43200a8e568871117`, workspace con cambios sin commit. Avance **81/130 tareas completas**, con implementación local de 11.1–11.5. El almacenamiento existente permanece intacto; no se habilitan adaptadores documentales.

El repositorio único añade el parcial Consulta y el puerto IConsultaImportacionRepository. El servicio de lectura reconstruye identidad/destino desde la referencia autorizada, verifica snapshot/recursos actuales y proyecta campos públicos explícitos. El ASMX expone ConsultarLote, ConsultarRecursosLote y ExportarPaginaLote mediante el dispatcher operativo existente; no eleva administración ni añade otro handler/repositorio/tablas.

El estado incluye revisión, totales originales, señal de detención e indicadores separados de intentos e incertidumbre. No recalcula ni modifica el estado agregado. Las acciones anunciadas son CONSULTAR/EXPORTAR_PAGINA tras autorización; ninguna etiqueta habilita avances, detención o reconciliación todavía pendientes.

Las páginas usan numeración desde 1 y orden ascendente por ID, conforme al puerto previo. El contrato 02 registra la decisión frente a la propuesta inicial de cursor opaco. Filtros raíz/estado/recepción parametrizados y cerrados; totalFiltrado y totalOriginal separados. Cada página usa transacción RepeatableRead para sus SELECT de lote/cuenta/filas; páginas sucesivas no representan un snapshot global. No se añaden FOR UPDATE, actualizaciones de acceso, eventos ni escrituras por consultar.

El CSV exporta explícitamente una página con sus metadatos, sin bucle de todo el lote ni omisión silenciosa de filas. Protege fórmulas con prefijos directos, espacios/controles y marcas invisibles iniciales, comillas, separadores y saltos de línea. La proyección de lectura/CSV omite staging, tokens, claves internas, valores efectivos y mensajes/evidencia libres del adaptador; los códigos públicos provienen de lista cerrada.

La política nueva `Doc111.Consulta.MaximoPagina` debe configurarse expresamente con entero positivo hasta 200; el techo es de transporte y no una cuota del inventario. Sin política válida la consulta queda indisponible. Se conserva la política general requerida por el guard. No se modificó Web.config ni se eligió un valor de ambiente. La acción CONSULTAR reutiliza de forma restrictiva las facultades de nivel/gabinete actuales; no se concede acceso nuevo de solo lectura a otros actores.

Verificación local PASS:

- Arnés .NET Framework existente: lectura de lote/páginas/CSV con doble de solo lectura; identidad ajena, permiso revocado, recurso denegado y destino movido rechazados; revisiones, totales y datos persistidos preservados.
- Tres recursos rechazados/inciertos/cancelados conservados al recorrer páginas; filtros separan cuenta seleccionada del inventario original; no se anuncian mutaciones ante diferentes estados de lote.
- Núcleo de consultas productivo con registros sintéticos y lector SQL explícito: predicados de usuario/empresa/nivel/lote, parámetros, orden ID, límite/offset, proyección de raíz/archivo, revisión y rechazo de filtros antes de consultar. No es una ejecución MySQL.
- Política ausente/inválida, tamaño de página excesivo, tipo/estado/referencia no canónicos y offset desbordado rechazados.
- ASMX con HttpContext sintético: los tres endpoints devuelven DTO tipado, usan el cuerpo JSON validado y rechazan token ausente, autoridad inyectada, facultad revocada y página inválida. La vía administrativa sigue denegada al importador.
- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: PASS, familias VB/C# incluyendo regresión previa.
- `node --test tests/doc111-importacion-carpetas-client.test.cjs tools/e2e/tests/doc88-production-document-upload-policy.test.cjs`: 15 pruebas locales PASS.
- Build Debug completo sin despliegue en `.tmp/doc111-consulta-build/`; log `.tmp/doc111-consulta-build.log`, código 0. OpenSpec estricto y `git diff --check` PASS.

**NO_EJECUTADO:** SQL/conexiones reales de este corte, aislamiento/paginación concurrente MySQL, pipeline HTTP/IIS, autenticación/E2E reales, descarga por navegador, limpieza, creación/almacenamiento/incorporación documental o despliegue. Las pruebas locales no acreditan ninguna de esas capacidades. 11.6–11.7 e interfaz continúan pendientes, al igual que las dependencias documentales del coordinador.

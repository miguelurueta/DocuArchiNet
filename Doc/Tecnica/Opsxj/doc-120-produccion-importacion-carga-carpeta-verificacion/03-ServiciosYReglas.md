# DOC-120 — ServiciosYReglas

## Corte de recepción — DOC120-REC01,2026-11-13

Tarea3.9: disponibilidad de creación por consulta, reutilizando la barrera de recepción y el ámbito completo, con dos SELECT de metadatos agrupados por página y lectura del plan. Se añade el booleano público recepcionListaParaCrear (false por defecto), sin endpoint ni esquema nuevo; pista interna excluida de JSON persistido. Cliente exige true, explica la recepción y acerca el envío. Sin cambios de creación/almacenamiento/SII/perfil Egresos ni efectos sobre lote31. Regresión local64/64 Node (incluye2 wrappers Framework/DDL) y compilación0 errores. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-recepcion-creacion-20261113.json. Avance25/29; cuatro tareas de autorización/aceptación/gobierno/entrega abiertas. Sin nueva prueba IIS autenticada; los apartados siguientes son cortes históricos y sus afirmaciones sin delta/DTO no describen esta corrección.

- Ticket: DOC-120
- Cambio OpenSpec: doc-120-produccion-importacion-carga-carpeta-verificacion
- Clasificacion: cross_cutting

## Servicios y reglas

Producto/servicios/repositorio sin delta desde8c0773d4. Consultar/CSV reautorizan y no reconcilian; coordinador/Barreras/Valores/RecuperacionV2 conservan revisiones, integridad, valores diferidos, fecha estable y fases. INCIERTO no se repite; stop no revierte lo confirmado.

Regresión actual del arnés Framework y contraste DDL:2 wrappers Node PASS dentro del corte5/5 inicial. Se fijan114 entradas del arnés (fuentes y contratos SQL); las fuentes compiladas usan disco temporal/dobles SQL/HTTP y no una BD real. Sus casos internos no se agregan al total Node.

INT-02 reutiliza exclusivamente la ausencia histórica de ra_integracion_evento y restauración/carga individual. ValidarMetadatos tiene verificaciones de tablas/motor/columnas/índices/claves/reglas; la prueba local no acredita esquema instalado ni cada incompatibilidad real. No se requiere variante real adicional sin requisito/diferencia concreta; auxiliar compatible corregido por separado.

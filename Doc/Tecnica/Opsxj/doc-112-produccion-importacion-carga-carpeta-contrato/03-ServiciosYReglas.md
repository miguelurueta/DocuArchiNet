# DOC-112 — Entrega documental de contratos

- Ticket: DOC-112
- Cambio OpenSpec: doc-112-produccion-importacion-carga-carpeta-contrato
- Clasificacion: cross_cutting (Transversal)

## Servicios y reglas

No se implementan servicios nuevos. Se contrastan los contratos con ServicioPreparacionImportacion, ServicioRecepcionImportacion, ServicioConsultaImportacion y CoordinadorImportacionCarpetas y sus parciales. Las responsabilidades conservan 07-A → 06 → 07-B y un único repositorio operativo. Los adaptadores 08/09 son internos; 10 coordina una fase explícita y 11 consulta/exporta sin ejecutar efectos.

Se reutilizan configuración publicada inmutable, reglas PREFLIGHT/TRAS_EXPEDIENTE, identidad cliente distinta de PK, idempotencia y revisiones, barreras EXPEDIENTE/LOTE/PARCIAL y evidencia para resultados inciertos. La revisión precisó tipo/nulabilidad e índices documentados, sin generar DDL. Un timeout no concede replay; recibir bytes no confirma incorporación. Los códigos de formato legacy con signo no se confunden con IDs UInt32.

Detalle y fuentes: [modelo](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/01-modelo-datos.md), [contratos](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/02-contratos-operacion.md) y [estados/recuperación](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/03-estados-recuperacion.md). Capacidad/retención permanecen en [decisiones P04/P05](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/04-decisiones-y-pendientes.md).

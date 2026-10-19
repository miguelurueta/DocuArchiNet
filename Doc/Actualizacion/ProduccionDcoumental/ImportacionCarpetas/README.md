# Importación documental por carpetas

Documentación de la integración propuesta para crear expedientes a partir de carpetas y almacenar sus documentos mediante perfiles configurables. SII permanece intacto.

## Diseño técnico

- [Modelo, contratos, estados y decisiones](Diseno/README.md): primera entrega documental del prompt 04. Ambos ámbitos de integridad están aprobados y son configurables en el perfil; no hay tablas nuevas creadas ni SQL aplicado.

## Prototipo visual de referencia

- [Abrir prototipo HTML interactivo](Prototipo/importacion-carpetas.html): perfil, selección de carpetas, revisión y resultados simulados.
- [Guía de uso e integración ASP.NET](Prototipo/README.md): alcance, capturas, pruebas y componentes que deben conectarse al servidor.

## Prompt de implementación

Leer primero la [guía de ejecución y dependencias](00-guia-ejecucion-prompts.md). El prompt 03 es el alcance general; los especializados lo dividen en entregas, no añaden implementaciones paralelas. Los scripts SQL se generan en sus entregas y se aplican manualmente por el equipo.

- [Perfil de importación](02-prompt-implementacion-perfil-importacion.md): administración, reglas, publicación y selección del perfil. La ejecución masiva queda fuera de esta primera entrega.
- [Carga por carpetas](03-prompt-implementacion-carga-carpetas.md): selección, revisión, recepción, expedientes, almacenamiento y recuperación. Depende del perfil publicado y exige SQL MySQL 5.1 para ejecución manual.

| Prompt especializado | Entrega |
|---|---|
| [04 — Diseño físico y contratos](04-prompt-diseno-fisico-contratos.md) | Modelo, contratos, estados y decisiones pendientes. |
| [05 — Ventana integrada en Producción](05-prompt-ventana-produccion.md) | Interfaz desde el árbol existente y conexión a servicios. |
| [06 — Validación y plan](06-prompt-validacion-plan.md) | Preflight, valores resueltos y plan sin efectos documentales. |
| [07 — Lotes y recepción](07-prompt-lotes-recepcion.md) | Persistencia, idempotencia, SQL y transferencia segura. |
| [08 — Adaptador de expedientes](08-prompt-adaptador-expedientes.md) | Creación/verificación bajo el nivel confirmado. |
| [09 — Almacenamiento e incorporación](09-prompt-almacenamiento-incorporacion.md) | Índices, gabinete, vínculo y reconciliación. |
| [10 — Coordinación y recuperación](10-prompt-coordinacion-recuperacion.md) | Fases, concurrencia, integridad configurable y reanudación. |
| [11 — Seguimiento y operación](11-prompt-seguimiento-operacion.md) | Resultados, árbol, exportación y limpieza segura. |
| [12 — Verificación integral](12-prompt-verificacion-integral.md) | Evidencia funcional, regresión y capacidad. |
| [13 — Ejecutor desatendido, opcional](13-prompt-ejecutor-desatendido-opcional.md) | Evolución durable para archivos ya recibidos. |

La política predeterminada exige todos los archivos, con modo parcial configurable. El usuario aprobó soportar ambos ámbitos —por expediente y por lote— como configuración del perfil. En modo completo se exige elegir uno al publicar; la elección queda fija en el lote. La creación de estos prompts no acredita componentes implementados ni tablas creadas.

## Exploración

- [Revisión estructural de los prompts 02–13](Exploracion/07-revision-estructural-prompts.md): correcciones, resultados de `opsxj:prompt-review` y límites de la validación.

- [Diagnóstico de coherencia e integración de prompts](Exploracion/06-diagnostico-coherencia-prompts.md): matriz de hallazgos, correcciones documentales, riesgos pendientes y verificación ejecutada.

- [Arquitectura y funcionamiento](Exploracion/00-exploracion-importacion-masiva-por-carpetas.md).
- [Diagnóstico de reutilización de tablas](Exploracion/01-diagnostico-reutilizacion-tablas.md).
- [Evidencia del esquema](Exploracion/02-evidencia-esquema-perfil.json).
- [Validación de base de datos y definición del perfil](Exploracion/03-validacion-bd-y-definicion-perfil.md).
- [Diagrama de flujo técnico del perfil](Exploracion/04-diagrama-flujo-tecnico-perfil.md).
- [Contexto técnico de carga por carpetas](Exploracion/05-contexto-tecnico-carga-carpetas.md): revisión del JavaScript, entradas ASP.NET y operaciones documentales reutilizables.

La documentación de diagnóstico y corrección de la carga individual existente permanece en el directorio hermano `CargaDocumento`.

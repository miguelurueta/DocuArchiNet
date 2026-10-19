# Prompt 04 — Diseño físico y contratos

## Encargo

Lee `00-guia-ejecucion-prompts.md`, los prompts 02/03 y los documentos 00–06 de `Exploracion/`, incluido el diagnóstico de coherencia. Concreta el diseño común de la importación antes de que cada componente establezca contratos distintos. Esta entrega produce diseño revisable, no código de aplicación ni cambios de base de datos. Si ya existe `Diseno/`, continúa desde esos artefactos y sus pendientes; no los reemplaces ignorando decisiones/evidencia.

## Trabajo

1. Verifica esquema observado, tipos, índices y operaciones actuales en los archivos fuente; distingue evidencia de hipótesis. La evidencia MySQL es de un esquema concreto, no de todos los ambientes.
2. Aplicar la decisión aprobada: soportar EXPEDIENTE y LOTE configurables en la versión del perfil. COMPLETO_OBLIGATORIO requiere elegir ámbito al publicar; PARCIAL_PERMITIDO usa NO_APLICA. Conservar modo/ámbito en plan y lote y diseñar ambas barreras con el mismo coordinador. No volver a preguntar esta decisión.
3. Define responsabilidades de perfil, versión y regla; plan, lote, raíz, archivo e intentos/eventos necesarios. Justifica cada tabla y relación; no fuerces un número fijo de tablas. No duplicar catálogos existentes.
4. Documenta PK/FK, ámbito, unicidad, revisión de concurrencia, tipos/longitudes, estados y fechas. Prevé nombres y rutas largas sin índices incompatibles con MySQL 5.1; si usas huellas para claves, define algoritmo y verificación de colisiones. No presupongas unicidad global del nombre visible.
5. Define identidad estable de raíz/archivo, correlación de operaciones, idempotencia de confirmación y fases. Misma clave con contenido diferente debe producir conflicto; repetir una solicitud idéntica devuelve su resultado previo.
6. Define matriz de transiciones de lote, raíz y archivo. Separa recepción, efecto documental, incorporación y resultado incierto. Define revisiones/exclusión, reclamación de trabajo y recuperación tras pérdida del proceso, sin garantizar exactamente una vez por tener una fila de estado.
7. Define contratos de apertura desde el árbol, perfiles compatibles, preparación, confirmación, recepción, ejecución acotada, consulta, cancelación y reconciliación. Especifica entradas, salidas, autoridad, límites, revisión, errores, reintentos y operaciones sin efectos documentales.
8. Define puertos de catálogo/perfil, repositorio operativo, expedientes, archivos y almacenamiento. Identifica funciones existentes candidatas, datos de sesión, efectos y capacidad real de reconciliación. No inventes una correlación que el almacenamiento legacy no conserve.
9. Concreta cómo se conserva la configuración efectiva: representación relacional o serialización versionada compatible, límites y validación. Una referencia a un borrador mutable no sirve como snapshot.
10. Registra volumen, navegadores y política de temporales todavía no confirmados. No convertir los 500 archivos/25 MB del prototipo en capacidad productiva.

## Entregables

Aplicar los contratos transversales de la guía 00: fijar propietario por endpoint/repositorio, secuencia 07-A → 06 → 07-B, errores globales frente a individuales, valores diferidos, disponibilidad del perfil, permisos por fase y recuperación verificable. Caracterizar nombre documental, copias consumibles y escritores externos del expediente/índice antes de declarar reusable un método legacy. Decidir representación y momento de resolución de IDs/consecutivos aún inexistentes y fechas generadas. No cerrar diseño de publicación con campos requeridos que solo puedan existir después de crear el expediente sin una fuente diferida explícita.

- `Diseno/01-modelo-datos.md`: diccionario físico, relaciones, índices y responsables de scripts.
- `Diseno/02-contratos-operacion.md`: contratos HTTP y de aplicación, ejemplos sin datos sensibles y matriz de errores.
- `Diseno/03-estados-recuperacion.md`: transiciones, exclusión, política completa/parcial y ventanas de fallo.
- `Diseno/04-decisiones-y-pendientes.md`: evidencia, decisiones confirmadas, pendientes y secuencia técnica.

Si las convenciones existentes exigen otros nombres, documenta la equivalencia. No generes migraciones especulativas desvinculadas del diseño; la implementación 02/07 entrega los scripts ejecutables correspondientes.

## Aceptación

Dos implementadores pueden construir cliente y servidor sin inventar campos o estados. Cada efecto tiene identidad, resultado confirmado y tratamiento de resultado incierto. Ambos ámbitos de integridad están aprobados y sus barreras deben quedar especificadas y probadas sin duplicar motores. El modelo conserva nivel de origen, perfil/versión, inventario original, política y trazabilidad documental. Todos los contratos respetan SII y MySQL 5.1.

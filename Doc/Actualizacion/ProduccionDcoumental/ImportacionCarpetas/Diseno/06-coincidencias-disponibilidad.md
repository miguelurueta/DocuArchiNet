# Coincidencias y disponibilidad — decisiones aprobadas

El usuario aprobó estas dos políticas el 2026-10-20. Esta aprobación no equivale a aprobar el refinamiento completo ni ejecutar implementación, DDL o pruebas reales.

## Disponibilidad del perfil

Desactivar impide nuevas selecciones y nuevas confirmaciones de importación. Los lotes ya confirmados conservan su snapshot y pueden continuar; no se suspenden por desactivación del perfil. Publicar otra versión tampoco los cambia. Para detener un lote se usa su acción explícita autorizada, en un punto seguro, conservando efectos confirmados.

La pérdida de autorización sí impide nuevos efectos afectados. No confundir perfil inactivo con permisos revocados. Consulta y reconciliación exigen autorización vigente; ninguna rama concede acceso por tener un lote anterior.

| Situación | Comportamiento |
|---|---|
| Perfil inactivo, plan aún no confirmado | Rechazar confirmación; no crear lote ejecutable. |
| Perfil desactivado después de confirmar | Lote sigue con snapshot original y permisos vigentes. |
| Confirmación y desactivación concurrentes | Ordenar ambas mediante control transaccional de la cabecera: si desactivar se confirma primero, rechazar nueva confirmación; si el lote se confirma primero, se considera previo. |
| Repetición de confirmación ya persistida | Recuperar el mismo lote autorizado; no tratar el reintento como confirmación nueva ni crear otro lote. |
| Detención explícita o pérdida de permiso | Detener nuevos efectos en punto seguro y conservar resultados; no prometer rollback global. |
| Reactivación | Permitir nuevas selecciones/confirmaciones tras validaciones vigentes; no sustituir versiones ni reanudar automáticamente lotes detenidos. |

DOC-110 administra disponibilidad e historial. Preparación/confirmación y coordinador implementan las consecuencias de ejecución en sus entregas; no añadir control de lotes al servicio del perfil.

## Coincidencias de expediente

La versión del perfil conserva una política explícita: `BLOQUEAR` por defecto o `REUTILIZAR_VALIDADO`. El servicio aplica el default al crear configuración nueva, lo persiste y lo muestra; publicación exige valor reconocido, no ausencia ambigua. El plan/lote guarda la política efectiva en su snapshot. No cambia por editar/publicar otro perfil.

| Resultado de búsqueda autorizada | BLOQUEAR | REUTILIZAR_VALIDADO |
|---|---|---|
| Ningún expediente coincidente | Proponer creación conforme a datos y permisos válidos | Igual |
| Un expediente coincidente | Conflicto; no crear duplicado ni incorporar allí | Reutilizar solo si cumple las condiciones de abajo |
| Más de un candidato | Conflicto, sin elegir primero | Conflicto, sin elegir primero |
| Resultado incierto de creación anterior | Reconciliar antes de crear/reintentar | Igual; no resolver incertidumbre mediante coincidencia aproximada |

Condiciones acumulativas de reutilización:

1. Coincidencia única según identidad documental caracterizada; nunca solo nombre de carpeta/expediente.
2. Empresa, estructura destino y clasificación compatibles.
3. Estado que permita incorporación y permisos vigentes.
4. Identidad completa y relación de destino verificadas, sin modificar datos existentes para hacerlos coincidir.

El lector legacy `Verfica_Existencia_Codigo_Unico_Expediente` consulta código, empresa, volumen y área; no demuestra por sí solo identidad suficiente, unicidad física ni autorización de reutilización. El adaptador 08 debe caracterizar la identidad real y su comparación/collation antes de habilitar reutilización operativa. No convertir una consulta previa en garantía contra carreras con otros escritores.

DOC-110 guarda y valida la opción configurada, sin ejecutar búsquedas ni creación durante publicación. Que una opción pueda configurarse no acredita que el adaptador operativo esté implementado. Preparación/ejecución rechazan capacidades no disponibles, sin degradar silenciosamente a otra política.

Reintentar una carpeta del mismo lote recupera primero su relación confirmada. No volver a aplicar BLOQUEAR contra el expediente creado por ese mismo intento como si fuese una nueva importación.

Reutilizar expediente no omite archivos, no concede sobrescritura documental y no resuelve coincidencias de documentos. Las políticas de documentos y la exigencia COMPLETO_OBLIGATORIO/PARCIAL_PERMITIDO permanecen independientes. En modo completo, un conflicto impide continuar conforme a la barrera EXPEDIENTE/LOTE seleccionada; no se transforma en omisión.

## Pruebas exigidas por entrega

- Perfil: default BLOQUEAR visible/persistido; ambas opciones válidas; valor desconocido impide publicar; versiones anteriores inmutables.
- Confirmación: perfil inactivo rechaza lote nuevo; reintento recupera lote ya confirmado; carrera confirmar/desactivar respeta orden confirmado.
- Adaptador: cero/uno/varios candidatos; empresa/nivel/clasificación/estado/permisos incompatibles; reintento propio; resultado incierto; no mutación correctiva de expediente existente.
- Coordinador: desactivar perfil no detiene lote previo; pérdida de permiso o detención explícita sí bloquean nuevos efectos; snapshot y resultados conservados.
- Integración: reutilización no reduce inventario de archivos ni declara completitud sin incorporación confirmada.

Estos son criterios de pruebas futuras. No se ejecutaron como parte de esta aprobación documental.

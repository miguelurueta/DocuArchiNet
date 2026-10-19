# Prompt 04 — Diseño físico y contratos

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior en ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1, respetando las fronteras existentes y el alcance de esta entrega.

## OBJETIVO Y ENCARGO

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

## ENTREGABLE FINAL

Aplicar los contratos transversales de la guía 00: fijar propietario por endpoint/repositorio, secuencia 07-A → 06 → 07-B, errores globales frente a individuales, valores diferidos, disponibilidad del perfil, permisos por fase y recuperación verificable. Caracterizar nombre documental, copias consumibles y escritores externos del expediente/índice antes de declarar reusable un método legacy. Decidir representación y momento de resolución de IDs/consecutivos aún inexistentes y fechas generadas. No cerrar diseño de publicación con campos requeridos que solo puedan existir después de crear el expediente sin una fuente diferida explícita.

- `Diseno/01-modelo-datos.md`: diccionario físico, relaciones, índices y responsables de scripts.
- `Diseno/02-contratos-operacion.md`: contratos HTTP y de aplicación, ejemplos sin datos sensibles y matriz de errores.
- `Diseno/03-estados-recuperacion.md`: transiciones, exclusión, política completa/parcial y ventanas de fallo.
- `Diseno/04-decisiones-y-pendientes.md`: evidencia, decisiones confirmadas, pendientes y secuencia técnica.

Si las convenciones existentes exigen otros nombres, documenta la equivalencia. No generes migraciones especulativas desvinculadas del diseño; la implementación 02/07 entrega los scripts ejecutables correspondientes.

## CRITERIOS DE ACEPTACIÓN

Dos implementadores pueden construir cliente y servidor sin inventar campos o estados. Cada efecto tiene identidad, resultado confirmado y tratamiento de resultado incierto. Ambos ámbitos de integridad están aprobados y sus barreras deben quedar especificadas y probadas sin duplicar motores. El modelo conserva nivel de origen, perfil/versión, inventario original, política y trazabilidad documental. Todos los contratos respetan SII y MySQL 5.1.

## CONTEXTO Y CONTRATO DE INTEGRACIÓN

Leer `00-guia-ejecucion-prompts.md` y los artefactos vigentes de `Diseno/`; distinguir propuestas de componentes existentes. Para esta entrega, documentar el contrato request/response, autoridad, errores, estados y efectos permitidos. Flujo de referencia: Cliente → entrada ASMX/ASHX (responsabilidad de Controller) → Service → Repository/adaptadores → respuesta; no imponer ASP.NET MVC a WebForms. Los adaptadores internos de 08/09 no se exponen como operaciones públicas independientes.

## PRECONDICIONES DE RUTAS

Antes de implementar, inventariar los archivos exactos en `Diseno/design.md`, con propietario (prompt), existentes/nuevos, responsabilidad y consumidores. Es un mapa de implementación complementario a `Diseno/01-modelo-datos.md` y `Diseno/02-contratos-operacion.md`, no otro diseño de contratos. Si existe un cambio OpenSpec, referenciar ese mapa desde su `design.md` sin duplicarlo. Verificar estas ubicaciones propuestas contra el repositorio; no afirmar que los archivos nuevos ya existen ni crear todas las capas por ejecutar esta entrega.

| Responsabilidad | Ubicación común propuesta | Propiedad |
|---|---|---|
| Modelos y puertos | `Modelo/ProduccionDocumental/ImportacionCarpetas/` | 04 define; cada entrega añade solo su contrato. |
| DTOs de entrada/salida | `DTOs/ProduccionDocumental/ImportacionCarpetas/` | 02/06/07/10/11 según operación. |
| Servicios y reglas | `Services/ProduccionDocumental/ImportacionCarpetas/` | 02 perfil; 06 plan; 07 recepción; 10 coordinación; 11 consulta/mantenimiento. |
| Repositorios | `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/` | 02 perfil; 07-A operación. Los demás consumen sus interfaces. |
| Adaptadores documentales | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/` | 08 expedientes; 09 almacenamiento e incorporación. |
| Servicio web nuevo | `webservice/WebServiceImportacionCarpetas.asmx` y `webservice/WebServiceImportacionCarpetas.asmx.vb` | Una fachada de las operaciones anteriores; sin reglas documentales en transporte. |
| Recepción multipart nueva | `generic_control/ImportacionCarpetasUpload.ashx` y `generic_control/ImportacionCarpetasUpload.ashx.vb` | 07-B; delega en su servicio; no sustituye el handler compartido. |
| Integración visual existente | `Gestion/WebFormProducionDocumental.aspx`, su code-behind `.aspx.vb` y `js/gestion/WebFormProducionDocumental.js` | 05 integra; 11 extiende resultados sobre la misma ventana. |
| JavaScript específico nuevo | `js/gestion/ImportacionCarpetas/` | 05 estado/transporte/renderizado; 11 seguimiento; nombres exactos en el inventario. |

Registrar los archivos nuevos de compilación y contenido en `GestionDocumental-Docuarchi.net.vbproj` según el tipo de recurso. Mantener namespaces y referencias compatibles con .NET Framework 4.6.1. Las rutas nuevas son destino de implementación, no evidencia de componentes existentes. El host opcional del 13 requiere su propio inventario tras decidir alojamiento; no inventar un ejecutable desplegado.

## RESTRICCIONES CRÍTICAS

No debe modificarse SII: código, tablas, configuración, contratos ni consumidores. Preservar carga individual y contratos legacy; no cambiar pruebas/huellas para ocultar regresiones. No crear otro árbol ni cambiar el nivel al elegir perfil. No usar autorregistro, tareas ficticias, valores fijos del ejemplo ni temporizadores del prototipo como implementación productiva.

Mantener COMPLETO_OBLIGATORIO configurable por EXPEDIENTE o LOTE; exigir elección al publicar y congelarla en plan/lote. PARCIAL_PERMITIDO usa NO_APLICA. No prometer rollback global: conservar efectos confirmados y reconciliar incertidumbre. Cada prompt implementa solo su responsabilidad según la guía 00; no duplicar motor, repositorios ni endpoints.

El equipo aplica SQL manualmente: MySQL 5.1, sin migraciones automáticas. Los scripts de perfil pertenecen a 02 y los operativos a 07; otros cambios justificados usan incrementos coordinados. No ejecutar SQL por tener conexión ni declarar aplicado un script por existir.

La tabla de rutas es una propuesta a verificar y documentar; esta entrega no crea modelos, DTOs, servicios ni modifica el vbproj.

## PRUEBAS OBLIGATORIAS

Recorrer manualmente contratos y transiciones con dos raíces: una lista y otra inválida; comprobar las dos barreras, valores diferidos, reintentos y responsabilidades 07-A → 06 → 07-B.

E2E no aplica a esta entrega: produce solo contratos y documentación, sin recorrido de usuario ejecutable. La QA manual revisa consistencia, enlaces, propiedad de tablas, fases y matriz de fallos. MSBuild no aplica porque no se modifica código; no atribuir resultados de compilación ni pruebas reales a este diseño. Documentar casos que las entregas de implementación deberán ejecutar.

## DOCUMENTACIÓN TÉCNICA

Actualizar documentación existente en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`: `Diseno/` para contratos y decisiones, y `Validacion/` para evidencia. Mantener guía 00 y consumidores sincronizados si cambia un contrato. No abrir una segunda fuente de verdad. Registrar archivos y funciones afectados, dependencia propietaria, casos ejecutados/no ejecutados y riesgos pendientes. Una revisión estructural PASS no certifica implementación ni ausencia de regresiones.

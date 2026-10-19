# Guía de ejecución de los prompts

Los prompts 02 y 03 describen el alcance funcional; 04–12 lo descomponen en entregas técnicas. El 13 es una evolución opcional. Ninguno de estos archivos acredita implementación, despliegue o creación de tablas.

## Instrucciones comunes obligatorias

Los prompts 02–13 incluyen ahora secciones explícitas de rol, objetivo, restricciones, rutas, aceptación, pruebas y evidencia para revisión individual con `opsxj:prompt-review --tech-profile legacy-webforms-vb --no-fix`. Las rutas nuevas son propuestas comunes: antes de implementar se concreta el inventario de archivos y propietarios en `Diseno/design.md`, complementario al diseño existente. No son componentes ya creados. La tabla repetida en los prompts delimita responsabilidades; no autoriza construir todas las capas en cada entrega.

Las pruebas del comportamiento implementado forman parte de cada entrega. El 12 consolida la verificación transversal y no posterga las comprobaciones de componentes anteriores. 04 solo produce documentación y no requiere E2E ni compilación; 07-A valida sus fundaciones sin depender de 06/UI y 07-B incorpora confirmación/recepción. Los adaptadores 08/09 se verifican por contrato sin endpoints de prueba que eludan 10. Una integración aún no disponible se declara pendiente. El 13 conserva su activación opcional explícita.

Resultado y límites de la revisión: [revisión estructural de prompts](Exploracion/07-revision-estructural-prompts.md).

Cada prompt especializado debe ejecutarse leyendo esta guía, el 03 y sus referencias indicadas. Actúa como arquitecto/desarrollador senior ASP.NET WebForms, VB.NET, JavaScript y MySQL. Revisa AGENTS.md, los cambios locales y el estado real de las dependencias antes de editar. No presupongas que ejecutar un prompt anterior significa que sus componentes existen o que su SQL fue aplicado.

- Respeta la aplicación existente: framework 4.6.1, WebForms, JavaScript, ASMX/ASHX y acceso a datos verificados en el repositorio. No migres de framework ni inventes infraestructura desplegada.
- SII permanece intacto: código, tablas, configuración, contratos y consumidores. No refactorizar ni extraer su implementación. Solo consumir operaciones compatibles sin cambiar su comportamiento. No usar tareas Workflow ficticias ni autorregistro como identidad del perfil nuevo.
- El usuario selecciona un nivel en el árbol existente de Producción y desde allí abre la ventana. El destino es explícito y de solo lectura dentro de ella; no se construye otro explorador ni creador de estructuras. El gabinete documental es distinto del nivel de Producción.
- Reutiliza catálogos y operaciones compatibles, evitando escritores directos que omitan las reglas documentales. Consulta el contexto técnico en `Exploracion/05-contexto-tecnico-carga-carpetas.md`.
- Datos de perfiles, reglas, niveles y permisos se verifican en servidor. Los ejemplos del prototipo no son valores de producción. No fijar separadores, tipologías, límites, responsables ni códigos de negocio.
- No imprimir ni guardar credenciales, cookies o cadenas de conexión. No modificar la base operativa por disponer de conexión. Inspecciones necesarias: solo lectura y limitadas al alcance.
- Todo cambio de esquema debe entregarse en un archivo SQL compatible con MySQL 5.1 para ejecución manual del equipo. No ejecutar migraciones automáticamente. InnoDB, tipos/collation compatibles; sin JSON nativo, utf8mb4, columnas generadas, CHECK como garantía efectiva ni sintaxis moderna no disponible. Documentar precondiciones, comprobaciones, repetición y recuperación de DDL parcial; no prometer rollback transaccional del esquema.
- No ejecutar E2E autenticado ni carga real sin autorización específica del ambiente/cuentas y lectura del runbook aplicable. Pruebas locales de contratos o dobles no equivalen a integración real.
- Implementa el alcance de la entrega hasta donde permitan sus dependencias. No simules éxito productivo para cubrir una dependencia ausente. Declara bloqueos concretos, completa partes independientes y registra qué falta integrar.
- Entrega rutas cambiadas, comportamiento, contratos, SQL si aplica y evidencia de verificación. Diferencia pruebas ejecutadas de pendientes. No modifiques cambios ajenos ni publiques despliegues.

## Política de integridad: decisión aprobada

El usuario indicó que todos los archivos deben pasar, con comportamiento configurable. El modo predeterminado será **COMPLETO_OBLIGATORIO**; un modo **PARCIAL_PERMITIDO** requiere configuración explícita en la versión publicada. El perfil, plan y lote deben conservar la política efectiva.

Decisión aprobada por el usuario: soportar ambos ámbitos, EXPEDIENTE y LOTE, configurables en la versión del perfil. En COMPLETO_OBLIGATORIO se exige seleccionar explícitamente uno antes de publicar; no existe un ámbito predeterminado silencioso. En PARCIAL_PERMITIDO la barrera completa no aplica: representar el ámbito efectivo como NO_APLICA y deshabilitar su selector. Esta normalización técnica evita combinar parcial con una barrera completa. El plan y el lote conservan modo/ámbito efectivos; cambiar el perfil después no cambia la operación confirmada. No volver a solicitar aprobación de esta decisión.

En modo completo, todos los archivos previstos de la unidad exigida deben estar validados y recibidos antes de iniciar sus efectos documentales. Solo será COMPLETADA cuando todos terminen con almacenamiento e incorporación confirmados. Un fallo posterior conserva avances y deja la unidad incompleta/pendiente de recuperación; no implica rollback de expedientes o documentos. Nunca omitir archivos silenciosamente ni marcar completa una unidad con rechazados, cancelados o resultados inciertos. Excluir un archivo antes de confirmar modifica el inventario, invalida el plan y exige nueva revisión; después de confirmar no se reduce el total original para simular éxito.

## Secuencia y responsabilidades

Regla de lectura: prevalecen las decisiones explícitas posteriores del usuario. Esta guía consolida los contratos transversales; los prompts especializados detallan su implementación. Las propuestas históricas y el prototipo no pueden reintroducir un alcance sustituido. Una discrepancia nueva debe actualizar el diseño común y sus consumidores, no resolverse de manera diferente en cada entrega.

| Prompt | Depende de | Responsabilidad principal |
|---|---|---|
| 04 | Exploración y decisiones | Diseño físico, contratos y política de integridad. |
| 02 | Diseño 04 | Configuración, versiones y publicación de perfiles. |
| 05 | Contratos 04; servicios 02/06 para integración final | Interfaz y apertura desde el árbol. |
| 06 | 04, perfil 02 y base de persistencia de plan 07-A si aplica | Plan validado sin efectos documentales. |
| 07-A | 04 y esquema/contratos de perfil 02 | Fundaciones: SQL y repositorios de plan/lote; no depende del servicio 06 implementado. |
| 07-B | 07-A y plan 06 | Confirmación y recepción; reutiliza las fundaciones de 07-A. |
| 08 | 04/07 y operaciones legacy verificadas | Adaptador de expedientes. |
| 09 | 04/07/08 y operaciones legacy verificadas | Adaptador de almacenamiento/incorporación. |
| 10 | 06–09 | Coordinación, exclusión, política de integridad y recuperación. |
| 11 | 07/10 y ventana 05 | Seguimiento, reanudación, árbol, limpieza y guía operativa. |
| 12 | Entregas integradas | Verificación integral, regresión y capacidad. |
| 13 opcional | 10–12 y decisión de despliegue | Ejecutor desatendido durable. |

El número del archivo identifica la entrega, no un orden ciego. 05 puede avanzar visualmente contra contratos, pero no se declara integrado antes de tener servicios reales.

07-A y 07-B son dos fases del mismo prompt 07, no dos esquemas ni equipos de implementación duplicados. Orden base: 04 → 02 → 07-A → 06 → 07-B → 08/09 → 10 → integración 05/11 → 12. La interfaz 05 puede avanzar contra contratos desde 04. Esto evita que 06 espere tablas de 07 mientras 07 espera a 06.

El 04 define y mantiene el diseño común en `Diseno/`. La implementación del 02 entrega `Sql/01-perfil-importacion-mysql51.sql`; el 07 entrega `Sql/02-carga-carpetas-mysql51.sql`. Los prompts 08–11 no crean repositorios ni tablas operativas paralelos: consumen los contratos del 07. Si cambia un esquema ya entregado/aplicado, producir un script incremental numerado y documentar su dependencia; no reescribir la historia de despliegue. Estos archivos SQL se generan durante las entregas; esta guía no afirma que ya existan.

## Contratos de integración que no deben divergir

| Responsabilidad | Propietario | Consumidores |
|---|---|---|
| Perfil, versiones, reglas y resolvedor | 02 | 06 y validación de valores diferidos en 08/09 |
| Contexto de apertura validado y preflight | 06 | 05 y revalidación de confirmación 07 |
| SQL/repositorios de plan y operación | 07-A | 06–11 |
| Confirmación idempotente y recepción | 07-B | 05/10/11 |
| Creación y verificación documental | 08/09 | 10; no endpoints públicos independientes que eludan el coordinador |
| Avance, cancelación y reconciliación | 10 | 05/11 y futuro 13 |
| UI etapas 1–3 y estructura visual de resultados | 05 | 11 conecta resultados, reanudación y actualización del árbol |
| Consulta/CSV/mantenimiento | 11 | UI y operación; consultar no ejecuta reconciliación |

Reglas comunes a implementar y verificar en 04–12:

1. **Errores e integridad parcial.** Distinguir errores globales (autorización, destino, perfil, manifiesto incoherente) que bloquean el plan, de errores por elemento. En PARCIAL_PERMITIDO el servicio define unidades elegibles y devuelve `puedeConfirmar` y causas, conservando todas las incidencias. La UI no bloquea todo solo por existir un error individual. En COMPLETO_OBLIGATORIO aplica el ámbito acordado; no inferir uno durante implementación. Sin unidades elegibles no se ejecutan efectos.
2. **Valores diferidos.** El plan previo no puede contener IDs/consecutivos de expedientes todavía inexistentes. Registrar fuentes simbólicas tipadas para esos campos, validar la definición en preflight y materializar/revalidar el valor después de confirmar el expediente, antes de almacenar. Persistir valor efectivo y procedencia; no cambiar reglas del snapshot. Fechas generadas se fijan una vez en la fase definida, no se recalculan en cada reintento. La barrera previa del modo completo exige todos los contenidos recibidos/validados y todas las comprobaciones posibles antes de crear; los valores diferidos se muestran pendientes de materialización, nunca como valores ya verificados. Un fallo en esa materialización conserva el expediente creado y bloquea completar la unidad; modo completo no garantiza ausencia de efectos parciales ante fallos posteriores.
3. **Temporales y nombre documental.** Separar staging retenido para recuperación de copia de trabajo consumible por legacy. `AlmacenamientoDocumentoProduccionDocumental` toma `File.Name` y elimina archivos de trabajo; no entregarle el único original recibido ni asumir que `NombreDocumento` preservará el nombre original. El adaptador debe demostrar conservación de nombre/extensión y aislamiento de rutas, o declarar no apto ese punto de reutilización. No cambiar el método compartido para corregirlo.
4. **Sesión y concurrentes externos.** Un bloqueo de lote no coordina automáticamente otras sesiones, la carga individual, SII ni escritores del índice XML. Verificar bloqueos/transacciones internos y efectos antes de reutilizar. Restaurar Session en Finally no revierte archivos borrados ni efectos ya producidos. Si no hay aislamiento demostrable sin tocar SII, esa vía queda bloqueada; no inventar una garantía ni modificar escritores compartidos.
5. **Reconciliación y concurrencia.** Ante resultado incierto, fallo al registrar resultado o reclamación vencida, no iniciar otro efecto mientras no exista evidencia de terminación/no ejecución del anterior. Una revisión CAS de la tabla no impide por sí sola duplicaciones dentro de un método legacy que ya está corriendo. Corrección manual de estado requiere una operación autorizada, evidencia y auditoría; no un botón de marcar éxito sin verificación.
6. **Disponibilidad y versión.** La versión fijada del lote no se sustituye por la última publicada. La política sobre desactivación y pérdida de permisos debe ser explícita en 04: detener nuevos efectos afectados, preservar resultados y permitir consulta/reconciliación solo a identidades autorizadas. No hacer imposible investigar un efecto incierto por usar el mismo permiso para todas las acciones.
7. **HTTP y ámbito.** Aplicar autenticación/autorización por operación y recurso y protección contra solicitudes falsificadas también en confirmar, recibir multipart, ejecutar, cancelar y reconciliar. No basta con proteger la administración del perfil. Usuario/empresa efectivos proceden de autenticación y recursos autorizados; los IDs de solicitud son solo referencias verificables.
8. **Despliegue y no regresión.** Esquema nuevo ausente/incompatible debe deshabilitar o rechazar solo la nueva capacidad de manera controlada, sin impedir abrir Producción ni usar la carga individual. No cambiar límites globales, bibliotecas ni arranque por conveniencia. Incluir compilación de nuevos archivos VB y publicación de recursos según el proyecto real. No modificar pruebas/huellas legacy para legitimar cambios fuera de alcance.

El prototipo es una referencia visual anterior a la política de integridad detallada: su validador bloquea todos los errores y su temporizador procesa archivo a archivo. No certifica la barrera de recepción completa ni implementa modo parcial real. Esos comportamientos se sustituyen por los contratos anteriores; la aprobación visual no los convierte en reglas de producción.

# DOC-111 — Fases del cliente y actualización de rama

Fecha: 2026-10-30. HEAD base `b2e93b726f221868cf2b82d43200a8e568871117`, workspace con cambios previos. Tareas 12.11, 13.8 y 13.9 completadas localmente; 114/130. Sin commit ni despliegue.

## Cliente y consulta

El transporte existente admite AvanzarLote, SolicitarDetencion y ReconciliarRecurso como mutaciones, conservando timeout/error/respuesta no interpretable como incertidumbre sin reenvío automático.

ConsultarLote anuncia solicitudes acotadas según autoridad ya comprobada, señal, actividad e incertidumbre. ConsultarRecursosLote proyecta el nombre NOMBRE confirmado de la raíz y el indicador de valores fijados del archivo, sin publicar valores/proveedores/evidencia interna. Las páginas conservan su snapshot/revisión propios; no se presentan como una transacción de lectura global. Las acciones son solicitudes disponibles, no una concesión de ejecución: el coordinador revalida recurso/revisión, permisos y barreras antes de producir efectos.

La vista incorpora carpetas paginadas y acciones por ID: Crear expediente, Preparar índices, Almacenar archivo y Verificar intento. Solo ofrece avance con facultad servidor, sin reclamación y con estado/recepción/padre compatibles. Una raíz confirmada en otra página debe consultarse para ofrecer las acciones de sus archivos; no se adivina su estado. La revisión de avance es la del recurso; detención/reconciliación usan la del lote. Cada clic ejecuta una fase, luego consulta; no encadena otra ni abre un worker.

Un error de mutación bloquea nuevas acciones hasta consulta explícita, sin volver a generar/envíar el mismo efecto. La lectura completa de las páginas requeridas despeja ese bloqueo local; una incertidumbre durable sigue impidiendo avance desde las acciones servidor. Un fallo de lectura de raíces descarta la página anterior y no despeja incertidumbre a partir de una lectura parcial. Solo los recursos INCIERTO con referencia de intento pueden solicitar verificación; la petición no fuerza éxito. Los estados finales/IDs se toman del servidor.

La consulta mantiene el nivel fijado del lote aunque cambie la selección del árbol. Cerrar conserva las referencias del lote, libera recursos y no promete cancelar una llamada del servidor ni acceder otra vez al disco del usuario.

## Rama de Producción

`Doc111ActualizarRama` en el script existente reutiliza `create_trenode_treview` sin modificar esa función. Identifica el padre por su ID de nivel y los nodos por `relacionNivelId|nivelRef|expedienteId|`, formato observado en `Class_niveles_organizacion.Lista_niveles_de_organizacion`. No compara nombres para decidir identidad.

Solo agrega raíces EXPEDIENTE_CONFIRMADO/COMPLETADA/PARCIAL sin intento activo y con IDs/nombre confirmado. Usa NOMBRE del plan confirmado, que puede diferir de la carpeta original. El helper existente asigna texto como texto del enlace; no inserta HTML del nombre. Comprueba duplicados por la relación completa antes de llamar al helper, por lo que consultas/repeticiones no duplican nodos y raíces homónimas con identidades diferentes permanecen distintas.

El origen debe corresponder al espacio opaco de recuperación capturado al abrir. El padre debe ser único, visible y tener su rama cargada. Si falta, está oculto, cambió el contexto o no existe el árbol, se omite la actualización sin invalidar los resultados persistidos. Una rama contraída cargada conserva su display; no se expande ni reconstruye el árbol, ni se cambia la selección actual, campos de selección o nodos ajenos. Los documentos se consultan mediante el recorrido existente al seleccionar el expediente; no se fabrican nodos documentales en el árbol de niveles/expedientes.

## Verificación

| Comprobación | Resultado y alcance |
|---|---|
| Cliente Chromium y unitarias | PASS, 13 pruebas en `tests/doc111-importacion-carpetas-client.test.cjs`. Servicios/HTTP sintéticos, navegador local. |
| Recorrido de fases | PASS: creación → índices → almacenamiento con respuesta perdida → consulta manual → reconciliación. Un solo envío ALMACENAR; detención impide nuevas fases. |
| Rama original | PASS con la función real `create_trenode_treview` y DOM Chromium: selección ajena conservada, rama contraída, sin duplicación por repetición, dos expedientes homónimos, nombre tratado como texto, rama oculta/ausente y contexto cambiado omitidos. Inicialización general y autocomplete usan dobles locales. |
| Vista móvil | PASS, 390 px sin desbordamiento en el recorrido nuevo; foco/cierre/recarga conservan las regresiones existentes. |
| Framework DOC-110/DOC-111 | PASS: `node --test tests/doc110-perfil-importacion-behavior.test.cjs`; nuevas proyecciones/nombre NOMBRE, capacidades restrictivas y casos previos. Sin BD/HTTP real. |
| Regresión local | PASS, otras 21 pruebas de contratos/carga individual y composición SII, revisión local; no acredita ejecución de carga individual/SII reales. |
| Registro productivo (13.8) | PASS, 87 archivos VB/JS/CSS/ASMX/ASHX de la familia de importación comprobados contra Compile/Content del vbproj. Sin fixtures DOC-111 ni tests compilados. Otros Content de pruebas preexistentes permanecen ajenos a este cambio. |
| Compilación (13.9) | PASS, MSBuild Debug completo con salidas/intermedios `.tmp/doc111-cliente-rama-build/`, log `.tmp/doc111-cliente-rama-build.log`. Sin despliegue o sustitución de DLL del sitio. |
| Sintaxis y coherencia | PASS, `node --check` de los cinco scripts de inventario/cliente/vista/transporte/Producción afectados o consumidos; OpenSpec estricto y `git diff --check`. 13.7 sigue pendiente del selector E2E todavía no implementado. |

Regresiones ejecutadas junto al cliente:

`node --test tests/doc111-importacion-carpetas-client.test.cjs tests/doc88-produccion-upload-contract.test.cjs tests/doc88-produccion-carga-almacenamiento.test.cjs tests/importar-servicio-web-production-composition.test.cjs tools/e2e/tests/doc88-production-document-upload-policy.test.cjs`

Resultado: 34/34 PASS. Los archivos de almacenamiento/creación legacy protegidos y consumidores SII/ImportarServicioWeb no tienen diferencias. La exclusión adicional del XML frente a otras cargas permanece descartada por el usuario.

IIS/HTTP autenticado, E2E del sistema, MySQL/rollback/carreras reales, XML, creación/almacenamiento/incorporación reales y limpieza: **NO_EJECUTADO**. Persisten pendientes los drivers/selector REC, aceptación autorizada y limpieza sujeta a política. Esta evidencia no hereda ni concede autorización de ambiente/cuentas/efectos reales.

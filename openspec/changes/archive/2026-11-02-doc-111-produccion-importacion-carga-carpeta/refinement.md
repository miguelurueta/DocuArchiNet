<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento — DOC-111

## Fuente y alcance

Ticket DOC-111, PRODUCCION-IMPORTACION-CARGA-CARPETA. Base inspeccionada: main 6313f91f, con DOC-110 fusionado. Stack observado: WebForms/VB.NET Framework 4.6.1, JavaScript, ASMX/ASHX y MySQL 5.1/InnoDB.

El ticket integra las entregas 04–12 de la guía 00, con sus propietarios y un único conjunto de contratos/repositorios. 13 (ejecución desatendida) queda fuera salvo activación explícita. Se conserva la instalación de empresa única confirmada por el usuario; no exige otra empresa para aceptación real.

## Contexto inspeccionado

DOC-110 implementa perfil/reglas/proveedores, pero ServicioPerfilImportacion.Seleccionar admite solo ADMINISTRACION. Su contexto, guard, repositorio y consulta de proveedor exigen PuedeAdministrar. Importar necesita autorización operativa propia, sin fingir esa facultad ni llamar a endpoints administrativos como puente.

ActivaCargaArchivos conserva la carga individual. El destino nuevo procede del árbol existente y queda fijo. Registrar_Expediente_Conservacion recibe id_nivel_padre e id_registro_relacion, pero no acredita correlación tras crash. AlmacenamientoDocumentoProduccionDocumental usa File.Name y elimina archivos de trabajo: requiere staging retenido, copia consumible y caracterización de sesión/efectos. No se reutilizan tareas Workflow ficticias ni se cambia SII.

Se leyeron la guía, el modelo/contratos/estados y decisiones comunes, la exploración y base gráfica, y se contrastaron los símbolos anteriores en el código actual. El inventario exacto y los propietarios se concretan en Diseno/design.md; Diseno/15-integracion-carga-doc111.md precisa la integración sin duplicar los contratos existentes.

## Decisiones para aprobación

| ID | Decisión verificable | Evidencia | Design | Requirement | Tasks |
|---|---|---|---|---|---|
| D-01 | Apertura desde nivel existente, destino fijo y ruta servidor de lectura. | ActivaCargaArchivos y PG_SELECCION_TREVIEEW_PRODUCCION; guía 00. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | IMPORTAR_CARPETAS y permisos documentales por acción; selección operativa y CSRF sin elevar administración. | ServicioPerfilImportacion.Seleccionar, ContextAdapter y RequestGuard. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Identidad propia de selección/raíz/archivo, rutas no confiables y manifiesto completo acotado. | Prototipo y FileUploadHandler.js por nombre; contrato 02. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Plan servidor con versión/snapshot, valores diferidos y elegibilidad según política; sin efectos documentales. | Resolvedor de DOC-110 y contratos 05/06. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Repositorio operativo único, seis entidades propias, confirmación transaccional/idempotente y SQL manual. | Modelo común 01 y repositorio del perfil. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Recepción privada con streaming, bytes/contenido/hash comprobados, separada de éxito documental. | Handler individual y almacenamiento que consume temporales. | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Adaptadores internos con destino, efectos y correlación comprobables. El usuario descartó el 2026-10-30 exigir aislamiento adicional del XML frente a otras cargas; la función de almacenamiento existente permanece intacta. | Registrar_Expediente_Conservacion y AlmacenamientoDocumentoProduccionDocumental. | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Una fase acotada por petición y barrera EXPEDIENTE/LOTE o parcial según snapshot. | Guía 00 y estados 03; integridad ya aprobada. | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | Timeout/lease vencida no habilitan repetir un efecto incierto; detener no borra resultados. | Ventanas entre persistencia legacy y registro del lote; estados 03. | D-09 | RQ-09 | Origen: D-09, RQ-09 |
| D-10 | Consultar/CSV no ejecutan; mantenimiento con política y sin borrar incertidumbre. | Contratos comunes 02/03. | D-10 | RQ-10 | Origen: D-10, RQ-10 |
| D-11 | Ventana por etapas, progreso real, recuperación autorizada y árbol por IDs confirmados. | Prototipo, Producción y UpdatePanel existentes. | D-11 | RQ-11 | Origen: D-11, RQ-11 |
| D-12 | Pruebas por capacidad y REC integrados, autorización real específica y evidencia diferenciada. | tools/e2e, DOC-88, DOC-110 y runbook. | D-12 | RQ-12 | Origen: D-12, RQ-12 |

## Requisitos verificables

| ID | Resultado | Criterio | Compatibilidad |
|---|---|---|---|
| RQ-01 | Destino explícito estable. | Árbol/pestaña no redirigen plan; nivel cambiado bloquea. | Sin otro árbol ni niveles nuevos. |
| RQ-02 | Importar no exige administrar. | Solo importador lee perfil compatible; solo administrador no ejecuta. | Default 0 y autorización vigente. |
| RQ-03 | Inventario íntegro con homónimos diferenciados. | Colisiones/rutas peligrosas se rechazan; cambiar inventario invalida plan. | Sin igualdad inferida por nombre/fecha/tamaño. |
| RQ-04 | Servidor decide valores y puedeConfirmar. | Fuentes diferidas no fabrican ID; errores globales y de elemento se distinguen. | No crea expedientes en preflight. |
| RQ-05 | Un lote por plan y confirmación. | Repetir devuelve mismo lote; contenido distinto da conflicto. | No transacción documental global. |
| RQ-06 | Recepción comprobada. | Reenvío compatible idempotente; bytes/formato distintos rechazados. | Sin rutas físicas cliente. |
| RQ-07 | Efectos y relaciones comprobados. | ID sin nivel/gabinete/vínculo no es éxito. | Adaptador sin correlación deja INCIERTO. |
| RQ-08 | Integridad de publicación preservada. | Probar completo/EXPEDIENTE, completo/LOTE y parcial. | Totales originales y efectos previos se conservan. |
| RQ-09 | Recuperación no duplica. | Dos clientes no reclaman igual efecto; cancelación conserva resultados. | Sin reintento por timeout. |
| RQ-10 | Seguimiento de lectura y mantenimiento autorizado. | Consulta/CSV no producen efectos; limpieza en seco sin política. | Incertidumbre conserva evidencia/staging. |
| RQ-11 | UI refleja estado durable. | Reabrir recupera lote y pide bytes faltantes; árbol sin duplicados. | Prototipo no es integración real. |
| RQ-12 | Evidencia real separada de dobles. | REC-01/02, SELECT, compilación y regresión por fase. | Nueva autorización para efectos DOC-111. |

## Condiciones operativas

Cuotas, máximos de entradas/bytes, duración de plan y retención se validan como política servidor; sin valores válidos se rechaza la capacidad correspondiente. No se hereda el límite ilustrativo de 500 ni se amplían límites globales IIS. La primera versión usa selección repetible de una raíz, manifiesto completo acotado y peticiones sucesivas; arrastre, fragmentación y host desatendido no son promesas implícitas.

Los formatos se anuncian después de caracterización, incluido Multi-TIFF si se habilita. El mapeo físico legacy se verifica antes de DDL/adaptadores. Sin correlación inequívoca un efecto queda INCIERTO, sin repetición automática. Persistencia y contratos pueden avanzar con dobles explícitos sin declarar terminada la carga.

## Resultado del refinamiento

Aprobación para continuar el flujo registrada el 2026-10-27 a partir de la indicación del usuario «continua con el flujo», posterior al ajuste del plan atómico y a su instrucción de priorizar el repositorio local. Se autoriza continuar la implementación del diseño y las tareas revisadas desde el directorio principal. Esta aprobación no autoriza SQL aplicado, autenticación E2E, carga o efectos documentales reales: esas actuaciones conservan su autorización específica y las condiciones de caracterización. Las autorizaciones de pruebas DOC-110 no se extienden automáticamente a efectos DOC-111.

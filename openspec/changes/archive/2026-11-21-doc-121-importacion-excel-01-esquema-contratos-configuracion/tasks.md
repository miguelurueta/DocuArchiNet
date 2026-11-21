<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## Alcance y dependencias

DOC-121, fase 01 contractual y SQL revisable. Tareas de las fases 02–07 no se implementan en este ticket. Instalar SQL es una operación manual posterior del usuario, separada de la terminación de estos entregables. Perfil legacy-webforms-vb. Complejidad relativa S/M; cada tarea tiene una salida verificable.

## 1. Refinamiento e inventario

- [x] 1.1 [M] Registrar catálogos, consumidores y metadatos históricos contrastados con código. Área/archivos: `Doc/Actualizacion/Importacion/Diseno/01-contratos-configuracion-doc121.md`. Origen: D-01, RQ-01. Verificación: inventario diferencia código actual, antecedente del 11 de noviembre y esquema aún no consultado.
- [x] 1.2 [M] Concretar decisiones y escenarios del alcance elegido por el usuario. Área/archivos: `refinement.md`, `design.md`, `specs/importacion-excel-01-esquema-contratos-configuracion/spec.md`. Origen: D-08, RQ-08. Verificación: ocho D/RQ trazables sin decisiones funcionales de fases posteriores ejecutadas.

## 2. SQL para ejecución manual

- [x] 2.1 [S] Entregar preflight de metadatos exclusivamente SELECT. Área/archivos: `Doc/Actualizacion/Importacion/Sql/00-preflight-doc121.sql`. Origen: D-02, RQ-02. Verificación: ninguna sentencia modificadora y separación lógica de RA/DA.
- [x] 2.2 [S] Entregar alta del permiso exclusivo inicialmente deshabilitado. Área/archivos: `Doc/Actualizacion/Importacion/Sql/01-permiso-carga-ra-doc121.sql`. Origen: D-04, RQ-04. Verificación: una columna DEFAULT 0, sin actualizar usuarios.
- [x] 2.3 [S] Entregar extensión aditiva de la cabecera histórica. Área/archivos: `Doc/Actualizacion/Importacion/Sql/02-plantilla-da-doc121.sql`. Origen: D-01, RQ-01. Verificación: solo dos columnas al final, sin cambios a datos/mapeos/orden previo.
- [x] 2.4 [M] Entregar la tabla operativa de ejecución. Área/archivos: `Doc/Actualizacion/Importacion/Sql/03-ejecucion-da-doc121.sql`. Origen: D-03, RQ-03. Verificación: una tabla InnoDB con identidad, snapshot, hashes, estados y preparación inicialmente nullable.
- [x] 2.5 [M] Entregar procedimiento de adecuación específica de clave. Área/archivos: `Doc/Actualizacion/Importacion/Sql/04-adecuacion-clave-doc121.sql`. Origen: D-05, RQ-05. Verificación: propuesta derivada del tipo/collation actual, índice completo y controles de datos/aislamiento antes del ALTER manual.
- [x] 2.6 [S] Entregar plantilla de concesiones de cuenta dedicada. Área/archivos: `Doc/Actualizacion/Importacion/Sql/05-concesiones-da-doc121.sql`. Origen: D-04, RQ-04. Verificación: solo privilegios acordados sobre DA, sin credenciales o concesión global.
- [x] 2.7 [S] Entregar comprobaciones posteriores exclusivamente SELECT. Área/archivos: `Doc/Actualizacion/Importacion/Sql/06-verificacion-doc121.sql`. Origen: D-02, RQ-02. Verificación: estructura, índices y permisos observables sin consultar credenciales históricas.

## 3. Contratos de las fases siguientes

- [x] 3.1 [M] Definir autoridad, configuración y límites efectivos. Área/archivos: `Doc/Actualizacion/Importacion/Diseno/01-contratos-configuracion-doc121.md`. Origen: D-04, RQ-04. Verificación: permiso propio, vínculo de servidor y 10000 filas/10000000 bytes independientes.
- [x] 3.2 [M] Definir operaciones y campos de request/response. Área/archivos: `Doc/Actualizacion/Importacion/Diseno/01-contratos-configuracion-doc121.md`. Origen: D-06, RQ-06. Verificación: recepción multipart y validación completa con muestra acotada; referencias y errores cerrados.
- [x] 3.3 [M] Definir transacciones y recuperación de confirmación/CREATE. Área/archivos: `Doc/Actualizacion/Importacion/Diseno/01-contratos-configuracion-doc121.md`. Origen: D-07, RQ-07. Verificación: mismo ID no duplica; incertidumbre no fuerza retry; sin pausa por fila.

## 4. Entrega y verificación

- [x] 4.1 [S] Entregar guía de aplicación manual y estado de instalación. Área/archivos: `Doc/Actualizacion/Importacion/Sql/README.md`. Origen: D-02, RQ-02. Verificación: orden, destino, precondiciones y PENDIENTE_EJECUCION_MANUAL explícitos.
- [x] 4.2 [M] Revisar estáticamente SQL y coherencia de contratos. Área/archivos: `Doc/Actualizacion/Importacion/Validacion/doc121-revision-fase01-20261120.md`. Origen: D-08, RQ-08. Verificación: auditoría reproducible de archivos; no se ejecuta SQL ni se acredita motor real.
- [x] 4.3 [S] Completar documentación de gobierno de la fase contractual. Área/archivos: `Doc/Tecnica/Opsxj/doc-121-importacion-excel-01-esquema-contratos-configuracion/`. Origen: D-08, RQ-08. Verificación: cinco documentos explican alcance, evidencia y E2E no aplicable sin plantillas vacías.
- [x] 4.4 [S] Verificar OpenSpec y trazabilidad antes de publicar la entrega. Área/archivos: artefactos DOC-121 y evidencia local OPSXJ. Origen: D-08, RQ-08. Verificación: validación estricta y auditoría de refinement aprobadas, evidencia vinculada al SHA comprometido.

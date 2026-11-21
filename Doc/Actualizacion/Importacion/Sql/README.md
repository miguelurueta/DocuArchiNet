# DOC-121 — SQL de instalación manual de importación Excel

Estado administrativo: [OpenSpec archivado y Jira Listo](../Validacion/doc121-archivo-cierre-20261121.md). PR133 integrado mediante 564a5600, CI 37/37 y gobierno 72/72. La carpeta principal quedó en main y se verificó el remoto antes del cierre. No se reaplica ningún SQL por archivar el cambio.

**Alcance de la fase 01:** contratos y scripts entregados, 16 tareas completas. El usuario aclaró que EGRESOS es solo de pruebas; no es necesario modificar esa tabla para entregar el módulo ni condiciona esta fase. Su condición de clave se revisaría únicamente si se elige como destino de carga. La plantilla 05 ya está entregada; cuenta/host/DA se configuran y verifican en la fase de conexión. Esto no certifica todavía la aplicación completa de sus concesiones.

[Verificación de entrega de DOC-121](../Validacion/doc121-verificacion-entrega-20261121.md): 8 requisitos y 24 escenarios revisados como contratos de fase 01, sin presentar las pruebas futuras de aplicación como ejecutadas.

**Estado actual de CONTRATOS, 2026-11-21: 08 APLICADO Y VERIFICADO.** El usuario autorizó después «aplica por mi». Se ejecutó un único ALTER en la base local inspeccionada: NCONTRATO NOT NULL y UNIQUE completo, conservando tipo/collation y las 625 filas con la misma huella de datos. [Autorización y resultado](../Validacion/doc121-contratos-indice-aplicado-20261121.md). No repetir 08 en esa base. Los demás scripts mantienen la política manual, salvo otra instrucción expresa.

**Corte anterior de preparación de claves:** 08 se entregó sin aplicar; [evidencia histórica y límites](../Validacion/doc121-preparacion-indices-20261121.md). Las incidencias observadas de EGRESOS impedirían usarla con esa clave única si se selecciona como destino; no bloquean la entrega contractual y no se corrigen por esta operación. La configuración efectiva del paso 05 sigue sin verificarse. 07/09 permiten comprobaciones SELECT, sin ejecutar otro ALTER.

**Corte 2026-11-21: el usuario informó archivos aplicados.** Se verificó la estructura 01–03 mediante 8 SELECT en la base física local previamente inspeccionada: 37 comprobaciones aprobadas. Falta precisar destinos RA/DA, adecuación de índices de 04 y cuenta/concesiones de 05. [Resultado y alcance](../Validacion/doc121-verificacion-instalacion-20261121.md). El agente no ejecutó DDL ni DML; no repetir los pasos compatibles ya instalados.

La entrega inicial del 20 de noviembre estaba en PENDIENTE_EJECUCION_MANUAL; ese estado se conserva en su evidencia histórica. Esta fase no agrega endpoints ni acredita cargas o permisos funcionales de cuentas. [Contrato y configuración](../Diseno/01-contratos-configuracion-doc121.md).

RA es la autoridad de usuario/empresa/permisos. DA contiene plantillas, ejecuciones y tablas de datos. El operador selecciona la conexión y base correctas; que una instalación histórica use el mismo esquema físico no permite asumir que siempre coincidan. Conservar respaldo y resultados saneados conforme al procedimiento del ambiente.

| Orden | Archivo | Destino / precondición | Efecto previsto |
|---|---|---|---|
| 00 | [Preflight](00-preflight-doc121.sql) | Ejecutar en RA y DA con cuenta que pueda ver sus metadatos. | Solo SELECT: versión, motor, columnas/orden, índices, FK, triggers y privilegios visibles. |
| 01 | [Permiso](01-permiso-carga-ra-doc121.sql) | RA correcta; columna ausente. | ADD ADMINISTRAR_CARGAS_IMPORTACION DEFAULT 0. No habilita usuarios. |
| 02 | [Cabecera de plantilla](02-plantilla-da-doc121.sql) | DA correcta; ambas columnas ausentes. | ADD CAMPO_UNICO nullable y REVISION_CARGA DEFAULT 1 al final. No cambia filas/mapeos. |
| 03 | [Registro de ejecución](03-ejecucion-da-doc121.sql) | DA transaccional del destino; tabla ausente, InnoDB disponible. | Una tabla nueva con 22 columnas, PK y tres índices secundarios. |
| 04 | [Adecuación por tabla](04-adecuacion-clave-doc121.sql) | DA, elección de clave e aislamiento revisados. | Solo SELECT que genera controles y propuesta específica de ALTER; todavía no aplica índice. |
| 05 | [Concesiones](05-concesiones-da-doc121.sql) | DBA, cuenta ya provisionada fuera de Git y destino DA/host confirmados. | Plantilla comentada; completar copia administrativa local antes de ejecutar GRANT. |
| 06 | [Verificación posterior](06-verificacion-doc121.sql) | Después de aplicar y confirmar 01–03 y los específicos pertinentes. | Solo SELECT: contrastar las definiciones esperadas con el resultado real. |
| 07 | [Preflight de claves específicas](07-preflight-claves-doc121.sql) | DA real de las fuentes; consulta de catálogo en RA si es distinta. | Solo SELECT: atributos, índices y conteos actuales; no limpia datos. |
| 08 | [Índice de CONTRATOS](08-indice-contratos-ncontrato-doc121.sql) | Aplicado por el agente por autorización expresa posterior; no repetir en la base inspeccionada. | NOT NULL y UNIQUE completo verificados; 625 filas preservadas. No modifica EGRESOS. |
| 09 | [Verificación de claves](09-verificacion-claves-doc121.sql) | Después de confirmar la aplicación manual del ALTER pertinente. | Solo SELECT: nulabilidad/atributos, índice completo y conteo de CONTRATOS. |

00, 06, 07 y 09 contienen únicamente SELECT; no aplican DDL/DML o ejecución dinámica. No consultan valores de imp01_xmlconexion. Los metadatos de columnas de esa tabla no revelan las credenciales almacenadas en sus filas.

## Antes de ejecutar

Confirmar por separado RA y DA del ambiente. Comparar el preflight con el contrato y los antecedentes; el inventario del 11 de noviembre no reemplaza esta lectura. Una ampliación existente como BENEFICIARIO debe conservarse. No cambiar motores, longitudes o codificaciones para hacer coincidir el antecedente.

Los DDL 01–03 son de una sola aplicación y fallan si ya existe el objeto. Si el paso está instalado con definición compatible, omitirlo después de verificarlo. Si 02 está parcialmente aplicado, no ejecutarlo de nuevo: entregar otro archivo con la diferencia. No usar ejecución del cliente con continuar-ante-error para aplicar todo como un lote ciego. Cada paso exige revisar su resultado antes del siguiente. DDL no tiene la garantía de rollback del reemplazo DML.

## Procedimiento del índice específico

04 conserva tipos/defaults/charset/collation/comentarios de las claves históricas CONSECUTIVO y NCONTRATO. No modifica BENEFICIARIO ni ejecuta el SQL que devuelve. El nombre del archivo refleja que es un generador, no una adecuación ya aplicada.

1. Confirmar en fuentes/proveedores la clave real y si la tabla es exclusiva por CONEXION. Si usa columna empresarial, crear otro archivo con ambos componentes requeridos y UNIQUE(empresa,clave); no aplicar la propuesta de clave simple.
2. Ejecutar manualmente los controles SELECT devueltos; exigir cero nulos/vacíos y grupos duplicados. Revisar índices completos existentes de 00, SUB_PART NULL y tipo/collation efectivos.
3. Revisar capacidad de la clave en bytes para el motor real y atributos que deben preservarse. Si hay un UNIQUE equivalente, omitir su ADD y adecuar solo nulabilidad cuando corresponda. Si hay incidencias, no limpiar/truncar ni elegir otra collation automáticamente.
4. Guardar la propuesta revisada como nuevo archivo SQL específico con tabla, aislamiento, precondiciones y SELECT de comprobación. Para conservarla en el repositorio, entregar el resultado saneado y generar ese archivo antes de ejecutarlo. Su creación no autoriza aplicar SQL desde el agente.
5. El usuario aplica ese archivo en su ventana acordada y confirma el resultado. Repetir SELECT para demostrar NOT NULL e índice completo y registrar su huella.

Las demás tablas requieren su propio archivo específico. Las tablas nuevas del futuro módulo incluirán la clave al crear; esta fase no crea tablas de negocio ni inserta datos de ejemplo.

## Cuenta y permisos

El DBA completa una copia local de 05 con nombres de ambiente, conserva el secreto fuera de Git y ejecuta la concesión manualmente. Sin host comodín, privilegios globales, DROP o GRANT OPTION. La concesión DA abarca sus tablas actuales y futuras: el filtro imp_02_ lo aplica el servidor y no es aislamiento de privilegios MySQL por prefijo.

La visibilidad de information_schema.SCHEMA_PRIVILEGES depende de la cuenta consultante; ausencia de filas no demuestra ausencia de concesiones de otra cuenta. Registrar lo observable y obtener confirmación administrativa de la cuenta dedicada sin imprimir credenciales. El permiso funcional de usuarios permanece 0; su habilitación requiere gestión autorizada posterior.

## Confirmación y revisión

Registrar por archivo: ambiente, destino lógico, fecha, aplicado/omitido/error y resultado saneado. Después de la confirmación del usuario el agente revisa exclusivamente SELECT; una diferencia que requiera cambios produce otro archivo para ejecución manual. Distinguir ENTREGADO, EJECUTADO_CONFIRMADO y VERIFICADO_SELECT, sin sustituir un estado por otro.

No ejecutar SQL de instalación desde tests, arranque o rutas indirectas de la aplicación. Las fases dependientes del nuevo esquema esperan confirmación y comprobación posterior antes de sus recorridos. No retirar índices, columnas o historial con un rollback automático; conservar objetos aditivos y generar un nuevo plan revisado si se necesita revertir estructura.

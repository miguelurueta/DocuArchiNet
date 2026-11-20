# DOC-121 — SQL de instalación manual de importación Excel

**Estado: PENDIENTE_EJECUCION_MANUAL. Ningún archivo de este paquete fue ejecutado por el agente.** Entrega de fase 01, sin endpoints, carga de datos o permisos habilitados. [Contrato y configuración](../Diseno/01-contratos-configuracion-doc121.md).

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

00 y 06 no contienen USE, SET, DDL, DML ni ejecución dinámica. No consultan valores de imp01_xmlconexion. Los metadatos de columnas de esa tabla no revelan las credenciales almacenadas en sus filas.

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

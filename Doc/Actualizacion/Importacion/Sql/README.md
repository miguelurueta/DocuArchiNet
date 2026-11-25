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

Revisión DOC122 solicitada por el usuario: el importador usa la cuenta DA del módulo mediante el resolver compartido. No exige otra cuenta ni otra conexión en Web.config. SQL05 permanece como antecedente de la propuesta inicial; no aplicarlo automáticamente sobre la cuenta compartida. Los SELECT de10 pueden revisar la cuenta efectiva del módulo, aunque su comentario histórico menciona una cuenta dedicada. No se modifican los scripts ya entregados.


DOC-122 añade 10-verificacion-doc122.sql, exclusivamente SELECT. Ejecutar sus bloques RA/DA en la conexión indicada y revisar metadatos de los catálogos y privilegios observables de la cuenta efectiva del módulo. La fase02 no introduce DDL nuevo: conserva los scripts DOC121. El archivo10 está ENTREGADO; no constituye una comprobación real de la cuenta ni habilita permisos.

El DBA completa una copia local de 05 con nombres de ambiente, conserva el secreto fuera de Git y ejecuta la concesión manualmente. Sin host comodín, privilegios globales, DROP o GRANT OPTION. La concesión DA abarca sus tablas actuales y futuras: el filtro imp_02_ lo aplica el servidor y no es aislamiento de privilegios MySQL por prefijo.

La visibilidad de information_schema.SCHEMA_PRIVILEGES depende de la cuenta consultante; ausencia de filas no demuestra ausencia de concesiones de otra cuenta. Registrar lo observable y obtener confirmación administrativa de la cuenta dedicada sin imprimir credenciales. El permiso funcional de usuarios permanece 0; su habilitación requiere gestión autorizada posterior.

## Confirmación y revisión

[13-plantilla-ordenes-2025-doc124.sql](13-plantilla-ordenes-2025-doc124.sql) documenta
la plantilla27 creada a solicitud expresa del usuario por el repositorio del proyecto:
conexión6, Hoja1, clave NROORDEN y siete campos. El alta ya está aplicada y verificada
por SELECT; no repetirla. El archivo SQL permite revisión o aplicación manual en otro
ambiente compatible. No crea tabla física ni importa órdenes. El máximo20000 queda
configurado en Web.config; el original se conserva y se entrega una copia con fila1
de encabezados, porque la fila1 original es un título.

DOC-124 entrega además [12-preflight-recursos-e2e-doc124.sql](12-preflight-recursos-e2e-doc124.sql),
solo SELECT sobre las fuentes existentes EGRESOS/CRÉDITOS y sus plantillas/índices.
No crea tablas de prueba ni corrige/activa datos o configuración. El control completo
de filas durante la E2E reutiliza la huella DOC32, sin el límite10000 de su vía Rows;
esto no modifica el límite del Excel que se prepara (20000 configuradas actualmente;
10000 por defecto en el lector). Los destinos existentes
pueden usarse en lectura con huellas antes/después, sin declararlos descartables.

[14-preflight-contratos-e2e-doc124.sql](14-preflight-contratos-e2e-doc124.sql)
prepara la comprobación de CONTRATOS para los casos AGREGAR/REEMPLAZAR. Contiene
solo SELECT, separados entre RA y DA: política, columnas autorizadas, esquema,
índice, conteos y plantillas. Estado: ENTREGADO, PENDIENTE_EJECUCION_MANUAL.
No activa fuentes ni crea plantillas. La [investigación posterior solicitada por
el usuario](../Validacion/doc124-empresa-contratos-verificada-20261123.md) confirmó
la empresa2 mediante la decisión previa de empresa única y12 SELECT de la
configuración actual; el esquema sin columna empresarial por sí solo no demuestra
exclusividad. Después se aplicaron15/16 por autorización expresa: fuente5 y
plantilla28, como se detalla en el apartado siguiente. Los modos de la plantilla son
distintos: una modificación exige revisión/huella nuevas y gestión explícita,
fuera del recorrido Preparar. [Plan de la matriz pendiente](../Validacion/doc124-matriz-pendiente-20261123.md).

## Configuración manual de CONTRATOS para DOC124

**SQL15/16 APLICADOS Y VERIFICADOS2026-11-23**, por instrucción posterior expresa
del usuario «aplica los archivos por mi»: fuente5/revisión2 y plantilla28/revisión1,
AGREGAR. [Autorización y resultado](../Validacion/doc124-sql15-16-aplicados-20261123.md).
No repetir15/16 en este ambiente. SQL17 se aplicó después del PASS real de AGREGAR, al continuar el
usuario la ejecución delegada: plantilla28/revisión2/REEMPLAZAR, con datos/mapeos
y cargas previas intactos. [Resultado y ventana siguiente](../Validacion/doc124-e2e-reemplazar-ventana-20261123.md).
No repetir SQL17 en este ambiente. Esta excepción no autoriza instalaciones desde pruebas
ni concede permiso general para ejecutar otros SQL. No repetir el índice08.

| Orden | Archivo / destino | Efecto previsto |
|---|---|---|
| Primero | [15-fuente-contratos-doc124.sql](15-fuente-contratos-doc124.sql), RA | Fuente DA/imp_02_contratos de empresa2, aislamiento CONEXION, cuatro columnas autorizadas, ALTA/revisión1 y ACTIVAR/revisión2 con historial. No publica proveedor. |
| Después | [16-plantilla-contratos-agregar-doc124.sql](16-plantilla-contratos-agregar-doc124.sql), DA del módulo11 | Una plantilla activa de conexión6, Hoja1, clave NCONTRATO, cuatro campos VARCHAR(250) y modo AGREGAR/revisión1. No crea otra tabla. |
| Tras PASS real AGREGAR | [17-plantilla-contratos-reemplazar-doc124.sql](17-plantilla-contratos-reemplazar-doc124.sql), misma DA | Cambia esa misma plantilla a REEMPLAZAR/revisión2, conservando campos y archivo de referencia. No reemplaza registros. |

En15 completar `@doc124_actor` con el ID real del operador registrado en RA;
su valor inicial NULL impide escrituras. Confirmar que RA/DA corresponden a los
módulos12/11 de empresa2 y que referencia6 coincide con el destino ya verificado.
Las precondiciones de los scripts rechazan configuración existente o incompatible
en vez de sobrescribirla. Si ya existe fuente/plantilla, inspeccionar y preparar
una adecuación nueva; no forzar estas altas.
En17 completar `@doc124_plantilla_esperada` con el ID devuelto por16 y usado en
el PASS de AGREGAR; también empieza en NULL para impedir cambios sin identificarlo.

Ejecutar cada archivo por separado, en una sesión SQL persistente y sin continuar
ante errores. Los archivos abren una transacción y **no ejecutan COMMIT**:
solo si aparece `LISTO_PARA_COMMIT` y no hubo errores, ejecutar `COMMIT;` manualmente
en esa misma sesión. Cualquier error u otro resultado exige `ROLLBACK;` y revisión.
Cerrar la sesión sin COMMIT no acredita aplicación. Confirmar archivo, ID devuelto
y COMMIT; el agente verificará mediante SELECT antes de la E2E dependiente.

15 y16 ya están aplicados y verificados en este ambiente. La ventana `--excel-add`
sobre plantilla28/revisión1 ya aprobó3/3 casos y quedó cerrada; [resultado real](../Validacion/doc124-e2e-agregar-resultado-20261123.md).
SQL17 ya está aplicado y verificado, con plantilla28/revisión2 y huella nueva.
`--excel-replace` aprobó2/2 casos y quedó cerrada; [resultado real](../Validacion/doc124-e2e-reemplazar-resultado-20261123.md).
No repetir AGREGAR, REEMPLAZAR ni SQL17 para completar la documentación.
Cada ventana usa IDs nuevos, consentimiento explícito y secretos efímeros;
no guarda plantillas ni aplica SQL. Los PASS parciales se revisan juntos con el
CREAR de Órdenes; una fase aislada no completa2.6 ni permite archivar DOC124.

DOC-124 entrega [11-preflight-doc124.sql](11-preflight-doc124.sql), únicamente SELECT,
para ejecutar manualmente por bloques RA/DA. Comprueba metadatos de las 22 columnas,
índices completos, política empresarial y capacidades observables. **ENTREGADO,
PENDIENTE_EJECUCION_MANUAL**: no se ha aplicado ni consultado en esta implementación.
No se prevé migración permanente nueva; no reaplicar 01–10. Una diferencia detectada
requiere otro archivo específico, aplicación manual y confirmación antes de verificar.
Las temporales propias de Preparar pertenecen a la operación, no a la instalación;
la E2E necesita autorización independiente. Conserva los datos de EGRESOS/CONTRATOS.

Registrar por archivo: ambiente, destino lógico, fecha, aplicado/omitido/error y resultado saneado. Después de la confirmación del usuario el agente revisa exclusivamente SELECT; una diferencia que requiera cambios produce otro archivo para ejecución manual. Distinguir ENTREGADO, EJECUTADO_CONFIRMADO y VERIFICADO_SELECT, sin sustituir un estado por otro.

No ejecutar SQL de instalación desde tests, arranque o rutas indirectas de la aplicación. Las fases dependientes del nuevo esquema esperan confirmación y comprobación posterior antes de sus recorridos. No retirar índices, columnas o historial con un rollback automático; conservar objetos aditivos y generar un nuevo plan revisado si se necesita revertir estructura.

## DOC125: preflight de confirmación

[18-preflight-doc125.sql](18-preflight-doc125.sql) queda **ENTREGADO / PENDIENTE_EJECUCION_MANUAL**.
Contiene sólo SELECT y no ha sido ejecutado por el agente. Primero ejecutar el
bloque DA en la base del módulo: capacidades, InnoDB, 22 columnas operativas,
índices, privilegios observables, triggers y relaciones. Después seleccionar
manualmente RA y ejecutar su bloque de presencia de permiso/política. No cambia
datos, esquema, permisos ni sql_mode; no requiere COMMIT.

Comparar las columnas con SQL03 y conservar SQL15–17 ya aplicados. Ejecutar con
la cuenta que usará la operación: los permisos del administrador no acreditan
los del escritor. En MySQL5.1 la visibilidad de TRIGGERS depende del privilegio
TRIGGER; cero filas sin visibilidad suficiente no acredita ausencia. La inspección
de claves foráneas debe abarcar referencias entrantes desde otras tablas/bases.
Si no se pueden acreditar metadatos completos, la confirmación queda cerrada;
cualquier adecuación necesaria se entregará en otro archivo para aplicación manual.

Confirmar archivo, destinos y resultado saneado antes de su revisión SELECT.
Este preflight no habilita una E2E escritora. DOC124 sólo preparó las cargas y
conservó los 625 contratos; DOC125 requiere autorización específica de ambiente,
cuentas, destinos descartables y efectos. El CREATE operativo del modo CREAR se
implementará en Confirmar con procedencia recuperable; la instalación de sus
recursos, plantillas, fuentes o permisos conserva el flujo manual.

### Fixtures descartables para la E2E de DOC125

**Aplicados y verificados el 2026-11-23**, por delegación expresa del usuario
«ejecuta», fuera de los tests. SQL18: diez controles SELECT; SQL19: dos tablas
descartables; SQL20: fuentes6/7 activas revisión2; SQL21: plantillas29/30/31
revisión1, conexión6 y tres filas sintéticas. SQL20/21 se confirmaron únicamente
tras LISTO_PARA_COMMIT. Relecturas en sesiones nuevas y huellas de configuración
previa, CONTRATOS625, EGRESOS y CRÉDITOS intactas. No volver a aplicar estos archivos.
Evidencia en [doc125-instalacion-ejecucion-20261123.md](../Validacion/doc125-instalacion-ejecucion-20261123.md).
Los intentos reales detenidos y la corrección del barrido global se registran en
[doc125-e2e-parada-correccion-20261124.md](../Validacion/doc125-e2e-parada-correccion-20261124.md).
Se añadieron fixtures porque la aceptación DOC124 conservó todos los datos de sus
destinos; la autorización DOC125 sólo escribe en sus fixtures descartables.

| Orden | Archivo y destino | Efecto |
| --- | --- | --- |
| 1 | [18-preflight-doc125.sql](18-preflight-doc125.sql), DA/RA | Diagnóstico SELECT y revisión de visibilidad de metadatos. |
| 2 | [19-fixtures-da-doc125.sql](19-fixtures-da-doc125.sql), DA | Dos tablas **nuevas** `imp_02_doc125_e2e_agregar/reemplazar`, con CLAVE/VALOR y dos índices UNIQUE. El destino `..._crear` permanece ausente. |
| 3 | [20-fuentes-e2e-ra-doc125.sql](20-fuentes-e2e-ra-doc125.sql), RA | Dos fuentes exclusivas de la empresa real indicada, dos columnas por fuente e historial de alta/activación. Completar empresa y actor reales; no publica proveedores. |
| 4 | [21-plantillas-e2e-da-doc125.sql](21-plantillas-e2e-da-doc125.sql), DA | Tres plantillas nuevas, hoja Datos, dos campos y tres filas **sintéticas** iniciales en los fixtures. Completar conexión histórica real. |

En SQL19 revisar la ausencia de los tres nombres antes de iniciar el bloque DDL. Una tabla
preexistente exige detener y revisar; no limpiar ni reapropiarse del nombre.
El DDL tiene commit implícito: conservar y reportar cualquier estructura parcial.
SQL20/21 abren transacción y no ejecutan COMMIT. Ante cualquier error o resultado
distinto de `LISTO_PARA_COMMIT`, hacer ROLLBACK y detener. Sólo confirmar manualmente
en la misma sesión si ambos controles son correctos. No repetir archivos ya aplicados.

Después de la confirmación manual, las comprobaciones del agente son SELECT.
La cuenta del módulo debe acreditar visibilidad de TRIGGER y de referencias FK
entrantes. La implementación actual exige SELECT global para acreditar referencias
desde otras bases; si no se acredita devuelve `METADATOS_NO_ACREDITADOS`. Estos
archivos **no conceden ese privilegio**: revisar el diagnóstico y la política de la
cuenta antes de decidir cualquier adecuación, que tendría su propio archivo/alcance.

El segundo UNIQUE sobre VALOR permite preparar un archivo con CLAVE única y
rechazar su escritura por VALOR repetido después del DELETE; el test debe conservar
el contenido previo mediante rollback. El actor externo de la carrera escribe una
sola fila del fixture autorizado, sin lock cooperativo, y conserva ese efecto.
No se incluyen endpoints de inyección de fallos ni limpieza de resultados.

El runner sigue siendo `test:doc110:session -- --excel-confirm`, con scope DOC125 y
consentimiento específico de ambiente/cuentas/efectos, recibido con «ejecuta».
La preparación local de libros sintéticos reutiliza el generador del harness
DOC123. La DLL ya está desplegada y la matriz real pasó5/5; las sesiones terminaron y
las claves se eliminaron. Resultado y mediciones en
[doc125-ejecucion-seguimiento-20261124.md](../Validacion/doc125-ejecucion-seguimiento-20261124.md).
Los efectos quedan conservados en los fixtures autorizados. Los SQL conservan sus valores NULL de entrega; los IDs reales
se completaron sólo en la ejecución delegada, sin reeditar los archivos aplicados.

## DOC126 — interfaz y nueva aceptación, pendiente de aplicación manual

La interfaz utiliza el esquema existente. Para su aceptación se entregan fixtures
separados: los resultados DOC125 permanecen intactos y su tabla CREAR ya contiene
datos. No reaplicar18–21 ni reutilizar aquella autorización para estos archivos.

| Orden | Archivo y destino | Estado y efectos |
| --- | --- | --- |
| 1 | [22-preflight-doc126.sql](22-preflight-doc126.sql), bloques RA/DA | Sólo SELECT. Revisar instalación, referencias y dos cuentas de prueba. Ejecutar cada bloque en su base indicada. |
| 2 | [23-fixtures-da-doc126.sql](23-fixtures-da-doc126.sql), DA | PENDIENTE_EJECUCION_MANUAL: dos tablas nuevas imp_02_doc126_e2e_agregar/reemplazar; CREAR debe seguir ausente. DDL tiene commit implícito. |
| 3 | [24-fuentes-e2e-ra-doc126.sql](24-fuentes-e2e-ra-doc126.sql), RA | PENDIENTE_EJECUCION_MANUAL: dos fuentes CONEXION con historial, sin proveedores. Completar empresa/actor reales en copia local. |
| 4 | [25-plantillas-e2e-da-doc126.sql](25-plantillas-e2e-da-doc126.sql), DA | PENDIENTE_EJECUCION_MANUAL: tres plantillas nuevas y tres filas sintéticas iniciales. Completar conexión histórica del módulo en copia local. |

Revisar ausencia de los tres destinos antes del DDL; si existe alguno, detener.
No ejecutar con opción de continuar ante errores. SQL24/25 mantienen transacción
abierta: sólo LISTO_PARA_COMMIT y cero errores permiten COMMIT manual en la misma
sesión; otro resultado exige ROLLBACK y revisión. Conservar objetos parciales y
todos los efectos; no hay limpieza automática. Verificar mediante SELECT en una
sesión nueva después de la confirmación de aplicación del usuario.

La E2E DOC126 exige propietario con ADMINISTRAR_CARGAS_IMPORTACION=1 y los cuatro
permisos de carpetas/perfiles/fuentes/proveedores en0, más una cuenta restringida
activa con cargas=0. Tener permisos adicionales no acredita el caso exclusivo.
Estos archivos no crean usuarios, retiran permisos ni conceden privilegios. Si
el preflight identifica una adecuación, se entregará como otro archivo revisado
antes de la ejecución que dependa de ella.

Reutilizar test:doc110:session con --excel-ui-read o --excel-ui-write, el perfil
existente y recursos DA/RA del repositorio. El scope DOC126 incluye hashes de
DLL y tres assets, cuentas, archivo/destinos y efectos máximos. Los controles
SELECT usan los DSN existentes mediante DOC126_DA_E2E y DOC126_RA_E2E; sus datos
de acceso son efímeros y no modifican Web.config. La instalación se hace fuera
del test. Esta entrega de archivos no acredita aplicación, despliegue ni E2E real.

## Recorrido funcional con cuenta existente — 2026-11-24

57/62 tareas: completada la adaptación 6.12; pendientes 6.9, 6.10, 6.11, 7.5 y 8.8. Usuario 141 activo y autorizado con sus cinco permisos vigentes, conservados. Modo funcional de agregar/reemplazar separado de exclusividad/restricción; 88/88 pruebas locales y cuatro E2E registradas, sin ejecutar la nueva ventana real. Preflight SELECT: faltan tablas, políticas y plantillas DOC126. Copias SQL23–25 parametrizadas en .tmp/doc126-functional-preparacion, sin aplicar. Ver `Doc/Actualizacion/Importacion/Validacion/doc126-cuenta-existente-funcional-20261124.md`.

## SQL23–25 aplicados con autorización — 2026-11-24

El usuario autorizó aplicar las copias parametrizadas. Dos tablas, dos políticas y tres plantillas activas verificadas desde conexiones nuevas; IDs57/58/59, filas sintéticas iniciales1/2 y destino CREAR ausente. Las indicaciones PENDIENTE de cortes anteriores son históricas: no reaplicar. La E2E funcional usa la cuenta141 con permisos actuales conservados y plantillas58/59; scope vigente válido y27 tablas protegidas. Consola privada preparada; no se acredita ejecución hasta disponer de su resultado. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-aplicacion-sql-funcional-20261124.md`.

## E2E funcional detenida en autenticación — 2026-11-24

SQL23–25 aplicados y verificados; no reaplicar. Corregido control compartido para la columna histórica AÑO_FERIADO, con38/38 regresiones. Segundo intento completó27 huellas iniciales, pero el servidor devolvió LOGIN_CREDENTIALS_REJECTED en el módulo GESTOR del perfil local. Cero sesiones autenticadas/cargas nuevas; fixtures permanecen1/2 filas. Confirmar el acceso manual y proporcionar la contraseña sólo en consola privada. No se cierran tareas de aceptación por estos intentos. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-aplicacion-sql-funcional-20261124.md`.

## Aceptación funcional real con la cuenta existente — 2026-11-24

E2E Playwright real PASS: un test, dos casos. Agregar1000:1→1001 filas; reemplazar1000:2→1000 filas, con consentimiento. Usuario141 en IIS local HTTPS/GESTOR, plantillas58/59 y hashes/propiedad/locks verificados. Huellas íntegras de27 tablas protegidas y configuración de plantillas sin cambios. Perfil local separado; perfil compartido, permisos, DLL y Web.config conservados. La creación/importación manual del usuario se reconoce como realizada. No acredita exclusividad/restricción, rollback/carrera o asistente automatizado. Ver `Doc/Actualizacion/Importacion/Validacion/doc126-e2e-funcional-real-20261124.md`.

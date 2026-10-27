# Mapa de bases verificado por conexión

Inspección directa 2026-10-20, exclusivamente SELECT. Se leyeron conexiones de `Web.config` en memoria y el catálogo `gestor_modulos` mediante `OdbcServicesGestor`. Luego se abrieron las conexiones de los módulos activos relevantes con la biblioteca MySql.Data del repo, como hace el acceso modular de la aplicación. No se guardaron credenciales, endpoints de servidor ni cadenas de conexión.

Evidencia: `openspec/changes/doc-110-produccion-importacion-carpeta-perfil/evidencia-mapa-bases.json`. Contiene metadatos y estados por origen, no registros de documentos o usuarios.

## Conexiones consultadas

| Origen configurado | Base seleccionada real | Resultado |
|---|---|---|
| ODBC `OdbcServices` | `docuarchi` | Consulta correcta. |
| ODBC `OdbcServicesGestor` | `docuarchi` | Consulta correcta; catálogo que determina módulos. |
| Módulo 12, GESTOR DOCUMENTAL, origen RA | `docuarchi` | Consulta correcta. |
| Módulo 11, DOCUARCHI CONTENEDOR, origen DA | `docuarchi` | Consulta correcta. |
| Módulo 8, WORKFLOW DOCUMENTAL | `workflowtconta` | Consulta correcta. |
| Módulo 14, WORKFLOW DOCUMENTAL | `workflowdocument` | Consulta correcta. |
| Configuración `MyDbContext` | No acreditada | Apertura/inspección fallida; error registrado sin detalles sensibles. No se utiliza para afirmar ubicación de tablas. |

RA y DA reportaron el mismo servidor/esquema en esta inspección: comparación en memoria de hostname, puerto, server_id y DATABASE(), sin publicar esos identificadores de servidor. Esto confirma la coincidencia actual; no implica que la arquitectura deba asumirla en otros ambientes.

## Existencia confirmada en `docuarchi`

Actualización de nomenclatura posterior: el usuario solicitó prefijo `ra_` para las tres tablas nuevas. Los nombres vigentes son `ra_integracion_perfil`, `ra_integracion_perfil_version`, `ra_integracion_perfil_regla`; nueva consulta SELECT confirmó su ausencia (`evidencia-nombres-ra.json`). Las menciones sin prefijo más abajo describen la consulta histórica, no el DDL vigente.

Las once tablas siguientes existen en el esquema seleccionado por RA y DA:

| Grupo | Tablas |
|---|---|
| Empresa/módulos | `empresa_gestion_documental`, `gestor_modulos` |
| Usuario/permisos | `remit_dest_interno`, `remit_dest_interno_perfil_produccion` |
| Estructura/permisos de nivel | `registro_organigrama`, `ra_pro_niveles`, `ra_pro_permisos_niveles` |
| Expedientes/tipologías | `expediente_archivo`, `tipo_doc_series` |
| Gabinetes/índices | `system1`, `detalle_gabienete` |

No existen `integracion_perfil`, `integracion_perfil_version` ni `integracion_perfil_regla` en esa base. Tampoco aparecieron en los esquemas visibles consultados. No se crearon durante esta inspección.

En las bases Workflow activas, de la lista consultada solo aparece `gestor_modulos`; no se encontraron allí las otras diez tablas documentales ni las tres nuevas. No significa que Workflow carezca de sus propias tablas: la búsqueda estuvo acotada al conjunto anterior.

## Tablas homónimas visibles

`information_schema` también muestra tablas documentales homónimas en `docuarchi_congente`, `docuarchi_ini`, `docuarchi_primavera`, `docuarchi_ucartagena` y `docuarchi_vehillanos`. No se selecciona una de ellas por semejanza de nombre o porque tenga las tablas: manda el enrutamiento del módulo autorizado.

No se inspeccionaron registros de esos esquemas ni se afirma que sean destinos activos de este despliegue. Tener visibilidad de metadatos no concede autorización para importar en ellos.

## Consecuencia para SQL 01 y adaptadores

- En la configuración inspeccionada, las tres tablas propias y el ALTER del permiso administrativo corresponden a `docuarchi`, origen documental RA. No hay un bloque DDL para Workflow o DA separado en esta entrega.
- Los SELECT del script se conservan como controles previos de aplicación manual. Ya no representan una investigación pendiente sobre existencia en este ambiente; detectan cambio de destino/estructura antes de ejecutar DDL.
- SQL no contiene USE fijo: otro despliegue debe verificar su enrutamiento real, no copiar el nombre observado aquí.
- En este ambiente, los consumidores RA y DA de `system1` alcanzan el mismo esquema reportado. En otros despliegues deben seguir usando su conexión correspondiente; no introducir una equivalencia global de IDs entre bases distintas.
- Sigue siendo necesario caracterizar el consumidor exacto de gabinete/índices en el adaptador operativo. La localización física está verificada aquí; no se confunde con una prueba de almacenamiento, permisos o consistencia transaccional.

No se ejecutaron CREATE, ALTER, UPDATE, concesiones, login/E2E ni almacenamiento.

# Diagnóstico de gabinetes incompletos en Perfiles de importación

El desplegable **no consulta todos los gabinetes de `system1`**. Consulta los autorizados para **almacenar imágenes** al usuario DocuArchi relacionado con el usuario de Gestión autenticado, por permiso directo o por grupo. No tiene un límite de nueve elementos ni filtra los gabinetes por la serie/subserie seleccionada.

La grabación muestra nueve opciones y un resultado de base con 41 filas. El mecanismo de exclusión está confirmado en el código; para atribuir la ausencia concreta del gabinete 43 a un permiso/referencia/origen determinados falta comprobar los datos efectivos de la sesión y sus permisos. El video no captura esas consultas ni la respuesta HTTP del catálogo. No se alteran permisos, filtros ni almacenamiento con este diagnóstico.

| Referencia | Evidencia |
|---|---|
| Fecha y código revisado | 2026-11-26; `6f7743e7` |
| Grabación | `Grabación 2026-11-26 180106-gabinetes-incompletos.mp4`, 13,74 segundos, 1624×892, 30 fps |
| 0–3,6 segundos | «Gabinete DA» muestra CONTABIL, CORRESPO, CREDITO, ESAL, MERCANTIL, PAGOS, PRODUCIONDOC, PROTOCOLO y RUP. |
| 6–13 segundos | El resultado de `system1` indica 41 filas; se ven gabinetes que no están en el desplegable. |
| Gabinete 43 | Nombre visible iniciado por `ORDENREPARAC…`; indicadores de inventario/TRD/unidad en 1. El nombre completo no se infiere de una columna recortada. |
| Revisión realizada | Video, código, especificación y pruebas locales de cliente/editor. Sin consultas al ambiente, inicio de sesión, carga o cambios de datos. |

## Recorrido exacto del catálogo

1. [PerfilImportacionEditor.js](../../../js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js), línea 303: crea el selector «Gabinete DA» con catálogo `GABINETE` y sin dependencias de serie/área.
2. El editor solicita `ConsultarCatalogo`, página 1 y tamaño 50, en la línea 237. Hay botón «Más resultados»; se habilita si una página trae 50 elementos, línea 241. Al recibir nueve no solicita otra página.
3. [WebServiceImportacionCarpetas.asmx.vb](../../../webservice/WebServiceImportacionCarpetas.asmx.vb), línea 214: delega al adaptador con contexto administrativo autorizado. No llama a `WebServiceRadicacion.asmx` para este catálogo.
4. [PerfilImportacionCatalogAdapter.vb](../../../Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.vb), líneas 175–178: el caso `GABINETE` usa **DA**, resuelve el usuario DA y consulta `system1` con `PermisoGabinete`.
5. El mismo archivo, líneas 219–226: resuelve `remit_dest_interno.RELACION_DA` en RA para el usuario/empresa actuales y comprueba en DA la relación inversa `usuarios_da.Relacion_Gestion`.
6. La constante `PermisoGabinete`, línea 22, exige una de estas condiciones:

```text
permiso directo del usuario DA para s.NOMBRE con ALMACENA_IMAGEN = 1
    O
pertenencia a un grupo con permiso para s.NOMBRE y ALMACENA_IMAGEN = 1
```

Tablas utilizadas: `permisos_usuarios_gabinetes`; `permisos_grupos_gabinetes`; `relacion_usu_grup`. La comparación usa el nombre del gabinete. No basta con que exista su ID en `system1` ni con estar autorizado a consultar documentos.

La consulta final ordena por nombre/ID y aplica `LIMIT @limite OFFSET @offset`, líneas 186–188. El catálogo valida un tamaño máximo de 50. En el video se observan 41 filas totales en la tabla: si ese fuera el mismo origen y todas fueran autorizadas, cabrían en una sola página. La paginación no explica por sí sola que aparezcan nueve.

## Permisos distintos que no se sustituyen

| Condición | Qué acredita | Qué no concede por sí sola |
|---|---|---|
| `ADMINISTRAR_PERFILES_IMPORTACION` | Acceder a administrar perfiles; [PerfilImportacionContextAdapter.vb](../../../Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionContextAdapter.vb) lo reconsulta. | Permiso DA de almacenamiento en cada gabinete. |
| `RELACION_DA` y relación inversa | Qué usuario DA se usa para ese usuario de Gestión. | Acceso a todos los gabinetes ni equivalencia con el usuario del cliente SQL. |
| `ALMACENA_IMAGEN = 1`, directo o grupo | Elegibilidad actual del gabinete para aparecer en ese desplegable. | Que inventario/TRD/unidad estén configurados para ejecutar una importación. |
| `INVENTARIO_DOCUMENTAL = 1`, `APLICA_TRD = 1`, `ASIGNA_UNIDAD = 1` | Condiciones comprobadas por `AutorizarGabineteImportacion`, líneas 231–241, antes de efectos operativos. | Que el gabinete aparezca si el usuario DA carece del permiso anterior. |

El administrador conectado al cliente SQL puede ver las 41 filas; eso no demuestra que el usuario DA vinculado a la aplicación tenga `ALMACENA_IMAGEN` en todas ellas. El catálogo no usa el usuario del cliente SQL como identidad de negocio.

## Hallazgo adicional de coherencia

La lista de configuración aplica el permiso DA, pero no incluye en su SQL los tres indicadores operativos. Por eso puede ofrecer un gabinete autorizado cuyo almacenamiento luego sea incompatible. En la grabación se ve PAGOS en la lista y, en el resultado mostrado, con indicadores en cero; la coincidencia de origen/configuración debe verificarse antes de atribuir un fallo de ambiente.

Son dos condiciones distintas: **visibilidad por permiso** y **compatibilidad para ejecutar**. No resolver una ausencia quitando el permiso ni activando indicadores automáticamente. Si se quisiera que un administrador configure gabinetes sin permiso personal de almacenamiento, habría que definir una autoridad administrativa específica y conservar la reautorización del operador real; no es una corrección puramente visual ni está aprobada por esta consulta.

## Comprobación puntual de solo lectura

Las consultas siguientes están preparadas para un ejecutor parametrizado autorizado; `@usuarioGestion`, `@empresa` y `@usuarioDA` no tienen valores reales en este documento. No ejecutarlas sin enlazar los parámetros ni interpretar resultados con parámetros nulos como diagnóstico válido. No contienen SET, INSERT, UPDATE ni concesiones de permisos.

En la conexión **RA** de la aplicación:

```sql
SELECT u.id_Remit_Dest_Int AS usuario_gestion,
       u.Empresa_Gestion_Documental_id_empresa AS empresa,
       u.Estado_Usuario,
       u.RELACION_DA AS usuario_da
FROM remit_dest_interno u
WHERE u.id_Remit_Dest_Int = @usuarioGestion
  AND u.Empresa_Gestion_Documental_id_empresa = @empresa;
```

En la conexión **DA** de la aplicación, con el ID anterior:

```sql
SELECT u.Clave_Usuario, u.Relacion_Gestion
FROM usuarios_da u
WHERE u.Clave_Usuario = @usuarioDA
  AND u.Relacion_Gestion = @usuarioGestion;
```

Después, en **DA**, comprobar todos los motivos por gabinete:

```sql
SELECT s.id_gabinete,
       s.NOMBRE,
       s.INVENTARIO_DOCUMENTAL,
       s.APLICA_TRD,
       s.ASIGNA_UNIDAD,
       EXISTS (
           SELECT 1
           FROM permisos_usuarios_gabinetes pu
           WHERE pu.USUARIOS_DA_Clave_Usuario = @usuarioDA
             AND pu.NOMBRE_GABINETE = s.NOMBRE
             AND pu.ALMACENA_IMAGEN = 1
       ) AS permiso_directo,
       EXISTS (
           SELECT 1
           FROM permisos_grupos_gabinetes pg
           INNER JOIN relacion_usu_grup ru
             ON ru.GRUPOS_DA_Clave_Grupo = pg.GRUPOS_DA_Clave_Grupo
           WHERE ru.USUARIOS_DA_Clave_Usuario = @usuarioDA
             AND pg.NOMBRE_GABINETE = s.NOMBRE
             AND pg.ALMACENA_IMAGEN = 1
       ) AS permiso_por_grupo
FROM system1 s
ORDER BY s.NOMBRE, s.id_gabinete;
```

Para el caso observado, localizar `id_gabinete = 43` en esa salida. Si ambos permisos son cero, la exclusión corresponde exactamente al filtro actual. Si alguno es uno, comprobar que la relación inversa es válida, que se consultó el mismo origen DA y que el catálogo volvió a solicitarse después del cambio. La lista del navegador no se actualiza por abrir la flecha nativa del selector: carga opciones al renderizar/consultar el perfil. Conservar cambios pendientes antes de reabrirlo.

Si las condiciones se cumplen y sigue ausente, capturar en la sesión del usuario la respuesta de `ConsultarCatalogo` para `GABINETE` y la versión desplegada. No se concluye que haya un defecto de permisos a partir de la grabación solamente ni se diagnostica la base por un nombre visual del servidor.

## Verificación local y límites

Pasaron 19 comprobaciones existentes con:

```powershell
node --test tests/doc110-perfil-importacion-editor.test.cjs tests/doc110-perfil-importacion-client.test.cjs
```

Cubren transporte, catálogos, paginación, referencia guardada fuera de página, errores y respuestas antiguas. Usan un catálogo sintético; no verifican los permisos reales del gabinete 43 ni sustituyen la lectura anterior. La [especificación del perfil](../../../openspec/specs/produccion-importacion-carpeta-perfil/spec.md) mantiene autorización por recurso; la instalación es de una sola empresa y no requiere inventar otra para diagnosticar este caso.

Frames y metadatos del video permanecen en `.tmp/diagnostico-gabinetes-perfiles-20261126/`, excluido de Git. No se copiaron imágenes con datos de personas al informe. No se modificó código de aplicación, perfiles, permisos, configuración ni funciones de almacenamiento.

Conclusión: la causa implementada de la lista reducida es el **filtro de almacenamiento por usuario DA/grupo**, no la cantidad total de gabinetes ni la selección de serie. Para confirmar el motivo puntual del gabinete 43 se necesita la lectura de relación/permisos/origen preparada arriba, sin alterar el motor de almacenamiento.

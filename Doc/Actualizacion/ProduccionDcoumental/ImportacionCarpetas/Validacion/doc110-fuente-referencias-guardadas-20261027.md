# DOC-110: revocación y referencias guardadas no disponibles

Estado vigente: **15.6 y 15.13 completas**. La comprobación autenticada de referencias publicadas con fuente revocada pasó el 2026-10-27: 1 passed (17.4 s). Los estados de ejecución pendiente y fallo del assert siguientes se conservan como historial; resultado final al final.

Fecha: 2026-10-27. Verificación de 15.6 y avance de 15.13, D-09/RQ-12.

## Revocación e historial: 15.6

La [aceptación autenticada acumulada](doc110-fuentes-aceptacion-preparada-20261026.md) completa la verificación primaria de 15.6: seis revisiones con actor, fecha UTC, motivo y snapshots; cuatro entradas anteriores comparadas íntegramente. Tras revocar, ValidarProveedor exige 403 ACCESO_DENEGADO sin datos y los documentos completos del proveedor, ambas publicaciones del perfil e historial permanecen intactos. Lectura final confirma fuente inactiva, sin borrar versiones ni modificar DA.

Resultado real aportado por el usuario: **1 passed (23.8 s)**; reporte fuente PASS al `2026-10-27T11:42:04.392Z`. El arnés .NET conserva la prueba de que revocar no necesita consultar el origen y exige contexto/capacidad válidos. No acredita rollback real ante fallo de historial ni concurrencia de fuentes: criterios pendientes en 15.5/15.14.

## Diagnóstico del editor: 15.13

El editor conservaba una opción «Referencia guardada» para valores ausentes, pero ante una columna retirada podía anunciar «Opciones autorizadas disponibles» y ante conexión/tabla retirada continuaba consultando catálogos descendientes.

Dos regresiones nuevas fallaron antes del cambio. ProveedorAdminEditor ahora informa «Referencias guardadas no disponibles» y conserva los valores originales. Si falta conexión o tabla guardada, detiene la carga descendiente; si falta clave, columna empresarial o propiedad compatible, conserva la selección con aviso. No elige otra referencia ni guarda/publica automáticamente. Publicados siguen en solo lectura. La autorización y validación del servidor permanecen obligatorias.

Referencia ASPX versionada `doc110-source-diagnostics-20261027-1`. Sin cambios VB/proyecto, SQL ni despliegue.

## Pruebas locales

```powershell
node --test tools/e2e/tests/doc110-provider-source-diagnostics.test.cjs tests/doc110-proveedor-admin-editor.test.cjs tests/doc110-fuente-admin-editor.test.cjs tests/doc110-perfil-importacion-behavior.test.cjs
```

**12 PASS**, incluido arnés .NET Framework. Referencias conservadas ante conexión/tabla retirada, columna ausente sin sustitución/tipo alterado, ausencia de escrituras, consultas descendientes evitadas, respuestas atrasadas y descubrimiento acotado de proveedor E2E para el diagnóstico.

Desde tools/e2e:

```powershell
npx.cmd playwright test tests/doc110-fuentes-guided.spec.cjs tests/doc110-perfil-guided.spec.cjs --config=playwright.config.cjs
```

**5 passed (9.7 s)** con transporte simulado. Tras revocar fuente, reabre proveedor publicado: aviso visible, conexión/tabla/clave/propiedad intactos, sin Guardar ni cambios en su documento. Regresión de fuentes/perfiles/proveedores conservada. Sintaxis Node y OpenSpec estricto comprobados.

## Comprobación HTTP de solo lectura preparada

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --provider-diagnostics --source-table imp_02_creditos --headed
```

Reutiliza sesión, consola, configuración y reporteros existentes. Busca solo proveedor E2E publicado inactivo de esa tabla con lecturas acotadas y confirma fuente revocada. Abre por UI y exige aviso, referencias originales, configuración publicada de solo lectura, documento posterior idéntico, cero solicitudes de escritura y ninguna consulta de columnas de la tabla retirada desde la UI. No activa ni revoca, no cambia permisos/datos ni requiere claves de consulta. Sin fixture se detiene; no crea otro.

Reporte saneado `doc110-provider-source-diagnostics-result.json`, solo estado y fechas, sin datos de negocio ni secretos. HTTP nuevo **NO_EJECUTADO** hasta recibir resultado. 15.13 permanece abierta: el PASS previo del ciclo fuente→proveedor→perfil no acredita por sí solo el aviso nuevo.

## Corrida autenticada y corrección del assert, 2026-10-27

Resultado aportado por el usuario: **1 failed, caso 33.9 s**. Descubrimiento del proveedor publicado inactivo y fuente revocada aprobados; se alcanza el detalle y pasan las aserciones previas de aviso y referencias originales. Falla el driver al usar toBeDisabled sobre el propio fieldset, aunque el DOM muestra `<fieldset disabled class="doc110-provider-form">`. No demuestra un formulario editable: la comprobación del contenedor no expresa correctamente la semántica de sus controles. La comparación posterior del documento y las aserciones finales aún no se alcanzaron.

Reporte local FAIL, inicio `2026-10-27T12:00:36.792Z`, fin `2026-10-27T12:00:52.332Z`. El recorrido es de solo lectura y no ejecutó operaciones de mutación. No se declara aceptación completa por las comprobaciones parciales.

Se extrae assertReadOnlyProvider, usado por el driver autenticado y por la suite Chromium: exige atributo disabled del fieldset, al menos un control interno, cada input/select/textarea/button efectivamente deshabilitado y ausencia de Guardar. Se prueba también que rechaza un fieldset editable aunque su input esté deshabilitado y un control habilitado dentro del primer legend de un fieldset disabled. No se cambia producto, permiso ni protección para hacer pasar la prueba.

Validación de la corrección: **2 Node y 7 Chromium PASS**, incluidos proveedor publicado real del fixture UI, referencias tras revocación y casos negativos del assert. Transporte simulado; sintaxis y OpenSpec estricto PASS. Repetición IIS pendiente bajo la misma autorización de solo lectura; 15.13 permanece abierta.

## Diagnóstico autenticado aprobado, 2026-10-27

Resultado aportado por el usuario: **1 passed (17.4 s)**, caso **16.4 s**, de --provider-diagnostics --source-table imp_02_creditos --headed. Reporte local PASS, inicio `2026-10-27T12:03:51.231Z`, fin `2026-10-27T12:03:51.923Z`.

Se localiza un proveedor E2E publicado inactivo de créditos y se confirma su fuente revocada. Al abrirlo por UI aparece el diagnóstico; conexión, tabla, clave, columna empresarial y columnas de propiedades conservan los valores publicados. El fieldset tiene disabled y cada control interno está efectivamente deshabilitado; no existe Guardar. La lectura posterior devuelve un documento completo idéntico. Se verifican cero solicitudes de escritura y cero solicitudes de columnas de la tabla retirada desde la UI. Marcador REVOKED_SOURCE_UI_DIAGNOSTIC_PRESERVES_PUBLISHED_REFERENCES_WITHOUT_WRITES aprobado.

Esta corrida completa el diagnóstico pendiente de 15.13. Se suma a la aceptación previa del catálogo: fuente registrada por UI disponible en el proveedor, FECHADOCUMENTO retirada de sus metadatos tras editar y fuente exclusiva con columna empresarial vacía guardada/consultada correctamente. Evidencia acumulada en [aceptación de fuentes](doc110-fuentes-aceptacion-preparada-20261026.md). No se ejecutan mutaciones en esta comprobación, ni se vuelven a activar la fuente o sus consumidores. No acredita aislamiento empresarial, concurrencia o rollback real de fuentes.

# DOC-111: ciclo Archivo Gestión, 2026-10-29

Decisión del usuario: «si archivo gestion», en respuesta a elegir entre reutilizar «Archivo Gestión» de Producción o configurar el ciclo por perfil. Se reutiliza esa política existente. No se interpreta como autorización para crear expedientes o ejecutar E2E.

El comando de creación captura CicloArchivoNombre = Archivo Gestión como propiedad de solo lectura. ResolverDatosCreacionExpediente del catálogo compartido resuelve su ID real mediante SELECT parametrizado por nombre, con LIMIT 2. La fila debe ser única y devolver el nombre exacto; no se fija ID, no se cambia de ciclo por ausencia y no se agregan campos/selectores al perfil administrativo. PrepararDatosCreacionLegacy exige además ID en rango Integer y el conjunto cerrado de datos con CICLO_ARCHIVO_ID/NOMBRE.

Validación PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: familias previas y casos nuevos con SELECT sintéticos. Verifica ciclo exacto, ausencia, duplicidad, nombre distinto, ID no canónico/fuera de Integer, columna ausente y datos de ciclo faltantes o alterados.
- Build Debug completo sin despliegue; salida/intermedios `.tmp/doc111-ciclo-gestion-build/{bin,obj}`, log `.tmp/doc111-ciclo-gestion-build.log`, código 0.
- Inspector existente ejecutado en modo Doc111Creation, reutilizando configuración y perfil 24/versión 38 del destino DOC-111 fijado. Solo SELECT, sin activación ni cambios. Resultado INSPECTED, una coincidencia de ciclo, una de clasificación y una ruta configurada. Reporte saneado local en `tools/e2e/artifacts/doc111-creation-compatibility-select.json`, terminado 2026-10-29T11:36:01.9293279Z.

La lectura real devolvió ID 4 y nombre exacto Archivo Gestión. El ID es evidencia de este ambiente, no configuración fija del código. Esta ejecución vuelve a producir el reporte local del inspector; la evidencia del 2026-10-28 conserva su propio corte histórico.

**95/130 tareas completas, sin incremento**: resuelto el pendiente de política del ciclo, pero 9.3/9.4 aún requieren composición/invocación final, barreras y comprobación/correlación del resultado. El ciclo resuelto debe acompañar la futura evidencia; no se certifica que un expediente lo haya persistido por leer el catálogo.

Sin HTTP/E2E autenticados, creación, almacenamiento, escrituras SQL, cambios de configuración o despliegue. ClassGaExpediente, ClassGaProducionDocumental, ClassAlmacenamiento, ClassNeodynamic y conect permanecen intactos; sin cambios de SII.

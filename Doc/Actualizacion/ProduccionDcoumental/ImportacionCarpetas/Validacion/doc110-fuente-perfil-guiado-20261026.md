# DOC-110: recorrido guiado fuente → proveedor → perfil → ensayo

Fecha: 2026-10-26. Ampliación de la suite existente `tools/e2e/tests/doc110-fuentes-guided.spec.cjs`, tarea 15.15 (D-09/RQ-12). Interfaz y bibliotecas reales con transporte simulado compartido por los tres editores. No inicia login, ASMX ni conexiones RA/DA.

## Cobertura entregada

La prueba registra una fuente DA inactiva desde la interfaz, selecciona columnas, activa la autorización y edita sus columnas conservando identidad. Crea y configura un proveedor desde UI, publica su versión y activa su disponibilidad. El editor de perfil selecciona esa versión/propiedad para un índice, guarda/reabre el borrador y ensaya el valor sintético. Verifica que la regla guardada conserva proveedor, versión y propiedad.

Tras revocar la fuente desde UI, compara documentos completos de proveedor y perfil, sin cambios. Comprueba las cuatro revisiones del historial de fuente (alta, activación, edición, revocación) y snapshot final inactivo. Reabre el perfil con su referencia original y verifica el diagnóstico de ensayo denegado, sin sobrescribir el borrador. Un nuevo proveedor ya no recibe opciones de la fuente revocada. Se conserva el escenario previo de error al crear por esquema incompleto con formulario intacto y sin reenvío.

El doble usa una sola fuente de estado para metadatos y consumidores y devuelve copias de documentos. Los valores/IDs son sintéticos. El mock devuelve el resultado de ensayo para probar el recorrido visual; no ejecuta resolvedor, consulta de negocio ni validación del servidor. El perfil se conserva como BORRADOR: esta prueba no acredita publicación de configuración completa.

Las pruebas existentes de componentes complementan las de navegador: respuesta antigua RA/DA/RA no sustituye opciones vigentes, cambiar tabla descarta columnas y escritura incierta no se repite; proveedor/perfil conservan descarte de respuestas atrasadas y referencias guardadas. No se introducen login, framework, configuración ni runner paralelos.

## Ejecución

Desde la raíz:

```powershell
node --test tests/doc110-fuente-admin-editor.test.cjs tests/doc110-proveedor-admin-editor.test.cjs tests/doc110-perfil-importacion-editor.test.cjs
node --check tools/e2e/tests/doc110-fuentes-guided.spec.cjs
```

**18 PASS**, 0 fallos; sintaxis PASS.

Desde `tools/e2e`:

```powershell
npx.cmd playwright test tests/doc110-fuentes-guided.spec.cjs tests/doc110-perfil-guided.spec.cjs --config=playwright.config.cjs
```

**4 passed (8.8 s)**: dos escenarios de fuentes y regresión guiada de perfil/proveedor. En la primera iteración una aserción de prueba buscó disponibilidad en el título del proveedor; se corrigió para comprobar el control real «Desactivar proveedor». No fue necesario cambiar código productivo. Capturas/trazas de esta suite contienen datos simulados.

OpenSpec estricto y diff sin errores de espacios comprobados tras actualizar la evidencia. Se cambian la suite y documentación; sin SQL, despliegue, permisos ni cambios SII.

## Pendiente de aceptación real

15.15 completa como automatización del recorrido con transporte simulado; **15.16 permanece NO_EJECUTADO** en esta continuación. El E2E de proveedores previamente aprobado no registra, edita ni revoca fuentes y no sustituye esta aceptación.

Antes de ejecutar el ciclo real se requiere confirmar esquema SQL 07 completo y facultad de fuentes, además de ambiente, cuentas y una autorización descartable que pueda crearse/editarse/revocarse sin afectar consumidores existentes. No revocar por inferencia la fuente compartida de `imp_02_egresos` utilizada en las pruebas anteriores. Si su identidad ya está registrada, crearla otra vez no es un alta válida; el equipo debe precisar un fixture aislado o un procedimiento autorizado sobre esa fuente. No borrar registros ni insertar semillas para eludir la UI. No se han autorizado nuevas mutaciones del catálogo en esta sesión.

# DOC-119 — selección de importaciones existentes

El usuario indicó que no comprendía qué «lotes» debía proporcionar. En este módulo un lote representa una importación de carpetas ya realizada. Se corrige la captura para mostrar opciones comprensibles, sin exigir referencias internas.

## Recorrido

1. Capturar DSN, decisión TLS y credenciales en consola; claves y cuentas enmascaradas.
2. Autenticar al propietario con la sesión existente. Hasta elegir una importación, la guardia no permite llamadas ASMX de importación.
3. Resolver cuenta/empresa mediante SELECT parametrizado y mostrar hasta 30 importaciones propias con facultades actuales: fecha UTC, destino, estado y cantidades. Elegir el número de una opción visible.
4. Ofrecer recuperación solo para importaciones con archivos por recibir; mostrar el archivo esperado y permitir omitir si no está disponible. Otro destino se elige por nombre.
5. Reutilizar la sesión propietaria en SEG-01 y solicitar la segunda cuenta al llegar a privacidad. Las consultas de descubrimiento consumen los mismos presupuestos, sin reset.

El plan autorizado sigue limitado a lectura, CSV y reselección local. No crea importaciones ni envía archivos. Si no hay datos adecuados para paginación/recuperación, se informa la limitación y no se fabrica aceptación.

## Verificación local

`node --test tests/doc111-importacion-carpetas-client.test.cjs tools/e2e/tests/doc119-seguimiento-policy.test.cjs`: **39/39 PASS**, 21 cliente/selector y 18 políticas. Son pruebas locales con servicios simulados; no acreditan aceptación real del ambiente. `--check` y sintaxis se verifican aparte.

La captura anterior `bb0f827ea00c4f259725092a996e0860` se cerró con `REPLACED_BEFORE_EXECUTION`: sin sesiones, HTTP ni SELECT consumidos. Su registro permanece en `.tmp` excluido de Git. No se reinició una ventana usada ni se borró evidencia.

Progreso OpenSpec: **4/24**. SEG-01/02 pendientes de ejecución y revisión con los recursos elegidos; SEG-03 sigue sin autorizar limpieza. Esta corrección afecta únicamente herramientas/documentación de aceptación.

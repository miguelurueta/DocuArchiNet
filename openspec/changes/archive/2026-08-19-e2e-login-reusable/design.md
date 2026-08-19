## Context

Las suites de `tools/e2e/tests/` crean por separado un `BrowserContext`, cargan `gestor.aspx`, seleccionan un módulo y disparan el postback Web Forms. Esa repetición hace que correcciones como tiempos de espera, HTTPS o manejo de cookies deban aplicarse en más de un archivo. La motivación y el alcance funcional se describen en `proposal.md`.

La utilidad debe preservar el aislamiento por prueba: cada llamada crea un contexto nuevo y entrega sus cookies autenticadas al llamador. Ningún secreto puede escribirse en código, evidencias, capturas ni salida estándar. La ejecución de una transición continúa siendo responsabilidad de la prueba que la solicita y conserva sus controles de autorización.

## Goals / Non-Goals

**Goals:**

- Proveer un único helper CommonJS bajo `tools/e2e/tests/` que inicie una sesión autenticada de Gestión Documental con Playwright.
- Admitir los prefijos de variables actuales de DOC-10 y DOC-11 mediante una configuración explícita, sin duplicar credenciales ni nombres de entorno.
- Encapsular selección de módulo, postback de `gestor.aspx`, espera de respuesta y limpieza del contexto cuando el login falla.
- Ofrecer pruebas locales con dobles de Playwright que cubran configuración válida, falta de variables y liberación del contexto ante fallo.

**Non-Goals:**

- No modificar el formulario de login, autenticación Forms, endpoints ASMX, tareas, datos, gate ni configuración de producción.
- No crear archivos `.env`, almacenar `storageState`, cookies, contraseñas o cuerpos de respuesta.
- No habilitar ni ejecutar E2E autenticada, carga o transiciones reales como parte de este cambio.

## Decisions

### Helper CommonJS con configuración explícita

Se creará un módulo común que reciba el `browser` y un objeto con URL base, módulo, nombres de variables de usuario/contraseña, y la opción de ignorar errores HTTPS. El módulo resolverá las variables solo al iniciar la sesión, validará presencia sin revelar valores y devolverá el `BrowserContext` autenticado.

Se evita acoplarlo a `DOC10_*` o `DOC11_*`: cada suite mantiene su contrato de entorno y aporta sus propias claves. Como alternativa se consideró un helper que lea todos los prefijos automáticamente; se descarta porque ocultaría qué identidad usa una prueba y volvería ambigua la migración de futuras suites.

### Bootstrap Web Forms como única implementación de login

El helper navegará a `gestor.aspx`, seleccionará el módulo, llenará los controles existentes y esperará el POST de autenticación antes de devolver el contexto. En caso de excepción cerrará el contexto creado y emitirá un error público de prueba sin incluir la contraseña, cookies ni HTML de respuesta.

Se descarta reutilizar manualmente cookies o llamar a un endpoint de autenticación alterno: ambos caminos esquivarían el bootstrap que inicializa la sesión de Workflow y no representarían el comportamiento del usuario.

### Propiedad y cierre de recursos en el llamador

La prueba consumidora conserva la propiedad del `BrowserContext` exitoso y lo cierra en `finally`. El helper solo lo cierra si no logra completar el login. Así, las llamadas ASMX del `context.request` comparten la sesión autenticada y cada prueba puede decidir si navega la UI, llama preview o ejercita una validación no mutante.

Se descarta devolver `page` o una sesión global compartida: una página no es necesaria para todas las pruebas y un estado compartido propagaría cookies y fallos entre casos.

### Pruebas unitarias sin secretos ni red

Se añadirá una prueba Node que simule `browser`, `context` y `page` para verificar selectores, contrato de configuración y limpieza ante errores. Las suites DOC-10 y DOC-11 se migrarán al helper y sus pruebas existentes seguirán cubriendo sus endpoints.

Se descarta validar esta refactorización mediante una E2E autenticada: tal ejecución requiere una autorización distinta y no es necesaria para verificar el contrato local del helper.

## Risks / Trade-offs

- [El formulario Web Forms cambia sus selectores o su postback] → Mantener los selectores centralizados y una prueba unitaria que haga visible el contrato roto.
- [Una suite omite cerrar el contexto retornado] → Conservar los bloques `try/finally` existentes y documentar la propiedad del recurso.
- [Un mensaje de error revela información sensible] → El helper usa mensajes fijos y no serializa respuestas, cookies ni variables de entorno.
- [Una futura suite necesita un flujo de login distinto] → Añadir otro helper o una opción deliberada solo cuando ese flujo esté documentado; no ampliar implícitamente el bootstrap actual.

## Migration Plan

1. Añadir el helper y su prueba local.
2. Migrar las suites DOC-10 y DOC-11 para consumirlo, sin cambiar sus variables de entorno ni semántica de sus pruebas.
3. Ejecutar las pruebas CJS de `tools/e2e` y las suites afectadas sin secretos ni acceso autenticado.
4. Si aparece una regresión, restaurar las funciones de login locales; el cambio no altera datos ni requiere migración o rollback de configuración.

## 1. Perfil y registro reutilizable

- [x] 1.1 Crear el lector y validador estricto de perfiles JSON externos, con lista de claves no sensibles y rechazo sin eco de campos desconocidos o sensibles. Verificación: pruebas unitarias aceptan un perfil DOC-32 válido y rechazan contraseña, cookies, tokens, cadenas de conexión y autorizaciones.
- [x] 1.2 Definir un registro confiable de DOC que declare esquema de perfil, etapas ordenadas, variables efímeras y autorizaciones; registrar DOC-32 como preview, ejecución y concurrencia. Verificación: un perfil no puede seleccionar comandos, scripts ni DOC no registrado.
- [x] 1.3 Implementar el análisis de `--doc`, `--profile` y `--authorize`, con rechazo previo a cualquier sesión cuando falte una autorización requerida. Verificación: una autorización del perfil no habilita una etapa y los flags incompletos bloquean antes de recuperar secretos.

## 2. Secretos efímeros de una sola captura

- [x] 2.1 Adaptar la consola interactiva existente para capturar una sola vez los secretos compartidos por la secuencia y fallar antes de iniciar E2E si no existe TTY. Verificación: una prueba simulada cubre TTY ausente, valor faltante y entrada oculta sin registrar valores.
- [x] 2.2 Mapear los secretos capturados únicamente al entorno efímero del DOC, sin escribir credenciales ni cadenas de conexión en disco, variables de usuario o argumentos. Verificación: la prueba focal confirma que los procesos hijos reciben el contrato esperado y que el perfil no contiene secretos.
- [x] 2.3 Garantizar limpieza de las variables efímeras y de referencias sensibles en los caminos de éxito, error e interrupción. Verificación: prueba focal verifica limpieza después de una etapa que falla.

## 3. Orquestación DOC-32 en un comando

- [x] 3.1 Crear el iniciador común de Workflow que valide perfil, registro, autorización, consultas SELECT, presupuestos y gate antes de recuperar secretos. Verificación: una validación fallida no abre navegador, sesión Gestión ni conexión MySQL.
- [x] 3.2 Implementar la ejecución secuencial DOC-32 que reutiliza la sesión efímera para preview, ejecución y concurrencia, y detiene etapas posteriores ante error o presupuesto excedido. Verificación: la prueba de orquestación comprueba el orden y que una falla de preview impide ejecución y concurrencia.
- [x] 3.3 Aplicar los controles de gate y de páginas legacy al inicio y en `finally`, preservando la evidencia saneada de las etapas realizadas. Verificación: la prueba de interrupción comprueba cierre, ausencia de secretos y sin cambios en páginas legacy.
- [x] 3.4 Publicar `test:workflow:run` en `tools/e2e/package.json`, mantener los comandos DOC-32 existentes y comprobar compatibilidad. Verificación: el nuevo comando y los comandos específicos se validan sin iniciar E2E real cuando faltan datos o autorizaciones.

## 4. Pruebas de seguridad y regresión

- [x] 4.1 Añadir pruebas unitarias para perfiles, registro DOC, autorizaciones, secuencia y limpieza de secretos usando consola simulada. Verificación: la suite no requiere TTY, navegador, sesión ni MySQL reales.
- [x] 4.2 Extender las pruebas de política DOC-32 para impedir secretos, cadenas de conexión, banderas de autorización y comandos arbitrarios en perfiles y evidencia. Verificación: las pruebas detectan cada categoría prohibida sin mostrar el contenido de prueba.
- [x] 4.3 Ejecutar las pruebas focales existentes de backend DOC-32 y de política E2E junto con la nueva suite del orquestador. Verificación: todas superan y no se inicia una E2E autenticada.

## 5. Documentación y adopción

- [x] 5.1 Incluir una plantilla de perfil JSON sin valores reales y documentar sus campos permitidos, captura interactiva única y flags de autorización no persistentes. Verificación: la plantilla no contiene secretos, cookies, tokens, URL MySQL ni autorizaciones.
- [x] 5.2 Actualizar `tools/e2e/AGENT-RUNBOOK.md` con el comando unificado, la captura efímera única, los controles por DOC y el cierre obligatorio. Verificación: el runbook prohíbe secretos en perfiles y conserva gate apagado, listas vacías y consultas SELECT de un parámetro.
- [x] 5.3 Documentar cómo registrar un DOC futuro sin cambiar la semántica ni las pruebas propietarias de su suite. Verificación: la guía enumera contrato de perfil, etapas, autorizaciones, evidencia y pruebas de política necesarias.

## 6. Validación autorizada

- [x] 6.1 Ejecutar el comando unificado DOC-32 contra el ambiente autorizado, usando un perfil no sensible e ingreso interactivo de la cuenta aprobada. Verificación: preview, devolución y carrera sobre las dos tareas descartables producen evidencia saneada, respetan presupuestos y confirman gate apagado y páginas legacy sin cambios.
